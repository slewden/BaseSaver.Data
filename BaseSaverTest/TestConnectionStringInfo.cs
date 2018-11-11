using System;
using BaseSaver.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseSaverTest
{
  /// <summary>
  /// Classe pour tester la classe ConnectionStringInfo
  /// </summary>
  [TestClass]
  public class TestConnectionStringInfo : TestUnitaireBase
  {
    /// <summary>
    /// Passer à true pour lancer tous les test (couverture de code maximale
    /// Dans ce cas : SSMS est lancé deux fois à chaque exécution des test !!
    /// </summary>
    private readonly bool activeLaunchISQL = false; 

    /// <summary>
    /// Teste une chaine en sécurité intégré
    /// </summary>
    [TestMethod]
    public void TestDecompositionSSPI()
    {
      string cnn = "Data Source=localhost;Initial Catalog=version_db3;integrated Security=SSPI;Pooling=False";
      ConnectionStringInfo info = new ConnectionStringInfo(cnn);

      AssertAreEqual(info.Base, "version_db3", "Base");
      AssertAreEqual(info.Serveur, "localhost", "Serveur");
      AssertAreEqual(info.Port, "[default]", "Port du serveur");
      AssertAreEqual(info.ServeurOnly, "localhost", "ServeurOnly");
      AssertAreEqual(info.IntegratedSecurity, true, "Sécurité intégrée");
      AssertAreEqual(info.User, string.Empty, "User");
      AssertAreEqual(info.Pass, string.Empty, "Mot de passe");
    }

    /// <summary>
    /// Test une chaine avec un user
    /// </summary>
    [TestMethod]
    public void TestDecompositionUser()
    {
      string cnn = "Data Source=localhost;Initial Catalog=version_db3;User id=sa;Password=polux;Pooling=False";
      ConnectionStringInfo info = new ConnectionStringInfo(cnn);

      AssertAreEqual(info.Base, "version_db3", "Base");
      AssertAreEqual(info.Serveur, "localhost", "Serveur");
      AssertAreEqual(info.Port, "[default]", "Port du serveur");
      AssertAreEqual(info.ServeurOnly, "localhost", "ServeurOnly");
      AssertAreEqual(info.IntegratedSecurity, false, "Sécurité intégrée");
      AssertAreEqual(info.User, "sa", "User");
      AssertAreEqual(info.Pass, "polux", "Mot de passe");
    }

    /// <summary>
    /// Test une chaine avec un port
    /// </summary>
    [TestMethod]
    public void TestDecompositionPort()
    {
      string cnn = "Data Source=theMachine,1434;Initial Catalog=version_db3;User id=sa;Password=polux;Pooling=False";
      ConnectionStringInfo info = new ConnectionStringInfo(cnn);

      AssertAreEqual(info.Base, "version_db3", "Base");
      AssertAreEqual(info.Serveur, "theMachine,1434", "Serveur");
      AssertAreEqual(info.Port, "1434", "Port du serveur");
      AssertAreEqual(info.ServeurOnly, "theMachine", "ServeurOnly");
      AssertAreEqual(info.IntegratedSecurity, false, "Sécurité intégrée");
      AssertAreEqual(info.User, "sa", "User");
      AssertAreEqual(info.Pass, "polux", "Mot de passe");
    }
    
    /// <summary>
    /// Test le Tostring
    /// </summary>
    [TestMethod]
    public void TestDecompositionToString()
    {
      string cnn = "Data Source=theMachine,1434;Initial Catalog=version3;User id=sa;Password=polux;Pooling=False";
      ConnectionStringInfo info = new ConnectionStringInfo(cnn);
      string res = info.ToString();

      AssertAreEqual(res, "Serveur = theMachine,1434; Base = version3", "Base");
    }

    /// <summary>
    /// Test une chaine Partielle
    /// </summary>
    [TestMethod]
    public void TestDecompositionKo()
    {
      string cnn = "Data Source=localhost;Initial Catalog=version";
      ConnectionStringInfo info = new ConnectionStringInfo(cnn);

      AssertAreEqual(info.Base, "version", "Base");
      AssertAreEqual(info.Serveur, "localhost", "Serveur");
      AssertAreEqual(info.Port, "[default]", "Port du serveur");
      AssertAreEqual(info.ServeurOnly, "localhost", "ServeurOnly");
      AssertAreEqual(info.IntegratedSecurity, false, "Sécurité intégrée");
      AssertAreEqual(info.User, string.Empty, "User");
      AssertAreEqual(info.Pass, string.Empty, "Mot de passe");

      cnn = "bonjour; les gens;";
      info = new ConnectionStringInfo(cnn);

      AssertAreEqual(info.Base, string.Empty, "Base");
      AssertAreEqual(info.Serveur, string.Empty, "Serveur");
      AssertAreEqual(info.Port, string.Empty, "Port du serveur");
      AssertAreEqual(info.ServeurOnly, string.Empty, "ServeurOnly");
      AssertAreEqual(info.IntegratedSecurity, false, "Sécurité intégrée");
      AssertAreEqual(info.User, string.Empty, "User");
      AssertAreEqual(info.Pass, string.Empty, "Mot de passe");
    }

    /// <summary>
    /// Test la fonction GetSqlCmdParam()
    /// </summary>
    [TestMethod]
    public void TestGetSqlCmdParam()
    {
      string cnn = "Data Source=theMachine,1434;Initial Catalog=version;User id=sa;Password=polux;Pooling=False";
      ConnectionStringInfo info = new ConnectionStringInfo(cnn);
      string res = info.GetSqlCmdParam();

      AssertAreEqual(res, " -S \"theMachine,1434\" -d \"version\" -U \"sa\" -P \"polux\"", "GetSqlCmdParam");

      cnn = "Data Source=theMachine,1434;Initial Catalog=version;integrated Security=SSPI;Pooling=False";
      info = new ConnectionStringInfo(cnn);
      res = info.GetSqlCmdParam();

      AssertAreEqual(res, " -S \"theMachine,1434\" -d \"version\" -E", "GetSqlCmdParam");
    }

    /// <summary>
    /// Test la méthode LaunchISQL
    /// </summary>
    [TestMethod]
    public void TestLaunchISQL()
    {
      if (this.activeLaunchISQL)
      {
        string cnn = "Data Source=localhost;Initial Catalog=version_db3;integrated Security=SSPI;Pooling=False";
        ConnectionStringInfo info = new ConnectionStringInfo(cnn);

        string res = info.LaunchIsql(string.Empty, null);
        AssertAreEqual(res, "Chemin d'accès non configuré", "GetSqlCmdParam Vide");

        res = info.LaunchIsql("ssms.exe", null);
        AssertAreEqual(res, string.Empty, "GetSqlCmdParam environnement vide");

        res = info.LaunchIsql("ssms.exe", @"C:\Projets\BaseSaverSolution\BaseSaverTest\Demo.sql");
        AssertAreEqual(res, string.Empty, "GetSqlCmdParam avec fichier Demo ouvert");

        res = info.LaunchIsql("sqlwb.exe", @"C:\Projets\BaseSaverSolution\BaseSaverTest\Demo.sql");
        AssertAreEqual(res, "Le fichier spécifié est introuvable", "GetSqlCmdParam avec sqlWb");

        res = info.LaunchIsql("isql.exe", @"C:\Projets\BaseSaverSolution\BaseSaverTest\Demo.sql");
        AssertAreEqual(res, "Le fichier spécifié est introuvable", "GetSqlCmdParam avec sqlWb");
      }
    }
  }
}
