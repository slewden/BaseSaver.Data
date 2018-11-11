using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseSaver.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseSaverTest
{
  /// <summary>
  /// Test de l'objet ConnectionParam
  /// </summary>
  [TestClass]
  public class TestSqlMessageEventArgs
  {
    /// <summary>
    /// Teste les différents constructeurs
    /// </summary>
    [TestMethod]
    public void TestConstructor()
    {
      string txt = "Erreur 12";
      SqlMessageEventArgs msg = SqlMessageEventArgs.From(txt);
      Assert.IsNotNull(msg, "TConst 1 : Objet bien créé");
      Assert.AreEqual(msg.IsError, false, "TConst 1 : objet pas une erreur");
      Assert.AreEqual(msg.Message, txt, "TConst 1 : le message est différent");
      Assert.IsNull(msg.Error, "TConst 1 : Le Sql error doit être null");
      Assert.AreEqual(msg.RowCount, 0, "TConst 1 : Le nombre de ligne doit être de 0");

      string res = msg.ToString();
      Assert.AreEqual(res, txt, "TConst1 : ToString");

      int nb = 99;
      StatementCompletedEventArgs st = new StatementCompletedEventArgs(nb);
      msg = SqlMessageEventArgs.From(st);
      Assert.IsNotNull(msg, "TConst 2 : Objet bien créé");
      Assert.AreEqual(msg.IsError, false, "TConst 2 : objet pas une erreur");
      Assert.AreEqual(msg.Message, null, "TConst 2 : le message est différent");
      Assert.IsNull(msg.Error, "TConst 2 : Le Sql error doit être null");
      Assert.AreEqual(msg.RowCount, nb, "TConst 2 : Le nombre de ligne doit être de 0");

      res = msg.ToString();
      Assert.AreEqual(res, "99 lignes affectées", "TConst 2 : ToString");



      string queryString = "EXECUTE NonExistantStoredProcedure";
      using (SqlConnection connection = new SqlConnection(TestConnectionParam.myconnectionString))
      {
        SqlCommand command = new SqlCommand(queryString, connection);
        try
        {
          command.Connection.Open();
          command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
          msg = SqlMessageEventArgs.From(ex.Errors[0]);
          string result = "Message 2812, Niveau 16, Etat 62, Ligne 1 : Procédure stockée 'NonExistantStoredProcedure' introuvable. Seveur : localhost Source : .Net SqlClient Data Provider";
          res = msg.ToString();

          for (int i = 0; i < res.Length; i++)
          {
            Assert.AreEqual(res[i], result[i], i.ToString());
          }
        }
      }
    }
  }
}
