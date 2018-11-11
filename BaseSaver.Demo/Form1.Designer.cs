namespace BaseSaver.Demo
{
  partial class Form1
  {
    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Code généré par le Concepteur Windows Form

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.label1 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.label2 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.button2 = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(61, 55);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(46, 17);
      this.label1.TabIndex = 0;
      this.label1.Text = "label1";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(179, 48);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(166, 31);
      this.button1.TabIndex = 1;
      this.button1.Text = "Play long NonQuery";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.Button1_Click);
      // 
      // timer1
      // 
      this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(299, 12);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(46, 17);
      this.label2.TabIndex = 2;
      this.label2.Text = "label2";
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(116, 147);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(100, 22);
      this.textBox1.TabIndex = 3;
      // 
      // trackBar1
      // 
      this.trackBar1.Location = new System.Drawing.Point(12, 199);
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size(375, 56);
      this.trackBar1.TabIndex = 4;
      // 
      // listBox1
      // 
      this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listBox1.FormattingEnabled = true;
      this.listBox1.IntegralHeight = false;
      this.listBox1.ItemHeight = 16;
      this.listBox1.Location = new System.Drawing.Point(448, 12);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(340, 433);
      this.listBox1.TabIndex = 5;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(179, 95);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(166, 31);
      this.button2.TabIndex = 6;
      this.button2.Text = "Play long DataSet";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.Button2_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.listBox1);
      this.Controls.Add(this.trackBar1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.label1);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TrackBar trackBar1;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Button button2;
  }
}

