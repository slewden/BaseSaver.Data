using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml.Linq;
using BaseSaver.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseSaverTest
{
  /// <summary>
  /// Test de l'objet ConnectionParam
  /// </summary>
  [TestClass]
  public class TestConnectionParam : TestUnitaireBase
  {
    /// <summary>
    /// La chaine de connexion à utiliser
    /// </summary>
    public static string myconnectionString = "Data Source=localhost;Initial Catalog=version_db3;integrated Security=SSPI;Pooling=False";

    /// <summary>
    /// Teste que la connexion fonctionne
    /// </summary>
    [TestMethod]
    public void TestConnectionOk()
    {
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString))
      {
        AssertAreEqual(cnn.ConnexionString, myconnectionString, "La chaine de connexion");
        AssertAreEqual(cnn.TimeOut, 0, "Le timeOut");
        AssertAreEqual(cnn.Connection, null, "La connexion");
        AssertAreEqual(cnn.Transaction, null, "La transaction");

        ConnectionStringInfo infos = new ConnectionStringInfo(myconnectionString);

        AssertAreEqual(cnn.Infos().ToString(), infos.ToString(), "COnnexion info");
      }
    }

    /// <summary>
    /// Teste les méthodes ExecuteNonQuery, Scalar, ...
    /// </summary>
    [TestMethod]
    public void TestNonQueryScalar()
    {
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        AssertAreEqual(cnn.Connection, null, "La connexion");
        AssertAreEqual(cnn.Transaction, null, "La transaction");
        AssertAreEqual(cnn.TimeOut, 200, "Le timeOut");

        cnn.ExecuteNonQuery(@"
IF (EXISTS (SELECT 1 FROM sys.tables WHERE name = '__test_unitaire__ConnectionParam'))
  DROP TABLE __test_unitaire__ConnectionParam
");

        AssertAreEqual(cnn.Connection, null, "La connexion");
        AssertAreEqual(cnn.Transaction, null, "La transaction");
      }

      // connexion maintenue
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, true))
      {
        AssertAreEqual(cnn.Connection, null, "La connexion");
        AssertAreEqual(cnn.Transaction, null, "La transaction");

        cnn.ExecuteNonQuery("CREATE TABLE __test_unitaire__ConnectionParam (id INT IDENTITY, nom VARCHAR(50))");

        Assert.IsNotNull(cnn.Connection, "La connexion est perdue");
        AssertAreEqual(cnn.Transaction, null, "La transaction");

        List<SqlParameter> parameters = new List<SqlParameter>
        {
          new SqlParameter("nom", "Test1"),
          new SqlParameter("toto", null),
          new SqlParameter("x", SqlDbType.Int, 32, ParameterDirection.InputOutput, true, 1, 1, "s", DataRowVersion.Default, null),
          new SqlParameter("y", SqlDbType.Int, 32, ParameterDirection.Input, true, 1, 1, "s", DataRowVersion.Default, null),
          new SqlParameter("z", SqlDbType.Int, 32, ParameterDirection.Input, true, 1, 1, "s", DataRowVersion.Default, 2),
        };

        cnn.OnConnectionInfoMessage += Cnn_OnConnectionInfoMessageInsert; 
        cnn.ExecuteNonQuery("INSERT INTO __test_unitaire__ConnectionParam (nom) VALUES (@nom);", parameters: parameters);
        cnn.OnConnectionInfoMessage -= Cnn_OnConnectionInfoMessageInsert;

        Assert.IsNotNull(cnn.Connection, "La connexion est perdue");
        AssertAreEqual(cnn.Transaction, null, "La transaction");

        int id = cnn.ExecuteInsert("INSERT INTO __test_unitaire__ConnectionParam (nom) VALUES ('TEST2');");

        AssertAreEqual(id, 2, "Id autoincrementé");

        parameters = new List<SqlParameter>
        {
          new SqlParameter("id", 1)
        };
        int nb = cnn.ExecuteGetId("SELECT COUNT(*) + 3 FROM __test_unitaire__ConnectionParam WHERE id = @id", -1, System.Data.CommandType.Text, parameters);

        Assert.IsNotNull(cnn.Connection, "La connexion est perdue");
        AssertAreEqual(nb, 4, "Nombre de lignes + 3 remontées");
        AssertAreEqual(cnn.Transaction, null, "La transaction");
      }
    }

    /// <summary>
    /// Teste les méthodes NonQueryAsync
    /// </summary>
    [TestMethod]
    public void TestAsyncNonQuery()
    {
      string sql = @"
DECLARE @i int = 0;
DECLARE @msg VARCHAR(50);
WHILE @i < 100000
BEGIN
   SET @msg = 'i = '+ Convert(VARCHAR(10), @i);
   IF ((@i % 100 ) = 0) 
    RAISERROR (@msg, 10, 0);
   SET @i = @i+1;
END
";
      int n = 0;
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        n = Task.Run(() => cnn.ExecuteNonQueryAsync(sql)).Result;
      }

      Assert.AreEqual(n, -1, "Resultat ExecuteNonQueryAsync ?");
    }

    /// <summary>
    /// Teste les méthodes ScalarAsync
    /// </summary>
    [TestMethod]
    public void TestAsyncScalar()
    {
      string sql = @"
SELECT 3 AS id
";
      int n = 0;
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        n = SqlConvert.To(Task.Run(() => cnn.ExecuteScalarAsync(sql)).Result, 0);
      }

      Assert.AreEqual(n, 3, "Resultat ExecuteScalarAsync ?");
    }

    /// <summary>
    /// Teste les méthodes DataSetAsync
    /// </summary>
    [TestMethod]
    public void TestAsyncDataSet()
    {
      string sql = @"
SELECT 3 AS id
SELECT 4 as id, 'toto' as name
";
      DataSet dst = null;
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        dst = Task.Run(() => cnn.ExecuteDataSetAsync(sql)).Result;
      }

      Assert.IsNotNull(dst, "Resultat ExecuteDataSetAsync ?");
      Assert.AreEqual(dst.Tables.Count, 2, "Resultat ExecuteDataSetAsync Nombre de table KO");
      Assert.AreEqual(dst.Tables[0].Rows.Count, 1, "Resultat ExecuteDataSetAsync T[0] nbRows KO");
      int n = SqlConvert.To(dst.Tables[0].Rows[0], "id", 0);
      Assert.AreEqual(n, 3, "Resultat ExecuteDataSetAsync Table[0].Rows[0][id] == 3 KO");

      Assert.AreEqual(dst.Tables[1].Rows.Count, 1, "Resultat ExecuteDataSetAsync T[1] nbRows KO");
      n = SqlConvert.To(dst.Tables[1].Rows[0], "id", 0);
      Assert.AreEqual(n, 4, "Resultat ExecuteDataSetAsync Table[1].Rows[0][id] == 4 KO");
      string nom = SqlConvert.To(dst.Tables[1].Rows[0], "name", string.Empty);
      Assert.AreEqual(nom, "toto", "Resultat ExecuteDataSetAsync Table[1].Rows[0][name] == toto KO");
    }

    /// <summary>
    /// Teste le GETID et tous les cas non couverts de ConvertToInt
    /// </summary>
    [TestMethod]
    public void TestGetId()
    {
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString))
      {
        int nb = cnn.ExecuteGetId(@" SELECT NULL", -66);
        AssertAreEqual(nb, -66, "Get ID Not Null");
      }

      using (ConnectionParam cnn = new ConnectionParam(myconnectionString))
      {
        int nb = cnn.ExecuteGetId(@" SELECT 'Robert'", -7);
        AssertAreEqual(nb, -7, "Get ID Not Null");
      }
    }

    /// <summary>
    /// Teste les fonction des DataSet Datatable, ...
    /// </summary>
    [TestMethod]
    public void TestExecuteDataXXX()
    {
      string sql = @"
SELECT 0 AS [Index], 'Doc' AS [Name] UNION SELECT 1, null UNION SELECT 2,  null UNION SELECT 1, 'titi' UNION SELECT -4, 'rien'
;
SELECT object_id, name FROM sys.tables
;";

      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        using (DataSet dst = cnn.ExecuteDataSet(sql, CommandType.Text))
        {
          Assert.IsNotNull(dst, "DataSet initialisé");
          AssertAreEqual(dst.Tables.Count, 2, "Nombre de tables");
          AssertAreEqual(dst.Tables[0].Rows.Count, 5, "Nombre de lignes dans la première table");
          Assert.IsTrue(dst.Tables[1].Rows.Count > 0, "Il n'y a pas de ligne dans la dernière table");
        }

        using (DataTable tbl = cnn.ExecuteDataTable(sql))
        {
          Assert.IsNotNull(tbl, "DataSet initialisé");
          AssertAreEqual(tbl.Rows.Count, 5, "Nombre de lignes dans la première table");
        }

        using (DataSet dst = cnn.ExecuteDataSetWithTableName(sql))
        {
          Assert.IsNotNull(dst, "DataSet initialisé");
          AssertAreEqual(dst.Tables[0].TableName, "Doc", "Nom de la première table");
          AssertAreEqual(dst.Tables[1].TableName, "titi", "Nom de la seconde table");
        }
      }

      // les cas débiles pour la couverture des tests
      sql = @"
