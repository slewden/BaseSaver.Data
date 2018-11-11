using System;
using System.Data;
using System.Data.SqlClient;

namespace BaseSaver.Data
{
  /// <summary>
  /// Classe paramètre pour les messages de notification en provenance de ConnectionParam
  /// Sert à notifier les message en provenance du serveur : Message d'information, Erreurs, Nombre de lignes impactées
  /// </summary>
  public sealed class SqlMessageEventArgs : EventArgs
  {
    /// <summary>
    /// Empêche la création d'une instance par défaut de la classe<see cref="SqlMessageEventArgs" />.
    /// </summary>
    private SqlMessageEventArgs()
    {
    }

    /// <summary>
    /// Obtient le texte du message
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Obtient le SQL Error (qui n'est pas forcement une erreur) mais le message renvoyé par le moteur SQL server
    /// Les messages dont la propriété 'class' est supérieure à 18 sont des erreurs 
    /// </summary>
    public SqlError Error { get; private set; }

    /// <summary>
    /// Obtient une valeur indiquant si ce message constitue une erreur ou pas
    /// </summary>
    public bool IsError => this.Error != null && this.Error.Class >= 16;

    /// <summary>
    /// Obtient le nombre de lignes affecté de la dernier instruction
    /// </summary>
    public int RowCount { get; private set; }

    /// <summary>
    /// Renvoie un SqlMessageEventArg créé à partir d'un SqlError
    /// </summary>
    /// <param name="e">Le SqlError</param>
    /// <returns>L'objet instancié</returns>
    public static SqlMessageEventArgs From(SqlError e)
        => new SqlMessageEventArgs()
        {
          Message = e.Message,
          Error = e,
        };

    /// <summary>
    /// Renvoie un SqlMessageEventArg créé à partir d'un seul message texte 
    /// </summary>
    /// <param name="message">Le message</param>
    /// <returns>L'objet instancié</returns>
    public static SqlMessageEventArgs From(string message)
        => new SqlMessageEventArgs()
        {
          Message = message,
        };

    /// <summary>
    /// Renvoie un SqlMessageEventArg créé à partir d'un résultat de requête
    /// </summary>
    /// <param name="e">Le résultat de requête</param>
    /// <returns>L'objet instancié</returns>
    public static SqlMessageEventArgs From(StatementCompletedEventArgs e)
        => new SqlMessageEventArgs()
        {
          RowCount = e.RecordCount
        };

    public override string ToString()
    {
      if (string.IsNullOrWhiteSpace(this.Message))
      {
        return SqlConvert.DisplayCount(this.RowCount, "Aucune ligne affectée", "Une seule ligne affectée", "{0} lignes affectées");
      }
      else if (this.Error == null)
      {
        return this.Message;
      }
      else 
      {
        string p = !string.IsNullOrWhiteSpace(this.Error.Procedure) ? $" Procédure : {this.Error.Procedure}" : string.Empty;
        string s = !string.IsNullOrWhiteSpace(this.Error.Server) ? $" Seveur : {this.Error.Server}" : string.Empty;
        string o = !string.IsNullOrWhiteSpace(this.Error.Source) ? $" Source : {this.Error.Source}" : string.Empty;

        string msg = this.Error.Message.Replace((char)160, ' ');
        return $"Message {this.Error.Number}, Niveau {this.Error.Class}, Etat {this.Error.State}, Ligne {this.Error.LineNumber} : {msg}{s}{p}{o}";
      }

    }
  }
}
