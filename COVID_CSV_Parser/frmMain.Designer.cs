namespace COVID_CSV_Parser
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.logTxt = new System.Windows.Forms.TextBox();
            this.btnGetLatestData = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnProcessSelectedFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtSelectedFilename = new System.Windows.Forms.TextBox();
            this.btnDownloadFile = new System.Windows.Forms.Button();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFileProcessed = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // logTxt
            // 
            this.logTxt.Location = new System.Drawing.Point(11, 248);
            this.logTxt.Multiline = true;
            this.logTxt.Name = "logTxt";
            this.logTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTxt.Size = new System.Drawing.Size(548, 210);
            this.logTxt.TabIndex = 0;
            // 
            // btnGetLatestData
            // 
            this.btnGetLatestData.Location = new System.Drawing.Point(12, 16);
            this.btnGetLatestData.Name = "btnGetLatestData";
            this.btnGetLatestData.Size = new System.Drawing.Size(149, 23);
            this.btnGetLatestData.TabIndex = 1;
            this.btnGetLatestData.Text = "Force Manual Update";
            this.btnGetLatestData.UseVisualStyleBackColor = true;
            this.btnGetLatestData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(59, 464);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(501, 20);
            this.txtStatus.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 471);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Status:";
            // 
            // btnProcessSelectedFile
            // 
            this.btnProcessSelectedFile.Location = new System.Drawing.Point(178, 16);
            this.btnProcessSelectedFile.Name = "btnProcessSelectedFile";
            this.btnProcessSelectedFile.Size = new System.Drawing.Size(149, 23);
            this.btnProcessSelectedFile.TabIndex = 4;
            this.btnProcessSelectedFile.Text = "Select File - Manual Update";
            this.btnProcessSelectedFile.UseVisualStyleBackColor = true;
            this.btnProcessSelectedFile.Click += new System.EventHandler(this.btnProcessSelectedFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csv";
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.InitialDirectory = "C:\\Temp";
            this.openFileDialog.Title = "Select CSV Data File for Processing";
            // 
            // txtSelectedFilename
            // 
            this.txtSelectedFilename.Location = new System.Drawing.Point(333, 19);
            this.txtSelectedFilename.Name = "txtSelectedFilename";
            this.txtSelectedFilename.ReadOnly = true;
            this.txtSelectedFilename.Size = new System.Drawing.Size(227, 20);
            this.txtSelectedFilename.TabIndex = 5;
            // 
            // btnDownloadFile
            // 
            this.btnDownloadFile.Location = new System.Drawing.Point(178, 61);
            this.btnDownloadFile.Name = "btnDownloadFile";
            this.btnDownloadFile.Size = new System.Drawing.Size(149, 23);
            this.btnDownloadFile.TabIndex = 6;
            this.btnDownloadFile.Text = "Download File for Date";
            this.btnDownloadFile.UseVisualStyleBackColor = true;
            this.btnDownloadFile.Click += new System.EventHandler(this.btnDownloadFile_Click);
            // 
            // monthCalendar
            // 
            this.monthCalendar.Location = new System.Drawing.Point(333, 61);
            this.monthCalendar.MaxDate = new System.DateTime(2020, 12, 31, 0, 0, 0, 0);
            this.monthCalendar.MaxSelectionCount = 1;
            this.monthCalendar.MinDate = new System.DateTime(2020, 1, 22, 0, 0, 0, 0);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 228);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Data:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 228);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "(Today\'s Date Not Valid Until 8:00 PM EDT)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Data file processeed:";
            // 
            // lblFileProcessed
            // 
            this.lblFileProcessed.AutoSize = true;
            this.lblFileProcessed.Location = new System.Drawing.Point(175, 115);
            this.lblFileProcessed.Name = "lblFileProcessed";
            this.lblFileProcessed.Size = new System.Drawing.Size(88, 13);
            this.lblFileProcessed.TabIndex = 11;
            this.lblFileProcessed.Text = "No date selected";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 496);
            this.Controls.Add(this.lblFileProcessed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.monthCalendar);
            this.Controls.Add(this.btnDownloadFile);
            this.Controls.Add(this.txtSelectedFilename);
            this.Controls.Add(this.btnProcessSelectedFile);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnGetLatestData);
            this.Controls.Add(this.logTxt);
            this.Name = "frmMain";
            this.Text = "COVID-19 CSV Data File Parser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logTxt;
        private System.Windows.Forms.Button btnGetLatestData;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnProcessSelectedFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox txtSelectedFilename;
        private System.Windows.Forms.Button btnDownloadFile;
        private System.Windows.Forms.MonthCalendar monthCalendar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFileProcessed;
    }
}

