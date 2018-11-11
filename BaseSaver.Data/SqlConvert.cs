using System;
using System.Data;

namespace BaseSaver.Data
{
  /// <summary>
  /// Convertit les données en provenance d'une dataRow
  /// SLew 08/2018 : Créaton et validation du code
  /// </summary>
  public static class SqlConvert
  {
    /// <summary>
    /// Affiche un compteur (normalement entier positif !)
    /// </summary>
    /// <param name="count">Nombre de valeur</param>
    /// <param name="textIfNone">Texte a afficher si le compteur est à 0</param>
    /// <param name="textIfOne">Texte a afficher si le compteur est à 1</param>
    /// <param name="text">Texte a afficher si le compteur est supérieur à 1 (doit contenir un formatteur d'indice 0 pour le nombre)</param>
    /// <returns>le texte a afficher</returns>
    public static string DisplayCount(this int count, string textIfNone, string textIfOne, string text)
    {
      if (count == 0)
      {
        return textIfNone;
      }
      else if (count == 1)
      {
        return textIfOne;
      }
      else
      {
        return string.Format(text, count);
      }
    }

    /// <summary>
    /// Renvoie la valeur de la colonne de la ligne de données dans un type nullable
    /// </summary>
    /// <typeparam name="T">Le type de base</typeparam>
    /// <param name="row">La ligne de donnée</param>
    /// <param name="columnName">La nom de la colonne</param>
    /// <param name="forType">Valeur ne servant à rien d'autre que de donner le type nullable de retour</param>
    /// <returns>L'objet attendu et dans le bon format</returns>
    public static T ToNull<T>(DataRow row, string columnName, T ifnull)
    {
      if (!string.IsNullOrWhiteSpace(columnName) && row != null && row.Table != null && row.Table.Columns != null && row.Table.Columns.Contains(columnName))
      {
        if (row[columnName] != DBNull.Value)
        {
          return SqlConvert.To(row[columnName], ifnull);
        }
      }

      return default(T);
    }

    /// <summary>
    /// Renvoie la donnée de la colonne demandé de la ligne de données
    /// </summary>
    /// <typeparam name="T">Le type de données voulu</typeparam>
    /// <param name="row">la ligne de données</param>
    /// <param name="columnName">Le nom de la colonne</param>
    /// <param name="ifNull">la valeur à renvoyer si o est null</param>
    /// <returns>La donnée</returns>
    public static T To<T>(DataRow row, string columnName, T ifNull)
    {
      if (!string.IsNullOrWhiteSpace(columnName) && row != null && row.Table != null && row.Table.Columns != null && row.Table.Columns.Contains(columnName))
      {
        return SqlConvert.To(row[columnName], ifNull);
      }
      
      return ifNull;
    }

    /// <summary>
    /// Renvoie la donnée formatté depuis la Bdd
    /// </summary>
    /// <typeparam name="T">Le type de données voulu</typeparam>
    /// <param name="o">L'info dans la base</param>
    /// <param name="ifNull">la valeur à renvoyer si o est null</param>
    /// <returns>La donnée</returns>
    public static T To<T>(object o, T ifNull)
    {
      //// TODO : manque les Types du style byte[] et XML

      // l'ordre des tests est fonction des statistiques d'utilisations des types (les plus utilisés en 1er)
      if (ifNull is int || (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(int)))
      {
        return (T)(object)SqlConvert.ToInt(o, (int)(object)ifNull);
      }
      else if (ifNull is string)
      {
        return (T)(object)SqlConvert.ToString(o, (string)(object)ifNull);
      }
      else if (ifNull is bool)
      {
        return (T)(object)SqlConvert.ToBoolean(o, (bool)(object)ifNull);
      }
      else if (ifNull is DateTime)
      {
        return (T)(object)SqlConvert.ToDateTime(o, (DateTime)(object)ifNull);
      }
      else if (ifNull is float)
      {
        return (T)(object)SqlConvert.ToFloat(o, (float)(object)ifNull);
      }
      else if (ifNull is decimal)
      {
        return (T)(object)SqlConvert.ToDecimal(o, (decimal)(object)ifNull);
      }
      else if (ifNull is Guid)
      {
        return (T)(object)SqlConvert.ToGuid(o, (Guid)(object)ifNull);
      }
      else if (ifNull is double)
      {
        return (T)(object)SqlConvert.ToDouble(o, (double)(object)ifNull);
      }
      else if (ifNull is long || (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(long)))
      {
        return (T)(object)SqlConvert.ToLong(o, (long)(object)ifNull);
      }
      else if (ifNull is short || (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(short)))
      {
        return (T)(object)SqlConvert.ToShort(o, (short)(object)ifNull);
      }
      else if (ifNull is byte || (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(byte)))
      {
        return (T)(object)SqlConvert.ToByte(o, (byte)(object)ifNull);
      }
      else if (ifNull is sbyte || (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(sbyte)))
      {
        return (T)(object)ToSByte(o, (sbyte)(object)ifNull);
      }
      else if (ifNull is ushort || (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(ushort)))
      {
        return (T)(object)ToUShort(o, (ushort)(object)ifNull);
      }
      else if (ifNull is uint || (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(uint)))
      {
        return (T)(object)ToUInt(o, (uint)(object)ifNull);
      }
      else if ((typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(ulong)) || ifNull is ulong)
      {
        return (T)(object)ToULong(o, (ulong)(object)ifNull);
      }
      else
      { // par défaut on caste à partir de strings
        throw new ArgumentException("Type non géré");
      }
    }

