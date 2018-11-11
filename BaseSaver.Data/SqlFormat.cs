using System;
using System.Globalization;

namespace BaseSaver.Data
{
  /// <summary>
  /// Donne le format sql des types string, int, float, datetime
  /// SLew 08/2018 : Créaton et validation du code
  /// </summary>
  public static class SqlFormat
  {
    /// <summary>
    /// date minimum sql
    /// </summary>
    public static readonly DateTime DATEMINSQL = new DateTime(1753, 1, 1);

    /// <summary>
    /// date maximum sql
    /// </summary>
    public static readonly DateTime DATEMAXSQL = new DateTime(9999, 12, 31, 23, 59, 59);

    /// <summary>
    /// Type de formattage de date
    /// Utilisé pour différentier le type de stockage du champ dans SQL Server
    /// </summary>
    public enum EFormatDate
    {
      /// <summary>
      /// Date time
      /// </summary>
      DateTime,

      /// <summary>
      /// Date time 2 Sql Server
      /// </summary>
      DateTime2,

      /// <summary>
      /// Time : Les heures
      /// </summary>
      Time
    }

    /// <summary>
    /// Obtient ou définit la date pivot pour les heure stockées dans un DateTime ou DateTime2 
    /// (peut être modifiée à volontée par l'application au chargement de l'assembly)
    /// </summary>
    public static DateTime DatePivotHeure { get; set; } = new DateTime(1900, 1, 1);

    /// <summary>
    /// Restaure la date pivot pour les heures
    /// </summary>
    public static void RestaureDatePivot()
    {
      DatePivotHeure = new DateTime(1900, 1, 1);
    }

    /// <summary>
    /// Formate un boolean nullable
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string BooleanFormat(bool? val)
    {
      return val == null ? "NULL" : BooleanFormat(val.Value);
    }

    /// <summary>
    /// Formate un boolean
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string BooleanFormat(bool val)
    {
      return val ? "1" : "0";
    }

    /// <summary>
    /// Format datetime yyyy-MM-dd HH:mm:ss
    /// </summary>
    /// <param name="val">La valeur nullable à formatter</param>
    /// <returns>Le texte à mettre dans le SQL</returns>
    public static string DateTimeFormat(DateTime? val)
    {
      if (val == null)
      {
        return DateTimeAllFormat(EFormatDate.DateTime, DateTime.MinValue, true);
      }
      else
      {
        return DateTimeAllFormat(EFormatDate.DateTime, val.Value, true);
      }
    }

    /// <summary>
    /// Format datetime yyyy-MM-dd HH:mm:ss
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le texte à mettre dans le SQL</returns>
    public static string DateTimeFormat(DateTime val, bool nullable = false)
    {
      return DateTimeAllFormat(EFormatDate.DateTime, val, nullable);
    }

    /// <summary>
    /// Format datetime yyyy-MM-dd HH:mm:ss
    /// </summary>
    /// <param name="val">La valeur nullable à formatter</param>
    /// <returns>Le texte à mettre dans le SQL</returns>
    public static string DateTime2Format(DateTime? val)
    {
      if (val == null)
      {
        return DateTimeAllFormat(EFormatDate.DateTime2, DateTime.MinValue, true);
      }
      else
      {
        return DateTimeAllFormat(EFormatDate.DateTime2, val.Value, true);
      }
    }

    /// <summary>
    /// Format datetime yyyy-MM-dd HH:mm:ss
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le texte à mettre dans le SQL</returns>
    public static string DateTime2Format(DateTime val, bool nullable = false)
    {
      return DateTimeAllFormat(EFormatDate.DateTime2, val, nullable);
    }

    /// <summary>
    /// Format une heure nullable 
    /// </summary>
    /// <param name="val">La valeur nullable à formatter</param>
    /// <param name="sqlServerType">Le type de données SQL server pour stoqué l'heure</param>
    /// <returns>Le texte à mettre dans le SQL</returns>
    public static string HeureFormat(DateTime? val, EFormatDate sqlServerType = EFormatDate.DateTime)
    {
      if (val == null)
      {
        return HeureFormat(DateTime.MinValue, sqlServerType, true);
      }
      else
      {
        return HeureFormat(val.Value, sqlServerType, true);
      }
    }

