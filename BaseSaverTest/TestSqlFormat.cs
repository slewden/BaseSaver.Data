using System;
using BaseSaver.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseSaverTest
{
  /// <summary>
  /// Teste la classe SQLFormat
  /// </summary>
  [TestClass]
  public class TestSqlFormat : TestUnitaireBase
  {
    /// <summary>
    /// Test BooleanFormat
    /// </summary>
    [TestMethod]
    public void TestBooleanFormat()
    {
      string v = SqlFormat.BooleanFormat(true);
      AssertAreEqual(v, "1", "Test True");

      v = SqlFormat.BooleanFormat(false);
      AssertAreEqual(v, "0", "Test False");

      bool? b = null;
      v = SqlFormat.BooleanFormat(b);
      AssertAreEqual(v, "NULL", "Test boolean Null");

      b = true;
      v = SqlFormat.BooleanFormat(b);
      AssertAreEqual(v, "1", "Test True?");

      b = false;
      v = SqlFormat.BooleanFormat(b);
      AssertAreEqual(v, "0", "Test False?");
    }

    /// <summary>
    /// Test DateTimeFormat
    /// </summary>
    [TestMethod]
    public void TestDateTimeFormat()
    {
      string v = SqlFormat.DateTimeFormat(null);
      AssertAreEqual(v, "NULL", "Date nulle");

      v = SqlFormat.DateTimeFormat(new DateTime(2000, 10, 23, 12, 30, 00, 100));
      AssertAreEqual(v, "'2000-10-23T12:30:00'", "Date 23/10/2000 12h30:00.100");

      v = SqlFormat.DateTimeFormat(new DateTime(2000, 10, 23, 12, 30, 00, 100), true);
      AssertAreEqual(v, "'2000-10-23T12:30:00'", "Date nullable 23/10/2000 12h30:00.100");

      v = SqlFormat.DateTimeFormat(DateTime.MinValue);
      AssertAreEqual(v, "GETDATE()", "Date MinValue not Null");

      v = SqlFormat.DateTimeFormat(DateTime.MinValue, true);
      AssertAreEqual(v, "NULL", "Date MinValue Null");

      v = SqlFormat.DateTimeFormat(DateTime.MaxValue);
      AssertAreEqual(v, "GETDATE()", "Date MaxValue not Null");

      v = SqlFormat.DateTimeFormat(new DateTime(1600, 1, 5), true);
      AssertAreEqual(v, "NULL", "Date 05/01/1600 inférieur à min SQL nullable");

      v = SqlFormat.DateTimeFormat(new DateTime(9999, 12, 31, 23, 59, 59, 300), true);
      AssertAreEqual(v, "NULL", "Date 31/12/9999 23h59:59.300 supérieur à max SQL nullable");

      DateTime? dt = null;
      v = SqlFormat.DateTimeFormat(dt);
      AssertAreEqual(v, "NULL", "Date?  Null");

      dt = new DateTime(2000, 10, 23, 12, 30, 00);
      v = SqlFormat.DateTimeFormat(dt);
      AssertAreEqual(v, "'2000-10-23T12:30:00'", "Date 23/10/2000 12h30");

      // DateTime2
      dt = null;
      v = SqlFormat.DateTime2Format(dt);
      AssertAreEqual(v, "NULL", "DateTime2?  Null");

      dt = new DateTime(2000, 10, 23, 12, 30, 00, 100);
      v = SqlFormat.DateTime2Format(dt);
      AssertAreEqual(v, "'2000-10-23T12:30:00.100'", "Date Time2 23/10/2000 12h30:00.100");

      v = SqlFormat.DateTime2Format(new DateTime(2000, 10, 23, 12, 30, 00, 100), true);
      AssertAreEqual(v, "'2000-10-23T12:30:00.100'", "Date nullable Time2 23/10/2000 12h30:00.100");

      // Heure Format
      DateTime pivot = SqlFormat.DatePivotHeure;
      AssertAreEqual(pivot, new DateTime(1900, 1, 1), "Heure Pivot base");

      dt = null;
      v = SqlFormat.HeureFormat(dt);
      AssertAreEqual(v, "NULL", "Heure? dans datetime  Null");

      dt = new DateTime(2000, 10, 23, 12, 30, 00, 100);
      v = SqlFormat.HeureFormat(dt);
      AssertAreEqual(v, "'1900-01-01T12:30:00'", "Heure? dans datetime 23/10/2000 12h30:00.100");

      v = SqlFormat.HeureFormat(new DateTime(2000, 10, 23, 12, 30, 00, 100), nullable: true);
      AssertAreEqual(v, "'1900-01-01T12:30:00'", "Heure? dans datetime nullable 23/10/2000 12h30:00.100");

      // Heure dans DateTime2
      dt = null;
      v = SqlFormat.HeureFormat(dt, SqlFormat.EFormatDate.DateTime2);
      AssertAreEqual(v, "NULL", "Heure? dans datetime2 Null");

      dt = new DateTime(2000, 10, 23, 12, 30, 00, 100);
      v = SqlFormat.HeureFormat(dt, SqlFormat.EFormatDate.DateTime2);
      AssertAreEqual(v, "'1900-01-01T12:30:00.100'", "Heure? dans datetime2 23/10/2000 12h30:00.100");

      v = SqlFormat.HeureFormat(new DateTime(2000, 10, 23, 12, 30, 00, 100), SqlFormat.EFormatDate.DateTime2, true);
      AssertAreEqual(v, "'1900-01-01T12:30:00.100'", "Heure? dans datetime2 nullable 23/10/2000 12h30:00.100");

      // Heure dans time 
      dt = null;
      v = SqlFormat.HeureFormat(dt, SqlFormat.EFormatDate.Time);
      AssertAreEqual(v, "NULL", "Heure? dans time Null");

      dt = new DateTime(2000, 10, 23, 12, 30, 00, 100);
      v = SqlFormat.HeureFormat(dt, SqlFormat.EFormatDate.Time);
      AssertAreEqual(v, "'12:30:00.100'", "Heure? dans time 12h30:00.100");

      v = SqlFormat.HeureFormat(new DateTime(2000, 10, 23, 12, 30, 00, 100), SqlFormat.EFormatDate.Time, true);
      AssertAreEqual(v, "'12:30:00.100'", "Heure? dans time nullable 23/10/2000 12h30:00.100");

      // mise à jour de la date pivot
      SqlFormat.DatePivotHeure = new DateTime(1753, 1, 1);
      pivot = SqlFormat.DatePivotHeure;
      AssertAreEqual(pivot, new DateTime(1753, 1, 1), "Date pivot non modifiée");

      dt = new DateTime(2000, 10, 23, 12, 30, 00, 100);
      v = SqlFormat.HeureFormat(dt);
      AssertAreEqual(v, "'1753-01-01T12:30:00'", "Heure? dans datetime 23/10/2000 12h30:00.100");

      SqlFormat.RestaureDatePivot();
      pivot = SqlFormat.DatePivotHeure;
      AssertAreEqual(pivot, new DateTime(1900, 1, 1), "Date pivot non restaurée");
    }

    /// <summary>
    /// Test double format
    /// </summary>
    [TestMethod]
    public void TestDoubleFormat()
    {
      double? d;
      string v;

      d = null;
      v = SqlFormat.DoubleFormat(d);
      AssertAreEqual(v, "NULL", "Double? null");

      d = 6.4;
      v = SqlFormat.DoubleFormat(d);
      AssertAreEqual(v, "6.4", "Double 6,4");

      v = SqlFormat.DoubleFormat(double.MinValue, false);
      AssertAreEqual(v, "0", "Double min non null");

      v = SqlFormat.DoubleFormat(double.MaxValue, false);
      AssertAreEqual(v, "0", "Double min non null");

      v = SqlFormat.DoubleFormat(double.MinValue, true);
      AssertAreEqual(v, "NULL", "Double min null");
    }

    /// <summary>
    /// Teste decimalFormat
    /// </summary>
    [TestMethod]
    public void TestDecimalFormat()
    {
      decimal? d;
      string v;

      d = null;
      v = SqlFormat.DecimalFormat(d);
      AssertAreEqual(v, "NULL", "Décimal? null");

      d = 4.5m;
      v = SqlFormat.DecimalFormat(d);
      AssertAreEqual(v, "4.5", "Décimal 4,5");

      v = SqlFormat.DecimalFormat(decimal.MinValue, false);
      AssertAreEqual(v, "0", "Décimal min non null");

      v = SqlFormat.DecimalFormat(decimal.MaxValue, false);
      AssertAreEqual(v, "0", "Décimal min non null");

      v = SqlFormat.DecimalFormat(decimal.MinValue, true);
      AssertAreEqual(v, "NULL", "Décimal min null");
    }

    /// <summary>
    /// Teste FloatFormat
    /// </summary>
    [TestMethod]
    public void TestFloatFormat()
    {
      float? d;
      string v;

      d = null;
      v = SqlFormat.FloatFormat(d);
      AssertAreEqual(v, "NULL", "Float? null");

      d = 9.1f;
      v = SqlFormat.FloatFormat(d);
      AssertAreEqual(v, "9.1", "Float 9,1");

      v = SqlFormat.FloatFormat(float.MinValue, false);
      AssertAreEqual(v, "0", "FLoat min non null");

      v = SqlFormat.FloatFormat(float.MaxValue, false);
      AssertAreEqual(v, "0", "Float min non null");

      v = SqlFormat.FloatFormat(float.MinValue, true);
      AssertAreEqual(v, "NULL", "float min null");
    }

    /// <summary>
    /// Teste IntegerFormat
    /// </summary>
    [TestMethod]
    public void TestIntegerFormat()
    {
      int? d;
      string v;

      d = null;
      v = SqlFormat.IntegerFormat(d);
      AssertAreEqual(v, "NULL", "Integer? null");

      d = 1456;
      v = SqlFormat.IntegerFormat(d);
      AssertAreEqual(v, "1456", "Int 1456");

      v = SqlFormat.IntegerFormat(int.MinValue, false);
      AssertAreEqual(v, "0", "Int min non null");

      v = SqlFormat.IntegerFormat(int.MaxValue, false);
      AssertAreEqual(v, "0", "Int min non null");

      v = SqlFormat.IntegerFormat(int.MinValue, true);
      AssertAreEqual(v, "NULL", "int min null");
    }

    /// <summary>
    /// Teste LongFormat
    /// </summary>
    [TestMethod]
    public void TestLongFormat()
    {
      long? d;
      string v;

      d = null;
      v = SqlFormat.LongFormat(d);
      AssertAreEqual(v, "NULL", "Long? null");

      d = 1456;
      v = SqlFormat.LongFormat(d);
      AssertAreEqual(v, "1456", "Long 1456");

      v = SqlFormat.LongFormat(long.MinValue, false);
      AssertAreEqual(v, "0", "Long min non null");

      v = SqlFormat.LongFormat(long.MaxValue, false);
      AssertAreEqual(v, "0", "Long min non null");

      v = SqlFormat.LongFormat(long.MinValue, true);
      AssertAreEqual(v, "NULL", "long min null");
    }

    /// <summary>
    /// Teste ShortFormat
    /// </summary>
    [TestMethod]
    public void TestShortFormat()
    {
      short? d;
      string v;

      d = null;
      v = SqlFormat.ShortFormat(d);
      AssertAreEqual(v, "NULL", "Short? null");

      d = 1456;
      v = SqlFormat.ShortFormat(d);
      AssertAreEqual(v, "1456", "short 1456");

      v = SqlFormat.ShortFormat(short.MinValue, false);
      AssertAreEqual(v, "0", "Short min non null");

      v = SqlFormat.ShortFormat(short.MaxValue, false);
      AssertAreEqual(v, "0", "Short min non null");

      v = SqlFormat.ShortFormat(short.MinValue, true);
      AssertAreEqual(v, "NULL", "Short min null");
    }

    /// <summary>
    /// Teste ByteFormat
    /// </summary>
    [TestMethod]
    public void TestByteFormat()
    {
      byte? d;
      string v;

      d = null;
      v = SqlFormat.ByteFormat(d);
      AssertAreEqual(v, "NULL", "Byte? null");

      d = 145;
      v = SqlFormat.ByteFormat(d);
      AssertAreEqual(v, "145", "Byte 145");

      v = SqlFormat.ByteFormat(byte.MinValue, false);
      AssertAreEqual(v, "0", "Byte min non null");

      v = SqlFormat.ByteFormat(byte.MaxValue, false);
      AssertAreEqual(v, "0", "Byte min non null");

      v = SqlFormat.ByteFormat(byte.MinValue, true);
      AssertAreEqual(v, "NULL", "Byte min null");
    }

    /// <summary>
    /// Teste les stringFormat
    /// </summary>
    [TestMethod]
    public void TestStringFormat()
    {
      string v;
      v = SqlFormat.StringFormatLike("Coucou  ", withTrim: false);
      AssertAreEqual(v, "'Coucou  '", "Chaine Like1");

      v = SqlFormat.StringFormatLike("Coucou  ", withTrim: false, avant: true);
      AssertAreEqual(v, "'%Coucou  '", "Chaine Like2");

      v = SqlFormat.StringFormatLike("Coucou  ", withTrim: false, apres: true);
      AssertAreEqual(v, "'Coucou  %'", "Chaine Like3");

      v = SqlFormat.StringFormatLike("Coucou  ", withTrim: false, avant: true, apres: true);
      AssertAreEqual(v, "'%Coucou  %'", "Chaine Like4");

      v = SqlFormat.StringFormatLike("Coucou  ", avant: false, apres: true, withTrim: true);
      AssertAreEqual(v, "'Coucou%'", "Chaine Like5");

      v = SqlFormat.StringFormatLike("Coucou  ", withTrim: true, unicode: true);
      AssertAreEqual(v, "N'Coucou'", "Chaine Like6");

      v = SqlFormat.StringFormat("Coucou les petits  ", maxLength: 6);
      AssertAreEqual(v, "'Coucou'", "Chaine 7");

      v = SqlFormat.StringFormat(" ", nullable: true);
      AssertAreEqual(v, "NULL", "Chaine 8.1");

      v = SqlFormat.StringFormat(" ", nullable: false);
      AssertAreEqual(v, "''", "Chaine 8.2");

      v = SqlFormat.StringFormat(string.Empty, nullable: true);
      AssertAreEqual(v, "NULL", "Chaine 9");

      v = SqlFormat.StringFormat(null, nullable: true);
      AssertAreEqual(v, "NULL", "Chaine 10");

      v = SqlFormat.StringFormat(null, 255, nullable: true);
      AssertAreEqual(v, "NULL", "Chaine 10");

      v = SqlFormat.StringFormat("Rock n'roll", unicode: true);
      AssertAreEqual(v, "N'Rock n''roll'", "Chaine 11");
    }

    /// <summary>
    /// Teste GuidFormat
    /// </summary>
    [TestMethod]
    public void TestGuidFormat()
    {
      string vide = Guid.Empty.ToString();
      string v;
      v = SqlFormat.GuidFormat(Guid.Empty, withCote: false);
      AssertAreEqual(v, vide, "Guid Null 1");

      v = SqlFormat.GuidFormat(Guid.Empty, withCote: true);
      AssertAreEqual(v, $"'{vide}'", "Guid Null 2");

      v = SqlFormat.GuidFormat(Guid.Empty, nullable: true);
      AssertAreEqual(v, "NULL", "Guid Null 3");

      v = SqlFormat.GuidFormat(Guid.Empty, nullable: false, withCote: true);
      AssertAreEqual(v, $"'{vide}'", "Guid Null 4");

      Guid g = Guid.NewGuid();
      v = SqlFormat.GuidFormat(g, withCote: false);
      AssertAreEqual(v, g.ToString(), "GUID 5");

      v = SqlFormat.GuidFormat(g, withCote: true);
      AssertAreEqual(v, $"'{g}'", "GUID 6");
    }

    /// <summary>
    /// Teste SqlFormat.ForeignKeyFormat
    /// </summary>
    [TestMethod]
    public void TestForeignKey()
    {
      string v;
      int d;

      d = -1;
      v = SqlFormat.ForeignKeyFormat(d);
      AssertAreEqual(v, "NULL", "ForeignKey int 1");

      d = 0;
      v = SqlFormat.ForeignKeyFormat(d);
      AssertAreEqual(v, "NULL", "ForeignKey int 2");

      d = 56;
      v = SqlFormat.ForeignKeyFormat(d);
      AssertAreEqual(v, "56", "ForeignKey int 3");

      long l;

      l = -1;
      v = SqlFormat.ForeignKeyFormat(l);
      AssertAreEqual(v, "NULL", "ForeignKey long 4");

      l = 0;
      v = SqlFormat.ForeignKeyFormat(l);
      AssertAreEqual(v, "NULL", "ForeignKey int 5");

      l = 56;
      v = SqlFormat.ForeignKeyFormat(l);
      AssertAreEqual(v, "56", "ForeignKey int 6");
    }
  }
}