    #region Private
    /// <summary>
    /// Convertit un objet en booléen
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">boolen par defaut si null</param>
    /// <returns>un booleen</returns>
    private static bool ToBoolean(object o, bool ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (bool.TryParse(s, out bool b))
      {
        return b;
      }
      else if (decimal.TryParse(s, out decimal d))
      {
        return d == 1;
      }
      else
      {
        return ifNull;
      }
    }

    /// <summary>
    /// Convertit un objet en byte
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int8 par défaut si null</param>
    /// <returns>un int 8</returns>
    private static byte ToByte(object o, byte ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(byte))
      {
        return (byte)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (byte.TryParse(s, out byte d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en short
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int8 par défaut si null</param>
    /// <returns>un int 8</returns>
    private static short ToShort(object o, short ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(short))
      {
        return (short)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (short.TryParse(s, out short d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en ushort
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int8 par défaut si null</param>
    /// <returns>un int 8</returns>
    private static ushort ToUShort(object o, ushort ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(ushort))
      {
        return (ushort)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (ushort.TryParse(s, out ushort d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en int 
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int32 par defaut si null</param>
    /// <returns>un int 32</returns>
    private static int ToInt(object o, int ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(int))
      {
        return (int)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (int.TryParse(s, out int d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en uint 
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int32 par defaut si null</param>
    /// <returns>un int 32</returns>
    private static uint ToUInt(object o, uint ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(uint))
      {
        return (uint)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (uint.TryParse(s, out uint d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en sbyte 
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int32 par defaut si null</param>
    /// <returns>un int 32</returns>
    private static sbyte ToSByte(object o, sbyte ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(sbyte))
      {
        return (sbyte)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (sbyte.TryParse(s, out sbyte d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en long 
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int32 par defaut si null</param>
    /// <returns>un int 32</returns>
    private static long ToLong(object o, long ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(long))
      {
        return (long)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (long.TryParse(s, out long d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en ulong 
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">int32 par defaut si null</param>
    /// <returns>un int 32</returns>
    private static ulong ToULong(object o, ulong ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (o.GetType().IsEnum && Enum.GetUnderlyingType(o.GetType()) == typeof(ulong))
      {
        return (ulong)o;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (ulong.TryParse(s, out ulong d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit avec sécutité un objet en double
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">decimal par defaut si null</param>
    /// <returns>un decimal</returns>
    private static double ToDouble(object o, double ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (double.TryParse(s, out double d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit avec sécutité un objet en float
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">decimal par defaut si null</param>
    /// <returns>un decimal</returns>
    private static float ToFloat(object o, float ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (float.TryParse(s, out float d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit avec sécutité un objet en decimal
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">decimal par defaut si null</param>
    /// <returns>un decimal</returns>
    private static decimal ToDecimal(object o, decimal ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      string s = GetCleanStringFromObjectNumber(o);
      if (decimal.TryParse(s, out decimal d))
      {
        return d;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit avec sécutité un objet en Guid
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">decimal par defaut si null</param>
    /// <returns>un decimal</returns>
    private static Guid ToGuid(object o, Guid ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (Guid.TryParse(o.ToString(), out Guid d))
      {
        return d;
      }

      return ifNull;
    }

    /////// <summary>
    /////// Convertit un objet en tableau de byte
    /////// </summary>
    /////// <param name="o">objet a convertir</param>
    /////// <param name="ifNull">tableau de byte par défaut si null</param>
    /////// <returns>un tableau de byte</returns>
    ////private static byte[] ToByteArray(object o, byte[] ifNull)
    ////{
    ////  if (o != null && o != DBNull.Value)
    ////  {
    ////    System.IO.MemoryStream ms = new System.IO.MemoryStream();
    ////    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    ////    b.Serialize(ms, o);
    ////    return ms.ToArray();
    ////  }
    ////  else
    ////  {
    ////    return ifNull;
    ////  }
    ////}

    /// <summary>
    /// Convertit un objet en datetime avec precision
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">datetime par defaut si null</param>
    /// <returns>un datetime avec precision</returns>
    private static DateTime ToDateTime(object o, DateTime ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      if (DateTime.TryParse(Convert.ToString(o), out DateTime dt))
      {
        return dt;
      }

      return ifNull;
    }

    /// <summary>
    /// Convertit un objet en string et le trim
    /// </summary>
    /// <param name="o">objet a convertir</param>
    /// <param name="ifNull">string par defaut si null</param>
    /// <returns>un string</returns>
    private static string ToString(object o, string ifNull)
    {
      if (o == DBNull.Value || o == null)
      {
        return ifNull;
      }

      return Convert.ToString(o);
    }

    /// <summary>
    /// Retourne une chaine de caracteres correctement formatée pour la transformation en nombre
    /// </summary>
    /// <param name="o">un objet c#</param>
    /// <returns>une chaine de caracteres correctement formatée</returns>
    private static string GetCleanStringFromObjectNumber(object o)
    {
      string s = o.ToString()
        .Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
        .Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
      return s;
    }
    #endregion
  }
}