SELECT 0 AS [Index], 'Doc' AS [NotName] UNION SELECT 1, null UNION SELECT 2,  null UNION SELECT 1, 'titi' UNION SELECT -4, 'rien'
;
SELECT object_id, name FROM sys.tables
;";
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        using (DataSet dst = cnn.ExecuteDataSetWithTableName(sql))
        {
          Assert.IsNotNull(dst, "DataSet initialisé");
          AssertAreEqual(dst.Tables[0].TableName, "Table", "Nom de la première table");
          AssertAreEqual(dst.Tables[1].TableName, "Table1", "Nom de la seconde table");
        }
      }

      sql = @"
      SELECT 0 AS [NotIndex], 'Doc' AS [Name] UNION SELECT 1, null UNION SELECT 2,  null UNION SELECT 1, 'titi' UNION SELECT -4, 'rien'
      ;
      SELECT object_id, name FROM sys.tables
      ;";
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        using (DataSet dst = cnn.ExecuteDataSetWithTableName(sql))
        {
          Assert.IsNotNull(dst, "DataSet initialisé");
          AssertAreEqual(dst.Tables[0].TableName, "Table", "Nom de la première table");
          AssertAreEqual(dst.Tables[1].TableName, "Table1", "Nom de la seconde table");
        }
      }

      // les cas débiles pour la couverture des tests
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString))
      {
        using (DataTable tbl = cnn.ExecuteDataTable("PRINT 'coucou'"))
        {
          Assert.IsNull(tbl, "Table non nulle");
        }

        using (DataSet dst = cnn.ExecuteDataSetWithTableName("print 'zoby'"))
        {
          Assert.IsNotNull(dst, "Dataset null");
          AssertAreEqual(dst.Tables.Count, 0, "Dataset pas null mais sans tables");
        }

        using (DataSet dst = cnn.ExecuteDataSetWithTableName("SELECT 1 as number"))
        {
          Assert.IsNotNull(dst, "DataSet initialisé");
          AssertAreEqual(dst.Tables[0].TableName, "Table", "Nom de la première table1");
        }

        using (DataSet dst = cnn.ExecuteDataSetWithTableName("SELECT 1 as number; SELECT 2 as number"))
        {
          Assert.IsNotNull(dst, "DataSet initialisé");
          AssertAreEqual(dst.Tables[0].TableName, "Table", "Nom de la première table2");
          AssertAreEqual(dst.Tables[1].TableName, "Table1", "Nom de la seconde table2");
        }
      }
    }

    /// <summary>
    /// Teste les fonctions de transation
    /// </summary>
    [TestMethod]
    public void TestTransaction()
    {
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString))
      {
        cnn.BeginTransaction();

        Assert.IsNotNull(cnn.Connection, "connexion nulle après begin trans");
        AssertAreEqual(cnn.TransactionCount, 1, "Niveau de profondeur des transactions après begin trans");

        cnn.ExecuteNonQuery(@"
IF (NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '__test_unitaire__ConnectionParam_TRANSACTION'))
  CREATE TABLE __test_unitaire__ConnectionParam_TRANSACTION (id int identity, nom varchar(50) NOT NULL);
");
        Assert.IsNotNull(cnn.Connection, "connexion nulle après SQL");
        AssertAreEqual(cnn.TransactionCount, 1, "Niveau de profondeur des transactions après sql");

        cnn.CommitTransaction();

        Assert.IsNull(cnn.Connection, "connexion nulle après commit");
        AssertAreEqual(cnn.TransactionCount, 0, "Niveau de profondeur des transactions après commit");
      }

      using (ConnectionParam cnn = new ConnectionParam(myconnectionString))
      {
        int nbrows = cnn.ExecuteGetId("SELECT count(*) FROM __test_unitaire__ConnectionParam_TRANSACTION", 0);

        cnn.BeginTransaction();

        Assert.IsNotNull(cnn.Connection, "connexion nulle après begin trans N°2");
        AssertAreEqual(cnn.TransactionCount, 1, "Niveau de profondeur des transactions après begin transN°2");

        cnn.ExecuteNonQuery(@"INSERT INTO __test_unitaire__ConnectionParam_TRANSACTION (nom) VALUES ('toto')");
        Assert.IsNotNull(cnn.Connection, "connexion nulle après SQL");

        cnn.BeginTransaction();
        AssertAreEqual(cnn.TransactionCount, 2, "Niveau de profondeur des transactions après begin trans Imbriqué");

        cnn.ExecuteNonQuery(@"INSERT INTO __test_unitaire__ConnectionParam_TRANSACTION (nom) VALUES ('tata')");
        int id = cnn.ExecuteGetId(@"SELECT SCOPE_IDENTITY()", -99);

        Assert.AreNotEqual(id, -99, "SCOPE identity fail");

        int nbrows2 = cnn.ExecuteGetId("SELECT count(*) FROM __test_unitaire__ConnectionParam_TRANSACTION", 0);
        AssertAreEqual(nbrows + 2, nbrows2, "Nombre de lignes incohérent avant rollback");

        cnn.CommitTransaction();
        AssertAreEqual(cnn.TransactionCount, 1, "Niveau de profondeur des transactions après premier commit Imbriqué");

        cnn.RollBackTransaction();

        Assert.IsNull(cnn.Connection, "connexion nulle après roolback");
        AssertAreEqual(cnn.TransactionCount, 0, "Niveau de profondeur des transactions après commit");
        int nbrows3 = cnn.ExecuteGetId("SELECT count(*) FROM __test_unitaire__ConnectionParam_TRANSACTION", 0);
        AssertAreEqual(nbrows, nbrows3, "Nombre de lignes incohérent après rollback");
      }

      // Test auto rollback (plus pour la couverture de code qu'autre chose !!
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, true))
      {
        cnn.BeginTransaction();
      }
    }

    /// <summary>
    /// Teste la méthode Execute XML
    /// </summary>
    [TestMethod]
    public void TestExcecuteXml()
    {
      string sql = "SELECT 'leon' AS Nom, 3 as Id FOR XML RAW('Crug')";
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        XElement result = cnn.ExecuteXml(sql);
        Assert.IsNotNull(result, "Noueud XML non trouvé");
        Assert.AreEqual(result.Name, "Crug", "Mauvais Nom");
        Assert.IsNotNull(result.Attribute("Id"), "id not found");
        Assert.AreEqual(result.Attribute("Id").Value, "3", "Bad id");
        Assert.IsNotNull(result.Attribute("Nom"), "nom not found");
        Assert.AreEqual(result.Attribute("Nom").Value, "leon", "Bad nom");

        sql = "SELECT 'leon' AS Nom, 3 as Id WHERE 1=0 FOR XML RAW('Crug')";
        result = cnn.ExecuteXml(sql, timeOut: 250);
        Assert.IsNull(result, "Noueud XML trouvé");
      }
    }

    /// <summary>
    /// Test l'évenement info message
    /// </summary>
    [TestMethod]
    public void TestInfoMessage()
    {
      string sql = "RAISERROR('coucou', 0, 0)";
      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, timeOut: 200))
      {
        cnn.OnConnectionInfoMessage += Cnn_OnConnectionInfoMessage;
        using (DataTable tbl = cnn.ExecuteDataTable(sql))
        {
        }

        cnn.OnConnectionInfoMessage -= Cnn_OnConnectionInfoMessage;
      }

      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, stayConnectionOpen: true, timeOut: 200))
      {
        using (DataTable tbl = cnn.ExecuteDataTable(sql))
        {
        }

        cnn.OnConnectionInfoMessage += Cnn_OnConnectionInfoMessage;
        using (DataTable tbl = cnn.ExecuteDataTable(sql))
        {
        }

        cnn.OnConnectionInfoMessage -= Cnn_OnConnectionInfoMessage;
      }

      using (ConnectionParam cnn = new ConnectionParam(myconnectionString, stayConnectionOpen: true, timeOut: 200))
      {
        using (DataTable tbl = cnn.ExecuteDataTable(sql))
        {
        }

        cnn.OnConnectionInfoMessage += Cnn_OnConnectionInfoMessage;
        using (DataTable tbl = cnn.ExecuteDataTable(sql))
        {
        }

        cnn.OnConnectionInfoMessage -= Cnn_OnConnectionInfoMessage;
      }
    }

    /// <summary>
    /// Arrivée d'information de nombre de ligne mise à jour
    /// </summary>
    /// <param name="sender">Qui appelle</param>
    /// <param name="e">Paramètres du message</param>
    private void Cnn_OnConnectionInfoMessageInsert(object sender, SqlMessageEventArgs e)
    {
      Assert.IsNotNull(e, "Insert : Message reçu");
      Assert.AreEqual(e.IsError, false, "Insert : objet pas une erreur");
      Assert.AreEqual(e.Message, null, "Insert : le message est différent");
      Assert.IsNull(e.Error, "Insert : Le Sql error doit être null");
      Assert.AreEqual(e.RowCount, 1, "Insert : Le nombre de ligne doit être de 0");
    }

    /// <summary>
    /// Reception des informations d ela connexion
    /// </summary>
    /// <param name="sender">Qui appelle</param>
    /// <param name="e">Paramètres du message</param>
    private void Cnn_OnConnectionInfoMessage(object sender, SqlMessageEventArgs e)
    {
      Assert.IsNotNull(sender, "Sender is null");
      Assert.IsNotNull(e, "e is null");
      Assert.AreEqual(e.IsError, false, "Insert : objet pas une erreur");
      Assert.AreEqual(e.Message, "coucou", "Mauvais message");
    }
  }
}
