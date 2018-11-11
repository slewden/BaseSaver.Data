using System;
using System.Data;
using BaseSaver.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseSaverTest
{
  /// <summary>
  /// Classe de test de la classe SqlConvert
  /// </summary>
  [TestClass]
  public class TestSqlConvert : TestUnitaireBase
  {
    #region Enums
    /// <summary>
    /// Enum pour test int
    /// </summary>
    private enum ETestInt : int
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      v1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      v2 = 2
    }

    /// <summary>
    /// Enum pour test Byte
    /// </summary>
    private enum ETestByte : byte
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      b1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      b2 = 2,
    }

    /// <summary>
    /// Enum pour test Sbyte
    /// </summary>
    private enum ETestSByte : sbyte
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      sb1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      sb2 = 2
    }

    /// <summary>
    /// Enum pour test short
    /// </summary>
    private enum ETestShort : short
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      s1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      s2 = 2
    }

    /// <summary>
    /// Enum pour test UShort
    /// </summary>
    private enum ETestUShort : ushort
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      us1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      us2 = 2
    }

    /// <summary>
    /// Enum de test des UInt
    /// </summary>
    private enum ETestUInt : uint
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      ui1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      ui2 = 2
    }

    /// <summary>
    /// Enum de tet des Long
    /// </summary>
    private enum ETestLong : long
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      l1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      l2 = 2,
    }

    /// <summary>
    /// Enum de test des Ulong
    /// </summary>
    private enum ETestULong : ulong
    {
      /// <summary>
      /// Valeur 1
      /// </summary>
      ul1 = 1,

      /// <summary>
      /// Valeur 2
      /// </summary>
      ul2 = 2
    }
    #endregion

    /// <summary>
    /// Teste displayCount
    /// </summary>
    [TestMethod]
    public void TestDisplayCount()
    {
      string v;
      int n;

      n = 0;
      v = SqlConvert.DisplayCount(n, "Aucun", "Un seul", "{0} valeurs");
      AssertAreEqual(v, "Aucun", "DisplaCount 1");

      n = 1;
      v = SqlConvert.DisplayCount(n, "Aucun", "Un seul", "{0} valeurs");
      AssertAreEqual(v, "Un seul", "DisplaCount 2");

      n = 2;
      v = SqlConvert.DisplayCount(n, "Aucun", "Un seul", "{0} valeurs");
      AssertAreEqual(v, "2 valeurs", "DisplaCount 3");

      n = 156;
      v = SqlConvert.DisplayCount(n, "Aucun", "Un seul", "{0} valeurs");
      AssertAreEqual(v, "156 valeurs", "DisplaCount 4");

      n = -1;
      v = SqlConvert.DisplayCount(n, "Aucun", "Un seul", "{0} valeurs");
      AssertAreEqual(v, "-1 valeurs", "DisplaCount 5");

      n = -2;
      v = SqlConvert.DisplayCount(n, "Aucun", "Un seul", "{0} valeurs");
      AssertAreEqual(v, "-2 valeurs", "DisplaCount 6");

      n = 2;
      v = SqlConvert.DisplayCount(n, "Aucun", "Un seul", "Coucou");
      AssertAreEqual(v, "Coucou", "DisplaCount 7");
    }

    /// <summary>
    /// Teste la versin DataRow et les string
    /// </summary>
    [TestMethod]
    public void TestConvertToDataRowVersion()
    {
      DataTable tbl = new DataTable();

      DataRow row = null;
      string v;

      v = SqlConvert.To(row, "toto", "defaut1");
      AssertAreEqual(v, "defaut1", "To dataRow Null 1");

      v = SqlConvert.To(row, null, "defaut2");
      AssertAreEqual(v, "defaut2", "To dataRow Null 2");

      row = tbl.NewRow();
      v = SqlConvert.To(row, "toto", "defaut3");
      AssertAreEqual(v, "defaut3", "To dataRow Null 3");

      tbl.Columns.Add("Nom");
      row = tbl.NewRow();
      v = SqlConvert.To(row, "toto", "defaut4");
      AssertAreEqual(v, "defaut4", "To dataRow Null 4");

      v = SqlConvert.To(row, "Nom", "defaut5");
      AssertAreEqual(v, "defaut5", "To dataRow Null 5");

      row["Nom"] = "Magic";
      v = SqlConvert.To(row, "Nom", "defaut6");
      AssertAreEqual(v, "Magic", "To dataRow 6");
    }

    /// <summary>
    /// Test les int
    /// </summary>
    [TestMethod]
    public void TestConvertToInt()
    {
      object x;
      int defaut = -1;

      // int
      x = 1;
      int i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, 1, "Int 1");

      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Int null 2");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Int null 3");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Int null 4");

      x = ETestInt.v1;
      ETestInt ti = SqlConvert.To(x, ETestInt.v2);
      AssertAreEqual(ti, ETestInt.v1, "Enum int 5");

      // UInt
      uint defaut2 = 127;
      x = 1;
      uint j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, (uint)1, "UInt 1");

      x = null;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "UInt null 2");

      x = "toto";
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "UInt null 3");

      x = DBNull.Value;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "UInt null 4");

      x = ETestUInt.ui1;
      ETestUInt sti = SqlConvert.To(x, ETestUInt.ui2);
      AssertAreEqual(sti, ETestUInt.ui1, "Enum UInt 5");
    }

    /// <summary>
    /// Test les Short
    /// </summary>
    [TestMethod]
    public void TestConvertToShort()
    {
      object x;
      short defaut = 255;

      // short
      x = 1;
      short i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, (short)1, "Short 1");

      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Short null 2");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Short null 3");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Short null 4");

      x = ETestShort.s1;
      ETestShort ti = SqlConvert.To(x, ETestShort.s2);
      AssertAreEqual(ti, ETestShort.s1, "Enum Short 5");

      // UShort
      ushort defaut2 = 127;
      x = 1;
      ushort j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, (ushort)1, "UShort 1");

      x = null;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "UShort null 2");

      x = "toto";
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "UShort null 3");

      x = DBNull.Value;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "UShort null 4");

      x = ETestUShort.us1;
      ETestUShort sti = SqlConvert.To(x, ETestUShort.us2);
      AssertAreEqual(sti, ETestUShort.us1, "Enum UShort 5");
    }

    /// <summary>
    /// Test les Bytes
    /// </summary>
    [TestMethod]
    public void TestConvertToByte()
    {
      object x;
      byte defaut = 255;

      // byte
      x = 1;
      byte i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, 1, "Byte 1");

      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Byte null 2");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Byte null 3");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Byte null 4");

      x = ETestByte.b1;
      ETestByte ti = SqlConvert.To(x, ETestByte.b2);
      AssertAreEqual(ti, ETestByte.b1, "Enum Byte 5");

      // SByte
      sbyte defaut2 = 127;
      x = 1;
      sbyte j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, (sbyte)1, "SByte 1");

      x = null;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "SByte null 2");

      x = "toto";
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "SByte null 3");

      x = DBNull.Value;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "SByte null 4");

      x = ETestSByte.sb1;
      ETestSByte sti = SqlConvert.To(x, ETestSByte.sb2);
      AssertAreEqual(sti, ETestSByte.sb1, "Enum SByte 5");

      ////// byte Array
      ////byte[] defaut3 = new byte[3] { 1, 0, 1 };
      ////x = new byte[2] { 1, 0 };
      ////byte[] k = SqlConvert.To(x, defaut3);
      ////Assert.IsNotNull(k, "Byte Array null");
      ////AssertAreEqual(k.Length, 2, "Taille du byte array");
      ////AssertAreEqual(k[0], 1, "Byte Array 1[0]");
      ////AssertAreEqual(k[1], 0, "Byte Array 1[1]");

      ////x = null;
      ////k = SqlConvert.To(x, defaut3);
      ////AssertAreEqual(k, defaut3, "Byte Array Null 2");

      ////x = "toto";
      ////k = SqlConvert.To(x, defaut3);
      ////AssertAreEqual(k, defaut3, "Byte Array Null 3");

      ////x = DBNull.Value;
      ////k = SqlConvert.To(x, defaut3);
      ////AssertAreEqual(k, defaut3, "Byte Array Null 4");
    }

    /// <summary>
    /// Test les long
    /// </summary>
    [TestMethod]
    public void TestConvertToLong()
    {
      object x;
      long defaut = -1;

      // long
      x = 1;
      long i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, 1, "Long 1");

      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Long null 2");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Long null 3");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Long null 4");

      x = ETestLong.l1;
      ETestLong ti = SqlConvert.To(x, ETestLong.l2);
      AssertAreEqual(ti, ETestLong.l1, "Enum long 5");

      // ULong
      ulong defaut2 = 127;
      x = 1;
      ulong j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, (ulong)1, "ULong 1");

      x = null;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "ULong null 2");

      x = "toto";
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "ULong null 3");

      x = DBNull.Value;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "ULong null 4");

      x = ETestULong.ul1;
      ETestULong sti = SqlConvert.To(x, ETestULong.ul2);
      AssertAreEqual(sti, ETestULong.ul1, "Enum ULong 5");
    }

    /// <summary>
    /// Test les booleen
    /// </summary>
    [TestMethod]
    public void TestConvertToBoolean()
    {
      object x;
      bool defaut = true;
      x = true;
      bool i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, true, "Booléen 1");

      x = false;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, false, "Booléen 2");

      x = 1;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, true, "Booléen 3");

      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Booléen null 4");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Booléen null 5");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Booléen null 6");
    }

    /// <summary>
    /// Test les DateTime
    /// </summary>
    [TestMethod]
    public void TestConvertToDateTime()
    {
      object x;
      DateTime defaut = DateTime.MinValue;
      x = new DateTime(2018, 12, 3, 17, 45, 0);
      DateTime i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, new DateTime(2018, 12, 3, 17, 45, 0), "Date Time 1");

      x = new DateTime(1753, 1, 1);
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, new DateTime(1753, 1, 1), "Date Time 2");
      
      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Date Time null 3");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Date Time null 4");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Date Time null 5");
    }

    /// <summary>
    /// Test les Decimal / double / Float
    /// </summary>
    [TestMethod]
    public void TestConvertToNumeric()
    {
      object x;

      // decimal
      decimal defaut = -1m;
      x = 3.5m;
      decimal i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, 3.5m, "Decimal 1");

      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Decimal null 2");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Decimal null 3");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Decimal null 4");

      // double
      double defaut2 = -1;
      x = 3.5m;
      double j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, 3.5, "Double 1");

      x = null;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "Double null 2");

      x = "toto";
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "Double null 3");

      x = DBNull.Value;
      j = SqlConvert.To(x, defaut2);
      AssertAreEqual(j, defaut2, "Double null 4");

      // Float
      float defaut3 = -1f;
      x = 3.5f;
      float k = SqlConvert.To(x, defaut3);
      AssertAreEqual(k, 3.5f, "Float 1");

      x = null;
      k = SqlConvert.To(x, defaut3);
      AssertAreEqual(k, defaut3, "Float null 2");

      x = "toto";
      k = SqlConvert.To(x, defaut3);
      AssertAreEqual(j, defaut3, "Float null 3");

      x = DBNull.Value;
      k = SqlConvert.To(x, defaut3);
      AssertAreEqual(j, defaut3, "Float null 4");
    }

    /// <summary>
    /// Test les Guid
    /// </summary>
    [TestMethod]
    public void TestConvertToGuid()
    {
      object x;
      Guid defaut = Guid.Empty;
      x = Guid.NewGuid();
      Guid i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, x, "Guid 1");

      x = null;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Guid null 2");

      x = "toto";
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Guid null 3");

      x = DBNull.Value;
      i = SqlConvert.To(x, defaut);
      AssertAreEqual(i, defaut, "Guid null 4");
    }

    /// <summary>
    /// Test autre chose
    /// </summary>
    [TestMethod]
    public void TestConvertToOther()
    {
      object x;
      char defaut = 'x';
      x = 'a';
      try
      {
        char i = SqlConvert.To(x, defaut);
      }
      catch (ArgumentException ex)
      {
        AssertAreEqual(ex.Message, "Type non géré", "Other 1");
      }
    }

    /// <summary>
    /// Les les types nullables
    /// </summary>
    [TestMethod]
    public void TestConvertNullable()
    {
      DataTable tbl = new DataTable();
      DataRow row = null;
      int? xint;
      int? defaut = 4;

      xint = SqlConvert.ToNull<int?>(row, "toto", defaut);
      AssertAreEqual(xint, null, "ToNullale Null 1");

      row = tbl.NewRow();
      xint = SqlConvert.ToNull<int?>(row, "toto", defaut);
      AssertAreEqual(xint, null, "ToNullale Null 2");

      xint = SqlConvert.ToNull<int?>(row, string.Empty, defaut);
      AssertAreEqual(xint, null, "ToNullale Null 3");

      tbl.Columns.Add("toto");
      row = tbl.NewRow();
      xint = SqlConvert.ToNull<int?>(row, "toto", defaut);
      AssertAreEqual(xint, null, "ToNullale vide 4");

      row["toto"] = 3;
      xint = SqlConvert.ToNull<int?>(row, "toto", defaut);
      AssertAreEqual(xint, 3, "ToNullale remplit 5");
    }
  }
}
