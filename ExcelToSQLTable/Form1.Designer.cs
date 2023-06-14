
namespace ExcelToSQLTable
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnSelezionaFile = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkDrop = new System.Windows.Forms.CheckBox();
            this.chkTran = new System.Windows.Forms.CheckBox();
            this.chkSelect = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCopia = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGeneraScript = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.linkLabel1);
            this.splitContainer1.Panel1.Controls.Add(this.checkedListBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox1);
            this.splitContainer1.Panel1.Controls.Add(this.btnSelezionaFile);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.btnCopia);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.btnGeneraScript);
            this.splitContainer1.Panel2.Controls.Add(this.txtOutput);
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1065, 637);
            this.splitContainer1.SplitterDistance = 325;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(914, 303);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(35, 15);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Tutto";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(914, 35);
            this.checkedListBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(145, 256);
            this.checkedListBox1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(914, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Column selector:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(191, 8);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(206, 23);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnSelezionaFile
            // 
            this.btnSelezionaFile.Location = new System.Drawing.Point(8, 7);
            this.btnSelezionaFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSelezionaFile.Name = "btnSelezionaFile";
            this.btnSelezionaFile.Size = new System.Drawing.Size(178, 24);
            this.btnSelezionaFile.TabIndex = 1;
            this.btnSelezionaFile.Text = "Seleziona il file";
            this.btnSelezionaFile.UseVisualStyleBackColor = true;
            this.btnSelezionaFile.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 35);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 33;
            this.dataGridView1.Size = new System.Drawing.Size(901, 288);
            this.dataGridView1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkDrop);
            this.groupBox1.Controls.Add(this.chkTran);
            this.groupBox1.Controls.Add(this.chkSelect);
            this.groupBox1.Location = new System.Drawing.Point(620, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(312, 40);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Opzioni";
            // 
            // chkDrop
            // 
            this.chkDrop.AutoSize = true;
            this.chkDrop.Checked = true;
            this.chkDrop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDrop.Location = new System.Drawing.Point(164, 18);
            this.chkDrop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkDrop.Name = "chkDrop";
            this.chkDrop.Size = new System.Drawing.Size(81, 19);
            this.chkDrop.TabIndex = 11;
            this.chkDrop.Text = "Drop table";
            this.chkDrop.UseVisualStyleBackColor = true;
            // 
            // chkTran
            // 
            this.chkTran.AutoSize = true;
            this.chkTran.Checked = true;
            this.chkTran.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTran.Location = new System.Drawing.Point(6, 18);
            this.chkTran.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkTran.Name = "chkTran";
            this.chkTran.Size = new System.Drawing.Size(87, 19);
            this.chkTran.TabIndex = 9;
            this.chkTran.Text = "Transazione";
            this.chkTran.UseVisualStyleBackColor = true;
            // 
            // chkSelect
            // 
            this.chkSelect.AutoSize = true;
            this.chkSelect.Checked = true;
            this.chkSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelect.Location = new System.Drawing.Point(100, 18);
            this.chkSelect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.Size = new System.Drawing.Size(57, 19);
            this.chkSelect.TabIndex = 10;
            this.chkSelect.Text = "Select";
            this.chkSelect.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(993, 7);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 35);
            this.button1.TabIndex = 8;
            this.button1.Text = "Svuota";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnCopia
            // 
            this.btnCopia.Location = new System.Drawing.Point(937, 7);
            this.btnCopia.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCopia.Name = "btnCopia";
            this.btnCopia.Size = new System.Drawing.Size(52, 35);
            this.btnCopia.TabIndex = 7;
            this.btnCopia.Text = "Copia";
            this.btnCopia.UseVisualStyleBackColor = true;
            this.btnCopia.Click += new System.EventHandler(this.btnCopia_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(575, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Inserire un nome per la tabella temporanea (in caso il campo si lasciasse vuoto v" +
    "errà preso di default #temp)";
            // 
            // btnGeneraScript
            // 
            this.btnGeneraScript.Location = new System.Drawing.Point(183, 22);
            this.btnGeneraScript.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGeneraScript.Name = "btnGeneraScript";
            this.btnGeneraScript.Size = new System.Drawing.Size(185, 24);
            this.btnGeneraScript.TabIndex = 5;
            this.btnGeneraScript.Text = "Genera script SQL";
            this.btnGeneraScript.UseVisualStyleBackColor = true;
            this.btnGeneraScript.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(8, 50);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(1050, 255);
            this.txtOutput.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(8, 23);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.PlaceholderText = "#temp";
            this.textBox1.Size = new System.Drawing.Size(171, 23);
            this.textBox1.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 637);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Excel columns to SQL temp table";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSelezionaFile;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnGeneraScript;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCopia;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox chkTran;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox chkSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkDrop;
    }
}

