using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BaseSaver.Data
{
  /// <summary>
  /// G�re la connexion � la base de donn�es
  /// SLew 08/2018 : Cr�aton et validation du code + ajout des tests unitaire +90%
  /// </summary>
  public class ConnectionParam : IDisposable
  {
    #region Membres priv�s
     /// <summary>
    /// y a t il un event a plugger
    /// </summary>
    private bool myconnectionEventExists;
    #endregion

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="ConnectionParam"/>
    /// </summary>
    /// <param name="connexion">La chaine de connexion</param>
    /// <param name="stayConnectionOpen">Indique si l'objet maintient les connexion ouvertes entre les solicitations</param>
    /// <param name="timeOut">Le time out</param>
    public ConnectionParam(string connexion, bool stayConnectionOpen = false, int timeOut = 0)
    {
      this.Transaction = null;
      this.ConnexionString = connexion;
      this.StayConnectionOpen = stayConnectionOpen;
      this.TimeOut = timeOut;
    }

    #region Event and Properties
    /// <summary>
    /// Event sur la connexion sql
    /// </summary>
    public event EventHandler<SqlMessageEventArgs> OnConnectionInfoMessage
    {
      add
      {
        if (!this.myconnectionEventExists)
        {
          this.myconnectionEventExists = true;
          this.MyonConnectionInfoMessage = value;
          if (this.Connection != null && this.Connection.State != ConnectionState.Closed && this.Connection.State != ConnectionState.Broken)
          {
            this.Connection.InfoMessage += this.Connection_InfoMessage;
          }
        }
      }

      remove
      {
        if (this.myconnectionEventExists)
        {
          if (this.Connection != null && this.Connection.State != ConnectionState.Closed && this.Connection.State != ConnectionState.Broken)
          {
            this.Connection.InfoMessage -= this.Connection_InfoMessage;
          }

          this.myconnectionEventExists = false;
          this.MyonConnectionInfoMessage = null;
        }
      }
    }

    /// <summary>
    /// L'�vent d'infos sur la connexion
    /// </summary>
    private event EventHandler<SqlMessageEventArgs> MyonConnectionInfoMessage;

    /// <summary>
    /// Obtient ou d�finit une valeur indiquant si il faut maintenir la connexion ouverte
    /// </summary>
    public bool StayConnectionOpen { get; set; }

    /// <summary>
    /// Obtient la chaine de connexion � la base de donn�es
    /// </summary>
    public string ConnexionString { get; private set; }

    /// <summary>
    /// Obtient ou d�finit le d�lai maximal d'ex�cutions des requ�tes
    /// </summary>
    public int TimeOut { get; set; }

    /// <summary>
    /// Obtient la connexion utilis�e par l'objet (peut �tre null tant qu'on a pas fait une requ�te)
    /// </summary>
    public SqlConnection Connection { get; private set; }

    /// <summary>
    /// Obtient la transaction qui a �t� d�marr�e (null si aucune transaction d�marr�e)
    /// </summary>
    public SqlTransaction Transaction { get; private set; }

    /// <summary>
    /// Obtient le nombre d'imbrication des transactions
    /// (Le commit n'agit que si on est � niveau �gal � 1)
    /// </summary>
    public int TransactionCount { get; private set; } = 0;

    /// <summary>
    /// Renvoie la connectionStringInfo associ�e � l'objet
    /// </summary>
    /// <returns>la chaine</returns>
    public ConnectionStringInfo Infos() => new ConnectionStringInfo(this.ConnexionString);
    #endregion

    /// <summary>
    /// Ex�cute une requete qui ne renvoie pa de r�sultat
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>Le code de retour</returns>
    public int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      int returnValue = 0;
      using (SqlCommand cmd = this.PrepareCommand(commandText, commandType, parameters, timeOut))
      {
        returnValue = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
      }

      this.EndConnectionIfNecessary();
      return returnValue;
    }

    /// <summary>
    /// Ex�cute une requete qui ne renvoie pas de r�sultat
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>La t�che avec le code de retour</returns>
    public async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      int returnValue = 0;
      using (SqlCommand cmd = this.PrepareCommand(commandText, commandType, parameters, timeOut))
      {
        returnValue = await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();
      }

      this.EndConnectionIfNecessary();
      return returnValue;
    }

    /// <summary>
    /// Ex�cute une requ�te et renvoie un seul objet
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>L'objet cherch�</returns>
    public object ExecuteScalar(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      object returnValue = null;
      using (SqlCommand cmd = this.PrepareCommand(commandText, commandType, parameters, timeOut))
      {
        returnValue = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
      }

      this.EndConnectionIfNecessary();
      return returnValue;
    }

    /// <summary>
    /// Ex�cute une requ�te et renvoie un seul objet
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>L'objet cherch�</returns>
    public async Task<object> ExecuteScalarAsync(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      object returnValue = null;
      using (SqlCommand cmd = this.PrepareCommand(commandText, commandType, parameters, timeOut))
      {
        returnValue = await cmd.ExecuteScalarAsync();
        cmd.Parameters.Clear();
      }

      this.EndConnectionIfNecessary();
      return returnValue;
    }

    /// <summary>
    /// Ex�cute une requ�te et renvoie un entier
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="defaultValue">Valeur par d�faut appliqu�e si la requ�te renvoie un truc null</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>Un entier ou la valeur par d�faut</returns>
    public int ExecuteGetId(string commandText, int defaultValue, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      return ConnectionParam.ConvertToInt(this.ExecuteScalar(commandText, commandType, parameters, timeOut), defaultValue);
    }

    /// <summary>
    /// Execute un insert et renvoie la valeur de l'autoincr�ment� de la table
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>La valeur de l'autoincr�ment 0 si Probl�me</returns>
    public int ExecuteInsert(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      commandText += "\n SELECT SCOPE_IDENTITY(); \n";
      return this.ExecuteGetId(commandText, 0, commandType, parameters, timeOut);
    }

    /// <summary>
    /// Execute et renvoie un Dataset
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>Le dataset remplit</returns>
    public DataSet ExecuteDataSet(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      DataSet returnValue = new DataSet();
      using (SqlCommand cmd = this.PrepareCommand(commandText, commandType, parameters, timeOut))
      {
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
          da.Fill(returnValue);
        }

        cmd.Parameters.Clear();
      }

      this.EndConnectionIfNecessary();
      return returnValue;
    }

    /// <summary>
    /// Execute et renvoie un Dataset
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>Le dataset remplit</returns>
    public async Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      DataSet returnValue = new DataSet();
      using (SqlCommand cmd = this.PrepareCommand(commandText, commandType, parameters, timeOut))
      {
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
          await Task.Run(() => da.Fill(returnValue));
        }

        cmd.Parameters.Clear();
      }

      this.EndConnectionIfNecessary();
      return returnValue;
    }

    /// <summary>
    /// Execute et renvoie un DataTable
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>Le dataTable remplit</returns>
    public DataTable ExecuteDataTable(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      using (DataSet dst = this.ExecuteDataSet(commandText, commandType, parameters, timeOut))
      {
        if (dst.Tables.Count > 0)
        {
          return dst.Tables[0];
        }
      }

      return null;
    }

    /// <summary>
    /// Renvoie le dataset, mais en renommant les tables avec les infos fournies dans la premi�re table !
    /// La premi�re table (table[0]) doit contenir les colonnes index et name : 
    ///     Index de la table dans le Dataset
    ///     Name nom � attribuer � l'objet DataTable du DataSet
    /// Le nombre de lignes de la premi�re table doit correspondre au nombre de de tables renvoy�es (Table 0 incluse !)
    /// Ex : Pour renvoyer 3 tables document�es (Table1, Table 2, table 3), 
    ///      il faut ins�rer une table "0" qui contiendra 4 lignes : 0, 'Documentation' UNION 1, 'Table1' UNION 2, 'Table 2' UNION 3, 'table 3'
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>Le dataset</returns>
    public DataSet ExecuteDataSetWithTableName(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      DataSet dst = this.ExecuteDataSet(commandText, commandType, parameters, timeOut);
      if (dst != null && dst.Tables.Count > 1 && dst.Tables[0].Rows.Count >= dst.Tables.Count)
      { // les conditions sont remplies : la premi�re table d�crit les autres tables
        if (dst.Tables[0].Columns.Contains("Index") && dst.Tables[0].Columns.Contains("Name"))
        { // la premi�re table a les bonnes infos
          int index;
          string name;
          foreach (DataRow r in dst.Tables[0].Rows)
          {
            index = ConnectionParam.ConvertToInt(r["Index"], 0);
            if (index >= 0 && index < dst.Tables.Count)
            {
              name = ConnectionParam.ConvertToString(r["Name"], string.Empty);
              if (!string.IsNullOrWhiteSpace(name))
              {
                dst.Tables[index].TableName = name;
              }
            }
          }
        }
      }

      return dst;
    }

    /// <summary>
    /// Renvoie le XElement issu de la requ�te fournie
    /// </summary>
    /// <param name="commandText">le sql ou le nom de la proc�dure stock�e</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les param�tres de la requ�te</param>
    /// <param name="timeOut">Le temps d'ex�cution allou� uniquement pour cette requete (en ms)</param>
    /// <returns>Le XElement</returns>
    public XElement ExecuteXml(string commandText, CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null, int? timeOut = null)
    {
      StringBuilder res = new StringBuilder();
      using (SqlCommand cmd = this.PrepareCommand(commandText, commandType, parameters, timeOut))
      {
        using (XmlReader xmlr = cmd.ExecuteXmlReader())
        {
          xmlr.Read();
          while (xmlr.ReadState != System.Xml.ReadState.EndOfFile)
          {
            res.Append(xmlr.ReadOuterXml());
          }
        }
      }

      if (res.Length > 0)
      {
        return XElement.Parse(res.ToString());
      }
      else
      {
        return null;
      }
    }

    #region Transaction
    /// <summary>
    /// D�marrer une transaction
    /// </summary>
    public void BeginTransaction()
    {
      if (this.TransactionCount == 0)
      {
        this.OpenConnection();

        this.Transaction = this.Connection.BeginTransaction();
      }

      this.TransactionCount++;
    }

    /// <summary>
    /// Valide une transaction
    /// </summary>
    /// <returns>True si le commit est fait (quand le nombre de transaction passe � 0)</returns>
    public bool CommitTransaction()
    {
      bool done = false;
      if (this.TransactionCount == 1 && this.Transaction != null)
      {
        this.Transaction.Commit();
        this.Transaction.Dispose();
        this.Transaction = null;
        done = true;
      }

      if (this.TransactionCount > 0)
      {
        this.TransactionCount--;
      }

      this.EndConnectionIfNecessary();
      return done;
    }

    /// <summary>
    /// Annule toutes les transaction en cours
    /// </summary>
    /// <returns>True si c'est fait</returns>
    public bool RollBackTransaction()
    {
      bool done = false;
      if (this.Transaction != null)
      {
        this.Transaction.Rollback();
        this.Transaction.Dispose();
        this.Transaction = null;
        this.TransactionCount = 0;
        done = true;
      }

      this.EndConnectionIfNecessary();
      return done;
    }
    #endregion

    /// <summary>
    /// Nettoye l'objet
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Nettoye l'objet
    /// </summary>
    /// <param name="disposing">Dispose les ressources interne ou externe</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.Transaction != null)
        {
          this.Transaction.Rollback();
          this.Transaction.Dispose();
          this.Transaction = null;
        }

        if (this.Connection != null)
        {
          this.Connection.Dispose();
          this.Connection = null;
        }
      }
    }

    #region M�thodes priv�es
    /// <summary>
    /// Convertit l'�l�m�nt fourni en int : si c'est pas possible renvoie la defaultValue
    /// </summary>
    /// <param name="element">El�ment a convertir</param>
    /// <param name="defaultValue">Valeur par d�faut</param>
    /// <returns>L'entier convertit</returns>
    private static int ConvertToInt(object element, int defaultValue)
    {
      if (element == DBNull.Value) 
      {
        return defaultValue;
      }

      if (int.TryParse(element.ToString(), out int number))
      {
        return number;
      }

      return defaultValue;
    }

    /// <summary>
    /// Convertit l'�l�ment en chaine; Si pas possible renvoie la valeur par d�faut
    /// </summary>
    /// <param name="element">El�ment � convertir</param>
    /// <param name="defaultValue">La valeur par d�faut</param>
    /// <returns>La chaine convertit</returns>
    private static string ConvertToString(object element, string defaultValue)
    {
      if (element == DBNull.Value)
      {
        return defaultValue;
      }

      return Convert.ToString(element);
    }

    /// <summary>
    /// Pr�pare et renvoie le SqlCommand qui va bien en fonction du contexte
    /// </summary>
    /// <param name="commandText">La requ�te</param>
    /// <param name="commandType">Le type de commande</param>
    /// <param name="parameters">Les �ventuels param�tres</param>
    /// <param name="timeOut">le timeout sp�cifique � cette valeur</param>
    /// <returns>Le SqlCommand remplit pr�t � l'emploi</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:V�rifier si les requ�tes SQL pr�sentent des failles de s�curit�", Justification = "SLew : Y faut bien passer du texte � un moment donn� !!")]
    private SqlCommand PrepareCommand(string commandText, CommandType commandType, IEnumerable<SqlParameter> parameters, int? timeOut)
    {
      SqlCommand cmd = new SqlCommand();

      if (this.Transaction != null && this.Transaction.Connection != null)
      { // la transaction est la et elle est utilisable
        cmd.Connection = this.Transaction.Connection;
        cmd.Transaction = this.Transaction;
      }
      else if (this.Connection != null)
      { // la connexion est d�j� op�rationnelle
        if (this.Connection.State == ConnectionState.Closed || this.Connection.State == ConnectionState.Broken)
        { // faut la r�ouvrir !!
          this.Connection.Open();
        }

        cmd.Connection = this.Connection;
      }
      else
      {
        this.OpenConnection();
        cmd.Connection = this.Connection;
      }

      cmd.CommandType = commandType;
      cmd.CommandText = commandText;
      cmd.CommandTimeout = timeOut ?? this.TimeOut;
      if (this.myconnectionEventExists)
      {
        cmd.StatementCompleted += this.Command_StatementCompleted;
      }

      if (parameters != null)
      { // Y a des param�tres
        foreach (SqlParameter parameter in parameters)
        { // pour chacun d'entre eux
          if (parameter != null)
          { // non null
            if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
            { // s'il est vide en direction input ==> on met DBNull.Value
              parameter.Value = DBNull.Value;
            }

            cmd.Parameters.Add(parameter);
          }
        }
      }

      return cmd;
    }

    /// <summary>
    /// Nettoie la connection si elle peut et doit l'�tre
    /// </summary>
    private void EndConnectionIfNecessary()
    {
      if (this.TransactionCount == 0 && this.Connection != null && !this.StayConnectionOpen)
      { // La connexion est ouverte, on doit la fermer � chaque fois et y a pas de transaction en cours
        if (this.myconnectionEventExists)
        { // d�tacher l'event sur la connexion
          this.Connection.InfoMessage -= this.Connection_InfoMessage;
        }

        this.Connection.Close();
        this.Connection.Dispose();
        this.Connection = null;
      }
    }

    /// <summary>
    /// Ouvre proprement la connexion
    /// </summary>
    private void OpenConnection()
    {
      if (this.Connection == null)
      {
        this.Connection = new SqlConnection(this.ConnexionString);
        this.Connection.Open();
        if (this.myconnectionEventExists)
        { // attacher l'event sur la connexion
          this.Connection.InfoMessage += this.Connection_InfoMessage;
        }
      }
    }

    /// <summary>
    /// Propage les �v�nements de la connexion au parent
    /// </summary>
    /// <param name="sender">Qui appelle</param>
    /// <param name="e">Param�tre du message</param>
    private void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      if (e.Errors.Count > 0)
      {
        foreach (SqlError err in e.Errors)
        {
          this.MyonConnectionInfoMessage.Invoke(this, SqlMessageEventArgs.From(err));
        }
      }
      else if (!string.IsNullOrWhiteSpace(e.Message))
      {
        this.MyonConnectionInfoMessage.Invoke(this, SqlMessageEventArgs.From(e.Message));
      }
    }

    /// <summary>
    /// Se produit quand une commande est compl�t�e
    /// </summary>
    /// <param name="sender">Qui appelle</param>
    /// <param name="e">Param�tre du message</param>
    private void Command_StatementCompleted(object sender, StatementCompletedEventArgs e)
    {
      this.MyonConnectionInfoMessage.Invoke(this, SqlMessageEventArgs.From(e));
    }
    #endregion
  }
}