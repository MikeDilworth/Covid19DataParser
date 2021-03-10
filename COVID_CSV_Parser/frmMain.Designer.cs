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
            this.btnGetLatestCountyData = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblData = new System.Windows.Forms.Label();
            this.btnProcessSelectedCountyFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtSelectedCountyFilename = new System.Windows.Forms.TextBox();
            this.btnDownloadCountyFile = new System.Windows.Forms.Button();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.label2 = new System.Windows.Forms.Label();
            this.lblProcessedCounty = new System.Windows.Forms.Label();
            this.lblCountyFileProcessed = new System.Windows.Forms.Label();
            this.chkShowLogData = new System.Windows.Forms.CheckBox();
            this.lblStateFileProcessed = new System.Windows.Forms.Label();
            this.lblProcessedState = new System.Windows.Forms.Label();
            this.btnGetLatestStateData = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGetLatestStateVaccinationData = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.btnProcessSelectedVaccinationFile = new System.Windows.Forms.Button();
            this.txtVaccinationDataFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // logTxt
            // 
            this.logTxt.Location = new System.Drawing.Point(11, 387);
            this.logTxt.Multiline = true;
            this.logTxt.Name = "logTxt";
            this.logTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTxt.Size = new System.Drawing.Size(548, 173);
            this.logTxt.TabIndex = 0;
            // 
            // btnGetLatestCountyData
            // 
            this.btnGetLatestCountyData.Location = new System.Drawing.Point(12, 147);
            this.btnGetLatestCountyData.Name = "btnGetLatestCountyData";
            this.btnGetLatestCountyData.Size = new System.Drawing.Size(149, 23);
            this.btnGetLatestCountyData.TabIndex = 1;
            this.btnGetLatestCountyData.Text = "Force Manual Update";
            this.btnGetLatestCountyData.UseVisualStyleBackColor = true;
            this.btnGetLatestCountyData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(58, 566);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(501, 20);
            this.txtStatus.TabIndex = 2;
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.Location = new System.Drawing.Point(12, 368);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(33, 13);
            this.lblData.TabIndex = 3;
            this.lblData.Text = "Data:";
            // 
            // btnProcessSelectedCountyFile
            // 
            this.btnProcessSelectedCountyFile.Location = new System.Drawing.Point(179, 147);
            this.btnProcessSelectedCountyFile.Name = "btnProcessSelectedCountyFile";
            this.btnProcessSelectedCountyFile.Size = new System.Drawing.Size(149, 23);
            this.btnProcessSelectedCountyFile.TabIndex = 4;
            this.btnProcessSelectedCountyFile.Text = "Select File - Manual Update";
            this.btnProcessSelectedCountyFile.UseVisualStyleBackColor = true;
            this.btnProcessSelectedCountyFile.Click += new System.EventHandler(this.btnProcessSelectedFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csv";
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.InitialDirectory = "C:\\Temp";
            this.openFileDialog.Title = "Select CSV Data File for Processing";
            // 
            // txtSelectedCountyFilename
            // 
            this.txtSelectedCountyFilename.Location = new System.Drawing.Point(334, 150);
            this.txtSelectedCountyFilename.Name = "txtSelectedCountyFilename";
            this.txtSelectedCountyFilename.ReadOnly = true;
            this.txtSelectedCountyFilename.Size = new System.Drawing.Size(227, 20);
            this.txtSelectedCountyFilename.TabIndex = 5;
            // 
            // btnDownloadCountyFile
            // 
            this.btnDownloadCountyFile.Location = new System.Drawing.Point(179, 188);
            this.btnDownloadCountyFile.Name = "btnDownloadCountyFile";
            this.btnDownloadCountyFile.Size = new System.Drawing.Size(149, 23);
            this.btnDownloadCountyFile.TabIndex = 6;
            this.btnDownloadCountyFile.Text = "Download File for Date";
            this.btnDownloadCountyFile.UseVisualStyleBackColor = true;
            this.btnDownloadCountyFile.Click += new System.EventHandler(this.btnDownloadFile_Click);
            // 
            // monthCalendar
            // 
            this.monthCalendar.Location = new System.Drawing.Point(332, 188);
            this.monthCalendar.MaxDate = new System.DateTime(2022, 12, 31, 0, 0, 0, 0);
            this.monthCalendar.MaxSelectionCount = 1;
            this.monthCalendar.MinDate = new System.DateTime(2020, 1, 22, 0, 0, 0, 0);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(329, 364);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "(Today\'s Date Not Valid Until 9:00 PM EDT)";
            // 
            // lblProcessedCounty
            // 
            this.lblProcessedCounty.AutoSize = true;
            this.lblProcessedCounty.Location = new System.Drawing.Point(176, 232);
            this.lblProcessedCounty.Name = "lblProcessedCounty";
            this.lblProcessedCounty.Size = new System.Drawing.Size(101, 13);
            this.lblProcessedCounty.TabIndex = 10;
            this.lblProcessedCounty.Text = "Data file processed:";
            // 
            // lblCountyFileProcessed
            // 
            this.lblCountyFileProcessed.AutoSize = true;
            this.lblCountyFileProcessed.Location = new System.Drawing.Point(176, 249);
            this.lblCountyFileProcessed.Name = "lblCountyFileProcessed";
            this.lblCountyFileProcessed.Size = new System.Drawing.Size(88, 13);
            this.lblCountyFileProcessed.TabIndex = 11;
            this.lblCountyFileProcessed.Text = "No date selected";
            // 
            // chkShowLogData
            // 
            this.chkShowLogData.AutoSize = true;
            this.chkShowLogData.Location = new System.Drawing.Point(51, 368);
            this.chkShowLogData.Name = "chkShowLogData";
            this.chkShowLogData.Size = new System.Drawing.Size(142, 17);
            this.chkShowLogData.TabIndex = 12;
            this.chkShowLogData.Text = "Show Detailed Log Data";
            this.chkShowLogData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkShowLogData.UseVisualStyleBackColor = true;
            this.chkShowLogData.Visible = false;
            // 
            // lblStateFileProcessed
            // 
            this.lblStateFileProcessed.AutoSize = true;
            this.lblStateFileProcessed.Location = new System.Drawing.Point(176, 39);
            this.lblStateFileProcessed.Name = "lblStateFileProcessed";
            this.lblStateFileProcessed.Size = new System.Drawing.Size(88, 13);
            this.lblStateFileProcessed.TabIndex = 18;
            this.lblStateFileProcessed.Text = "No date selected";
            // 
            // lblProcessedState
            // 
            this.lblProcessedState.AutoSize = true;
            this.lblProcessedState.Location = new System.Drawing.Point(176, 21);
            this.lblProcessedState.Name = "lblProcessedState";
            this.lblProcessedState.Size = new System.Drawing.Size(101, 13);
            this.lblProcessedState.TabIndex = 17;
            this.lblProcessedState.Text = "Data file processed:";
            // 
            // btnGetLatestStateData
            // 
            this.btnGetLatestStateData.Location = new System.Drawing.Point(13, 33);
            this.btnGetLatestStateData.Name = "btnGetLatestStateData";
            this.btnGetLatestStateData.Size = new System.Drawing.Size(149, 23);
            this.btnGetLatestStateData.TabIndex = 13;
            this.btnGetLatestStateData.Text = "Force Manual Update";
            this.btnGetLatestStateData.UseVisualStyleBackColor = true;
            this.btnGetLatestStateData.Click += new System.EventHandler(this.btnGetLatestStateData_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(10, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 16);
            this.label5.TabIndex = 19;
            this.label5.Text = "County-Level Data:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(10, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 16);
            this.label6.TabIndex = 20;
            this.label6.Text = "State-Level Data:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 573);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Status:";
            // 
            // btnGetLatestStateVaccinationData
            // 
            this.btnGetLatestStateVaccinationData.Location = new System.Drawing.Point(13, 62);
            this.btnGetLatestStateVaccinationData.Name = "btnGetLatestStateVaccinationData";
            this.btnGetLatestStateVaccinationData.Size = new System.Drawing.Size(149, 23);
            this.btnGetLatestStateVaccinationData.TabIndex = 22;
            this.btnGetLatestStateVaccinationData.Text = "Vaccination Data";
            this.btnGetLatestStateVaccinationData.UseVisualStyleBackColor = true;
            this.btnGetLatestStateVaccinationData.Click += new System.EventHandler(this.btnGetLatestStateVaccinationData_Click);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "json";
            this.openFileDialog2.FileName = "openFileDialog1";
            this.openFileDialog2.InitialDirectory = "C:\\Covid19JSONVaccinationDataFiles";
            this.openFileDialog2.Title = "Select CSV Data File for Processing";
            // 
            // btnProcessSelectedVaccinationFile
            // 
            this.btnProcessSelectedVaccinationFile.Location = new System.Drawing.Point(13, 91);
            this.btnProcessSelectedVaccinationFile.Name = "btnProcessSelectedVaccinationFile";
            this.btnProcessSelectedVaccinationFile.Size = new System.Drawing.Size(149, 23);
            this.btnProcessSelectedVaccinationFile.TabIndex = 23;
            this.btnProcessSelectedVaccinationFile.Text = "Select File - Manual Update";
            this.btnProcessSelectedVaccinationFile.UseVisualStyleBackColor = true;
            this.btnProcessSelectedVaccinationFile.Click += new System.EventHandler(this.btnProcessSelectedVaccinationFile_Click);
            // 
            // txtVaccinationDataFile
            // 
            this.txtVaccinationDataFile.AutoSize = true;
            this.txtVaccinationDataFile.Location = new System.Drawing.Point(176, 96);
            this.txtVaccinationDataFile.Name = "txtVaccinationDataFile";
            this.txtVaccinationDataFile.Size = new System.Drawing.Size(80, 13);
            this.txtVaccinationDataFile.TabIndex = 24;
            this.txtVaccinationDataFile.Text = "No file selected";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 606);
            this.Controls.Add(this.txtVaccinationDataFile);
            this.Controls.Add(this.btnProcessSelectedVaccinationFile);
            this.Controls.Add(this.btnGetLatestStateVaccinationData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblStateFileProcessed);
            this.Controls.Add(this.lblProcessedState);
            this.Controls.Add(this.btnGetLatestStateData);
            this.Controls.Add(this.chkShowLogData);
            this.Controls.Add(this.lblCountyFileProcessed);
            this.Controls.Add(this.lblProcessedCounty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.monthCalendar);
            this.Controls.Add(this.btnDownloadCountyFile);
            this.Controls.Add(this.txtSelectedCountyFilename);
            this.Controls.Add(this.btnProcessSelectedCountyFile);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnGetLatestCountyData);
            this.Controls.Add(this.logTxt);
            this.Name = "frmMain";
            this.Text = "COVID-19 CSV Data File Parser  Version 1.1.5";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logTxt;
        private System.Windows.Forms.Button btnGetLatestCountyData;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.Button btnProcessSelectedCountyFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox txtSelectedCountyFilename;
        private System.Windows.Forms.Button btnDownloadCountyFile;
        private System.Windows.Forms.MonthCalendar monthCalendar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblProcessedCounty;
        private System.Windows.Forms.Label lblCountyFileProcessed;
        private System.Windows.Forms.CheckBox chkShowLogData;
        private System.Windows.Forms.Label lblStateFileProcessed;
        private System.Windows.Forms.Label lblProcessedState;
        private System.Windows.Forms.Button btnGetLatestStateData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGetLatestStateVaccinationData;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button btnProcessSelectedVaccinationFile;
        private System.Windows.Forms.Label txtVaccinationDataFile;
    }
}

