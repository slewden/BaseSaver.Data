using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseSaverTest
{
  /// <summary>
  /// Classe de base pour les test unitaire apporte les méthodes utiles à toutes les classes
  /// </summary>
  public abstract class TestUnitaireBase
  {
    /// <summary>
    /// Test si les 2 valeur son égale et renvoie un message formatté
    /// </summary>
    /// <typeparam name="T">Le type des données a tester</typeparam>
    /// <param name="v">la valeur calculée</param>
    /// <param name="r">La valeur référence</param>
    /// <param name="msg">Le message d'explication de ce qui est testé</param>
    protected static void AssertAreEqual<T>(T v, T r, string msg)
    {
      Assert.AreEqual(v, r, $"{msg} non correspondant : Trouvé {v} au lieu de {r}");
    }
  }
}