    /// <summary>
    /// Formatte une heur : on utilise ici la date pivot du 01/01/1900 pour indique que c'est une heure 
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="sqlServerType">Le type de données SQL server pour stoqué l'heure</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le texte à mettre dans le SQL</returns>
    public static string HeureFormat(DateTime val, EFormatDate sqlServerType = EFormatDate.DateTime, bool nullable = false)
    {
      if (val == DateTime.MinValue || val == DateTime.MaxValue || val < DATEMINSQL || val > DATEMAXSQL)
      { // Val est considéré comme null
        return DateTimeAllFormat(sqlServerType, DateTime.MinValue, nullable);
      }
      else
      {
        DateTime dt = new DateTime(DatePivotHeure.Year, DatePivotHeure.Month, DatePivotHeure.Day, val.Hour, val.Minute, val.Second, val.Millisecond);
        return DateTimeAllFormat(sqlServerType, dt, nullable);
      }
    }

    /// <summary>
    /// Format un double ###.###
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string DoubleFormat(double? val)
    {
      if (val == null)
      {
        return DoubleFormat(double.MinValue, true);
      }
      else
      {
        return DoubleFormat(val.Value, true);
      }
    }

    /// <summary>
    /// Format un double ###.###
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string DoubleFormat(double val, bool nullable = false)
    {
      string f = null;
      if (val == double.MinValue || val == double.MaxValue)
      {
        f = nullable ? "NULL" : "0";
      }
      else
      {
        f = val.ToString().Replace(",", ".").Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, ".");
      }

      return f;
    }

    /// <summary>
    /// Format un decimal ###.###
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string DecimalFormat(decimal? val)
    {
      if (val == null)
      {
        return DecimalFormat(decimal.MinValue, true);
      }
      else
      {
        return DecimalFormat(val.Value, true);
      }
    }

    /// <summary>
    /// Formatage des décimaux en SQL
    /// </summary>
    /// <param name="val">décimal à traiter</param>
    /// <param name="nullable">peut être nulle</param>
    /// <returns>décimal formaté</returns>
    public static string DecimalFormat(decimal val, bool nullable = false)
    {
      string f = null;
      if (val == decimal.MinValue || val == decimal.MaxValue)
      {
        f = nullable ? "NULL" : "0";
      }
      else
      {
        f = val.ToString().Replace(",", ".").Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, ".");
      }

