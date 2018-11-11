using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseSaver.Data;

namespace BaseSaver.Demo
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// La chaine de connexion
    /// </summary>
    public static string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["versionDb"].ConnectionString;

    delegate void CallMe();
    delegate void CallMeParam(int n);
    delegate void CallMeDataset(DataSet d);

    public Form1()
    {
      InitializeComponent();
    }

    private void Timer1_Tick(object sender, EventArgs e)
    {
      this.label2.Text = $"{DateTime.Now:HH:mm:ss.fff}";
    }

    private void Button1_Click(object sender, EventArgs e)
    {
      Task.Run(() => this.ProcessingNonquery());
    }

    private void Button2_Click(object sender, EventArgs e)
    {
      Task.Run(() => this.ProcessingDataSet());
    }

    private async Task ProcessingNonquery()
    {
      this.Start();

      string sql = @"
DECLARE @i int = 0;
DECLARE @msg VARCHAR(50);
WHILE @i < 100000
BEGIN
   SET @msg = 'i = '+ Convert(VARCHAR(10), @i);
   IF ((@i % 100 ) = 0) 
    RAISERROR (@msg, 10, 0);
   SET @i = @i+1;
END
";
      int n = 0;
      using (ConnectionParam cnn = new ConnectionParam(ConnectionString))
      {
        cnn.OnConnectionInfoMessage += this.Cnn_OnConnectionInfoMessage;
        n = await cnn.ExecuteNonQueryAsync(sql);
        cnn.OnConnectionInfoMessage -= this.Cnn_OnConnectionInfoMessage;
      }

      this.Stop(n);
    }

    private async Task ProcessingDataSet()
    {
      this.Start();

      string sql = @"
DECLARE @i int = 0;
DECLARE @msg VARCHAR(50);
WHILE @i < 1000000
BEGIN
   SET @msg = 'i = '+ Convert(VARCHAR(10), @i);
   IF ((@i % 100 ) = 0) 
    RAISERROR (@msg, 10, 0);
   SET @i = @i+1;
END
;

 SELECT TOP 10 t.object_id, t.name FROM sys.tables t
;
 SELECT TOp 20 c.object_id, c.name, c.column_id FROm sys.columns c
;";
      DataSet dst = null;
      using (ConnectionParam cnn = new ConnectionParam(ConnectionString))
      {
        dst = await cnn.ExecuteDataSetAsync(sql);
      }

      this.Stop(dst);
    }

    private void Cnn_OnConnectionInfoMessage(object sender, SqlMessageEventArgs e)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new EventHandler<SqlMessageEventArgs>(Cnn_OnConnectionInfoMessage), sender, e);
      }
      else
      {
        this.listBox1.Items.Add(e);
        this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
      }
    }

    private void Start()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new CallMe(Start));
      }
      else
      {
        this.label1.Text = "Running...";
        this.listBox1.Items.Clear();
        this.timer1.Enabled = true;
        this.button1.Enabled = false;
        Application.DoEvents();
      }
    }

    private void Stop(int code)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new CallMeParam(Stop), code);
      }
      else
      {
        this.label1.Text = "Fini";
        this.timer1.Enabled = false;
        this.button1.Enabled = true;
      }
    }

    private void Stop(DataSet dst)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new CallMeDataset(Stop), dst);
      }
      else
      {
        this.listBox1.Items.Clear();
        int i = 1;
        foreach(DataTable tbl in dst.Tables)
        {
          this.listBox1.Items.Add($"Table {i++}");
          foreach (DataRow row in tbl.Rows)
          {
            foreach (DataColumn cl in tbl.Columns)
            {
              this.listBox1.Items.Add($"Col{cl.ColumnName} = '{SqlConvert.To(row, cl.ColumnName, string.Empty)}'");
            }
          }
        }

        this.label1.Text = "Fini";
        this.timer1.Enabled = false;
        this.button1.Enabled = true;
      }
    }
  }
}
