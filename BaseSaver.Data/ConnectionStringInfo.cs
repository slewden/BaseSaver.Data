using System;

namespace BaseSaver.Data
{
  /// <summary>
  /// Classe qui analyse ue chaine de connexion pour fournir de l'info dessus
  /// SLew 08/2018 : dernière relecture + ajout des tests unitaire 100%
  /// </summary>
  public class ConnectionStringInfo
  {
    /// <summary>
    /// Nom à afficher par défaut du port
    /// </summary>
    private const string DEFAULTPORT = "[default]";

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="ConnectionStringInfo" />
    /// </summary>
    /// <param name="cnn">La chaine de connexion</param>
    public ConnectionStringInfo(string cnn)
    {
      string[] nfo = cnn.Split(';');
      foreach (string d in nfo)
      {
        if (d.ToLowerInvariant().StartsWith("data source="))
        {
          string srv = d.Substring(12);
          int n = srv.IndexOf(",", StringComparison.InvariantCulture);
          if (n != -1)
          {
            this.Port = srv.Substring(n + 1);
            this.ServeurOnly = srv.Substring(0, n);
          }
          else
          {
            this.ServeurOnly = srv;
            this.Port = DEFAULTPORT;
          }
        }
        else if (d.ToLowerInvariant().StartsWith("initial catalog="))
        {
          this.Base = d.Substring(16);
        }
        else if (d.ToLowerInvariant().StartsWith("integrated security=sspi"))
        {
          this.IntegratedSecurity = true;
          this.User = string.Empty;
          this.Pass = string.Empty;
        }
        else if (d.ToLowerInvariant().StartsWith("user id="))
        {
          this.IntegratedSecurity = false;
          this.User = d.Substring(8);
        }
        else if (d.ToLowerInvariant().StartsWith("password="))
        {
          this.IntegratedSecurity = false;
          this.Pass = d.Substring(9);
        }
      }
    }

    #region Properties
    /// <summary>
    /// Obtient le nom du serveur seul
    /// </summary>
    public string ServeurOnly { get; private set; } = string.Empty;

    /// <summary>
    /// Obtient les infos du serveur et son N° de port
    /// </summary>
    public string Serveur
    {
      get
      {
        if (string.IsNullOrEmpty(this.Port) || this.Port == DEFAULTPORT)
        {
          return this.ServeurOnly;
        }
        else
        {
          return $"{this.ServeurOnly},{this.Port}";
        }
      }
    }

    /// <summary>
    /// Obtient le nom de la base
    /// </summary>
    public string Base { get; private set; } = string.Empty;

    /// <summary>
    /// Obtient une valeur indiquant si le type de sécurité est intégrée ou pas
    /// </summary>
    public bool IntegratedSecurity { get; private set; } = false;

    /// <summary>
    /// Obtient l'utilisateur de la bdd
    /// </summary>
    public string User { get; private set; } = string.Empty;

    /// <summary>
    /// Obtient le mot de passe
    /// </summary>
    public string Pass { get; private set; } = string.Empty;

    /// <summary>
    /// Obtient le port
    /// </summary>
    public string Port { get; private set; } = string.Empty;
    #endregion

    /// <summary>
    /// Pour affichage ( = la chaine sans mot de passe !!
    /// </summary>
    /// <returns>Le texte à afficher</returns>
    public override string ToString()
    {
      return $"Serveur = {this.Serveur}; Base = {this.Base}";
    }

    /// <summary>
    /// Lance SSMS sur la base référencée par l'objet et ouvre un fichier si besoin
    /// </summary>
    /// <param name="isqlPath">Le dossier d'installation des outils</param>
    /// <param name="file">Le fichier  (dossier + nom ) à ouvrir en plus (si non null)</param>
    /// <returns>Le message d'erreur en cas de problème. Chaine vide en cas de succès</returns>
    public string LaunchIsql(string isqlPath, string file = null)
    {
      if (string.IsNullOrWhiteSpace(isqlPath))
      {
        return "Chemin d'accès non configuré";
      }

      bool v9 = isqlPath.ToLowerInvariant().IndexOf("sqlwb.exe", StringComparison.InvariantCulture) >= 0;
      bool v10 = isqlPath.ToLowerInvariant().IndexOf("ssms.exe", StringComparison.InvariantCulture) >= 0;

      System.Text.StringBuilder fil = new System.Text.StringBuilder();
      fil.AppendFormat(this.GetSqlCmdParam());

      if (!string.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file))
      { // Fichier fournit et il existe sur disque
        string vers = (v9 || v10) ? string.Empty : "-f";
        fil.Append($" {vers} \"{file}\"");
      }

      if (v10)
      { // Gagne du temps !!
        fil.Append(" -nosplash ");
      }

      System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo
      {
        FileName = isqlPath,
        Arguments = fil.ToString(),
        UseShellExecute = true
      };
      try
      {
        System.Diagnostics.Process.Start(p);
        return string.Empty;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Renvoie les paramètre de connexion fonction de la Base
    /// Util pour les utilitaires SQL Server (comme BCP, ISQL, SSMS, ...)
    /// </summary>
    /// <returns>La ligne de commande</returns>
    public string GetSqlCmdParam()
    {
      System.Text.StringBuilder fil = new System.Text.StringBuilder();
      fil.Append($" -S \"{this.Serveur}\"");
      fil.Append($" -d \"{this.Base}\"");
      if (this.IntegratedSecurity)
      {
        fil.Append(" -E");
      }
      else
      {
        fil.Append($" -U \"{this.User}\" -P \"{this.Pass}\"");
      }

      return fil.ToString();
    }
  }
}