      return f;
    }

    /// <summary>
    /// Formatage des Float en SQL
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string FloatFormat(float? val)
    {
      if (val == null)
      {
        return FloatFormat(float.MinValue, true);
      }
      else
      {
        return FloatFormat(val.Value, true);
      }
    }

    /// <summary>
    /// Formatage des Float en SQL
    /// </summary>
    /// <param name="val">décimal à traiter</param>
    /// <param name="nullable">peut être nulle</param>
    /// <returns>décimal formaté</returns>
    public static string FloatFormat(float val, bool nullable = false)
    {
      string f = null;
      if (val == float.MinValue || val == float.MaxValue)
      {
        f = nullable ? "NULL" : "0";
      }
      else
      {
        f = val.ToString().Replace(",", ".").Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, ".");
      }

      return f;
    }

    /// <summary>
    /// Formate un entier
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string IntegerFormat(int? val)
    {
      if (val == null)
      {
        return IntegerFormat(int.MinValue, true);
      }
      else
      {
        return IntegerFormat(val.Value, true);
      }
    }

    /// <summary>
    /// Formate un entier
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string IntegerFormat(int val, bool nullable = false)
    {
      string f = null;
      if (val == int.MinValue || val == int.MaxValue)
      {
        f = nullable ? "NULL" : "0";
      }
      else
      {
        f = val.ToString();
      }

      return f;
    }

    /// <summary>
    /// Formate un entier long
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string LongFormat(long? val)
    {
      if (val == null)
      {
        return LongFormat(long.MinValue, true);
      }
      else
      {
        return LongFormat(val.Value, true);
      }
    }

    /// <summary>
    /// Formate un long
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string LongFormat(long val, bool nullable = false)
    {
      string f = null;
      if (val == long.MinValue || val == long.MaxValue)
      {
        f = nullable ? "NULL" : "0";
      }
      else
      {
        f = val.ToString();
      }

      return f;
    }

    /// <summary>
    /// Formate un short
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string ShortFormat(short? val)
    {
      if (val == null)
      {
        return ShortFormat(short.MinValue, true);
      }
      else
      {
        return ShortFormat(val.Value, true);
      }
    }

    /// <summary>
    /// Formate un short
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string ShortFormat(short val, bool nullable = false)
    {
      string f = null;
      if (val == short.MinValue || val == short.MaxValue)
      {
        f = nullable ? "NULL" : "0";
      }
      else
      {
        f = val.ToString();
      }

      return f;
    }

    /// <summary>
    /// Formate un Byte
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string ByteFormat(byte? val)
    {
      if (val == null)
      {
        return ByteFormat(byte.MinValue, true);
      }
      else
      {
        return ByteFormat(val.Value, true);
      }
    }

    /// <summary>
    /// Formate un byte
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string ByteFormat(byte val, bool nullable = false)
    {
      string f = null;
      if (val == byte.MinValue || val == byte.MaxValue)
      {
        f = nullable ? "NULL" : "0";
      }
      else
      {
        f = val.ToString();
      }

      return f;
    }

    /// <summary>
    /// Formatte une chaine pour un like en utilisant un wildCard (%)  avant ou après
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="avant">Indique si le wildcard doit être avant</param>
    /// <param name="apres">Indique si le wildcard doit être après</param>
    /// <param name="withTrim">Indique si on supprime les espace autour de la chaine</param>
    /// <param name="unicode">Indique si on génère une chaine unicode ou pas</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string StringFormatLike(string val, bool avant = false, bool apres = false, bool withTrim = true, bool unicode = false)
    {
      string txtAv = avant ? "%" : string.Empty;
      string txtAp = apres ? "%" : string.Empty;
      string txt = withTrim ? val.Trim() : val;
      return StringFormat($"{txtAv}{txt}{txtAp}", withTrim: withTrim, unicode: unicode);
    }

    /// <summary>
    /// Formatte une chaine et s'assure qu'elle ne dépasse pas maxLength caractères
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="maxLength">Taille maximale de la chaine</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <param name="withTrim">Avec suppression des espaces ou pas</param>
    /// <param name="unicode">Indique si on génère une chaine unicode ou pas</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string StringFormat(string val, int maxLength = 0, bool nullable = false, bool withTrim = false, bool unicode = false)
    {
      if (maxLength > 0 && val != null && val.Length > maxLength)
      {
        val = val.Substring(0, maxLength);
      }

      string f;
      if (string.IsNullOrWhiteSpace(val))
      { // Val est null
        f = nullable ? "NULL" : "''";
      }
      else
      { // Val est non null
        string u = unicode ? "N" : string.Empty;
        string t = val.Replace("'", "''");
        if (withTrim)
        {
          t = t.Trim();
        }

        f = $"{u}'{t}'";
      }

      return f;
    }

    /// <summary>
    /// Formate un guid en SQL
    /// </summary>
    /// <param name="val">guid à traiter</param>
    /// <param name="nullable">peut être nulle</param>
    /// <param name="withCote">doit-on ajouter les cotes sql</param>
    /// <returns>guid formatée</returns>
    public static string GuidFormat(Guid val, bool nullable = false, bool withCote = true)
    {
      string f;
      if (val == Guid.Empty)
      { // null
        f = nullable ? "NULL" : withCote ? $"'{Guid.Empty}'" : Guid.Empty.ToString();
      }
      else
      {
        f = withCote ? $"'{val}'" : val.ToString();
      }

      return f;
    }

    /// <summary>
    /// Formatte une clé étrangère (entier > 0 sinon = NULL)
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string ForeignKeyFormat(int val)
    {
      return val > 0 ? val.ToString() : "NULL";
    }

    /// <summary>
    /// Formatte une clé étrangère (long > 0 sinon = NULL)
    /// </summary>
    /// <param name="val">La valeur à formatter</param>
    /// <returns>Le text à mettre dans le SQL</returns>
    public static string ForeignKeyFormat(long val)
    {
      return val > 0 ? val.ToString() : "NULL";
    }

    /// <summary>
    /// Format datetime yyyy-MM-dd HH:mm:ss
    /// </summary>
    /// <param name="format">Le type de format demandé</param>
    /// <param name="val">La valeur à formatter</param>
    /// <param name="nullable">Peut elle prendre null comme valeur</param>
    /// <returns>Le texte à mettre dans le SQL</returns>
    private static string DateTimeAllFormat(EFormatDate format, DateTime val, bool nullable = false)
    {
      string f = string.Empty;
      if (val == DateTime.MinValue || val == DateTime.MaxValue || (format != EFormatDate.DateTime2 && val < DATEMINSQL) || (format != EFormatDate.DateTime2 && val > DATEMAXSQL))
      { // Val est considéré comme null
        f = nullable ? "NULL" : "GETDATE()";
      }
      else
      { // Val est non null : Date time2 a un format custom VS datetime et heures
        if (format == EFormatDate.DateTime)
        {
          f = $"'{val:s}'";
        }
        else if (format == EFormatDate.DateTime2)
        {
          f = $"'{val:s}.{val:fff}'";
        }
        else if (format == EFormatDate.Time)
        {
          f = $"'{val:HH:mm:ss.fff}'";
        }
      }

      return f;
    }
  }
}
