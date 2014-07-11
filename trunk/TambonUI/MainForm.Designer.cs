namespace De.AHoerstemeier.Tambon.UI
{
    partial class MainForm
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
            if ( disposing && (components != null) )
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
            System.Windows.Forms.Button btn_LoadGazetteXML;
            this.btnCheckNames = new System.Windows.Forms.Button();
            this.btn_PopulationAllProvinces = new System.Windows.Forms.Button();
            this.btnCreateKml = new System.Windows.Forms.Button();
            this.btn_GazetteShowAll = new System.Windows.Forms.Button();
            this.btn_CheckForNews = new System.Windows.Forms.Button();
            this.btn_GazetteShow = new System.Windows.Forms.Button();
            this.btn_GazetteLoadAll = new System.Windows.Forms.Button();
            this.btn_PopulationAll = new System.Windows.Forms.Button();
            this.cbxChangwat = new System.Windows.Forms.ComboBox();
            this.edtYear = new System.Windows.Forms.NumericUpDown();
            this.btn_Population = new System.Windows.Forms.Button();
            this.btnCheckTerms = new System.Windows.Forms.Button();
            this.btnCreationStatistics = new System.Windows.Forms.Button();
            this.btnTambonStatistics = new System.Windows.Forms.Button();
            this.btnMubanStatistics = new System.Windows.Forms.Button();
            this.btnChumchonStatistics = new System.Windows.Forms.Button();
            this.btnTermEnds = new System.Windows.Forms.Button();
            this.btnTimeBetweenElection = new System.Windows.Forms.Button();
            this.btnElectionWeekday = new System.Windows.Forms.Button();
            this.btnMubanHelper = new System.Windows.Forms.Button();
            this.btnPendingElections = new System.Windows.Forms.Button();
            this.btnWikiData = new System.Windows.Forms.Button();
            this.btnElectionDates = new System.Windows.Forms.Button();
            this.chkAllProvince = new System.Windows.Forms.CheckBox();
            this.btnNayokResign = new System.Windows.Forms.Button();
            this.btnShowEntityData = new System.Windows.Forms.Button();
            btn_LoadGazetteXML = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.edtYear)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_LoadGazetteXML
            // 
            btn_LoadGazetteXML.Location = new System.Drawing.Point(57, -34);
            btn_LoadGazetteXML.Name = "btn_LoadGazetteXML";
            btn_LoadGazetteXML.Size = new System.Drawing.Size(98, 23);
            btn_LoadGazetteXML.TabIndex = 34;
            btn_LoadGazetteXML.Text = "Gazette XML";
            btn_LoadGazetteXML.UseVisualStyleBackColor = true;
            // 
            // btnCheckNames
            // 
            this.btnCheckNames.Location = new System.Drawing.Point(12, 39);
            this.btnCheckNames.Name = "btnCheckNames";
            this.btnCheckNames.Size = new System.Drawing.Size(88, 23);
            this.btnCheckNames.TabIndex = 45;
            this.btnCheckNames.Text = "Check Names";
            this.btnCheckNames.UseVisualStyleBackColor = true;
            this.btnCheckNames.Click += new System.EventHandler(this.btnCheckNames_Click);
            // 
            // btn_PopulationAllProvinces
            // 
            this.btn_PopulationAllProvinces.Location = new System.Drawing.Point(419, 303);
            this.btn_PopulationAllProvinces.Name = "btn_PopulationAllProvinces";
            this.btn_PopulationAllProvinces.Size = new System.Drawing.Size(75, 23);
            this.btn_PopulationAllProvinces.TabIndex = 44;
            this.btn_PopulationAllProvinces.Text = "All provinces";
            this.btn_PopulationAllProvinces.UseVisualStyleBackColor = true;
            // 
            // btnCreateKml
            // 
            this.btnCreateKml.Location = new System.Drawing.Point(197, 361);
            this.btnCreateKml.Name = "btnCreateKml";
            this.btnCreateKml.Size = new System.Drawing.Size(99, 23);
            this.btnCreateKml.TabIndex = 43;
            this.btnCreateKml.Text = "Create KML";
            this.btnCreateKml.UseVisualStyleBackColor = true;
            this.btnCreateKml.Click += new System.EventHandler(this.btnCreateKml_Click);
            // 
            // btn_GazetteShowAll
            // 
            this.btn_GazetteShowAll.Enabled = false;
            this.btn_GazetteShowAll.Location = new System.Drawing.Point(419, 274);
            this.btn_GazetteShowAll.Name = "btn_GazetteShowAll";
            this.btn_GazetteShowAll.Size = new System.Drawing.Size(75, 23);
            this.btn_GazetteShowAll.TabIndex = 42;
            this.btn_GazetteShowAll.Text = "Show All";
            this.btn_GazetteShowAll.UseVisualStyleBackColor = true;
            // 
            // btn_CheckForNews
            // 
            this.btn_CheckForNews.Location = new System.Drawing.Point(197, 332);
            this.btn_CheckForNews.Name = "btn_CheckForNews";
            this.btn_CheckForNews.Size = new System.Drawing.Size(112, 23);
            this.btn_CheckForNews.TabIndex = 41;
            this.btn_CheckForNews.Text = "Check for News";
            this.btn_CheckForNews.UseVisualStyleBackColor = true;
            // 
            // btn_GazetteShow
            // 
            this.btn_GazetteShow.Enabled = false;
            this.btn_GazetteShow.Location = new System.Drawing.Point(334, 274);
            this.btn_GazetteShow.Name = "btn_GazetteShow";
            this.btn_GazetteShow.Size = new System.Drawing.Size(79, 23);
            this.btn_GazetteShow.TabIndex = 40;
            this.btn_GazetteShow.Text = "Filtered";
            this.btn_GazetteShow.UseVisualStyleBackColor = true;
            // 
            // btn_GazetteLoadAll
            // 
            this.btn_GazetteLoadAll.Location = new System.Drawing.Point(419, 10);
            this.btn_GazetteLoadAll.Name = "btn_GazetteLoadAll";
            this.btn_GazetteLoadAll.Size = new System.Drawing.Size(112, 23);
            this.btn_GazetteLoadAll.TabIndex = 39;
            this.btn_GazetteLoadAll.Text = "Load gazette";
            this.btn_GazetteLoadAll.UseVisualStyleBackColor = true;
            this.btn_GazetteLoadAll.Click += new System.EventHandler(this.btnGazetteLoadAll_Click);
            // 
            // btn_PopulationAll
            // 
            this.btn_PopulationAll.Enabled = false;
            this.btn_PopulationAll.Location = new System.Drawing.Point(315, 303);
            this.btn_PopulationAll.Name = "btn_PopulationAll";
            this.btn_PopulationAll.Size = new System.Drawing.Size(98, 23);
            this.btn_PopulationAll.TabIndex = 38;
            this.btn_PopulationAll.Text = "All Years";
            this.btn_PopulationAll.UseVisualStyleBackColor = true;
            // 
            // cbxChangwat
            // 
            this.cbxChangwat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxChangwat.FormattingEnabled = true;
            this.cbxChangwat.Location = new System.Drawing.Point(12, 12);
            this.cbxChangwat.Name = "cbxChangwat";
            this.cbxChangwat.Size = new System.Drawing.Size(226, 21);
            this.cbxChangwat.TabIndex = 37;
            // 
            // edtYear
            // 
            this.edtYear.Location = new System.Drawing.Point(12, 277);
            this.edtYear.Maximum = new decimal(new int[] {
            2008,
            0,
            0,
            0});
            this.edtYear.Minimum = new decimal(new int[] {
            1993,
            0,
            0,
            0});
            this.edtYear.Name = "edtYear";
            this.edtYear.Size = new System.Drawing.Size(57, 20);
            this.edtYear.TabIndex = 36;
            this.edtYear.Value = new decimal(new int[] {
            2005,
            0,
            0,
            0});
            // 
            // btn_Population
            // 
            this.btn_Population.Location = new System.Drawing.Point(197, 303);
            this.btn_Population.Name = "btn_Population";
            this.btn_Population.Size = new System.Drawing.Size(112, 23);
            this.btn_Population.TabIndex = 35;
            this.btn_Population.Text = "Population";
            this.btn_Population.UseVisualStyleBackColor = true;
            this.btn_Population.Click += new System.EventHandler(this.btn_Population_Click);
            // 
            // btnCheckTerms
            // 
            this.btnCheckTerms.Location = new System.Drawing.Point(12, 68);
            this.btnCheckTerms.Name = "btnCheckTerms";
            this.btnCheckTerms.Size = new System.Drawing.Size(88, 23);
            this.btnCheckTerms.TabIndex = 46;
            this.btnCheckTerms.Text = "Check terms";
            this.btnCheckTerms.UseVisualStyleBackColor = true;
            this.btnCheckTerms.Click += new System.EventHandler(this.btnCheckTerms_Click);
            // 
            // btnCreationStatistics
            // 
            this.btnCreationStatistics.Location = new System.Drawing.Point(18, 102);
            this.btnCreationStatistics.Name = "btnCreationStatistics";
            this.btnCreationStatistics.Size = new System.Drawing.Size(103, 23);
            this.btnCreationStatistics.TabIndex = 47;
            this.btnCreationStatistics.Text = "Creation Statistics";
            this.btnCreationStatistics.UseVisualStyleBackColor = true;
            this.btnCreationStatistics.Click += new System.EventHandler(this.btnCreationStatistics_Click);
            // 
            // btnTambonStatistics
            // 
            this.btnTambonStatistics.Location = new System.Drawing.Point(32, 148);
            this.btnTambonStatistics.Name = "btnTambonStatistics";
            this.btnTambonStatistics.Size = new System.Drawing.Size(105, 23);
            this.btnTambonStatistics.TabIndex = 48;
            this.btnTambonStatistics.Text = "Tambon Stats";
            this.btnTambonStatistics.UseVisualStyleBackColor = true;
            this.btnTambonStatistics.Click += new System.EventHandler(this.btnTambonStatistics_Click);
            // 
            // btnMubanStatistics
            // 
            this.btnMubanStatistics.Location = new System.Drawing.Point(32, 177);
            this.btnMubanStatistics.Name = "btnMubanStatistics";
            this.btnMubanStatistics.Size = new System.Drawing.Size(105, 23);
            this.btnMubanStatistics.TabIndex = 49;
            this.btnMubanStatistics.Text = "Muban Stats";
            this.btnMubanStatistics.UseVisualStyleBackColor = true;
            this.btnMubanStatistics.Click += new System.EventHandler(this.btnMubanStatistics_Click);
            // 
            // btnChumchonStatistics
            // 
            this.btnChumchonStatistics.Location = new System.Drawing.Point(32, 206);
            this.btnChumchonStatistics.Name = "btnChumchonStatistics";
            this.btnChumchonStatistics.Size = new System.Drawing.Size(105, 23);
            this.btnChumchonStatistics.TabIndex = 50;
            this.btnChumchonStatistics.Text = "Chumchon Stats";
            this.btnChumchonStatistics.UseVisualStyleBackColor = true;
            this.btnChumchonStatistics.Click += new System.EventHandler(this.btnChumchonStatistics_Click);
            // 
            // btnTermEnds
            // 
            this.btnTermEnds.Location = new System.Drawing.Point(266, 97);
            this.btnTermEnds.Name = "btnTermEnds";
            this.btnTermEnds.Size = new System.Drawing.Size(99, 23);
            this.btnTermEnds.TabIndex = 51;
            this.btnTermEnds.Text = "Term ends";
            this.btnTermEnds.UseVisualStyleBackColor = true;
            this.btnTermEnds.Click += new System.EventHandler(this.btnTermEnds_Click);
            // 
            // btnTimeBetweenElection
            // 
            this.btnTimeBetweenElection.Location = new System.Drawing.Point(163, 45);
            this.btnTimeBetweenElection.Name = "btnTimeBetweenElection";
            this.btnTimeBetweenElection.Size = new System.Drawing.Size(75, 23);
            this.btnTimeBetweenElection.TabIndex = 52;
            this.btnTimeBetweenElection.Text = "Interregnum";
            this.btnTimeBetweenElection.UseVisualStyleBackColor = true;
            this.btnTimeBetweenElection.Click += new System.EventHandler(this.btnTimeBetweenElection_Click);
            // 
            // btnElectionWeekday
            // 
            this.btnElectionWeekday.Location = new System.Drawing.Point(163, 74);
            this.btnElectionWeekday.Name = "btnElectionWeekday";
            this.btnElectionWeekday.Size = new System.Drawing.Size(75, 23);
            this.btnElectionWeekday.TabIndex = 53;
            this.btnElectionWeekday.Text = "Weekdays";
            this.btnElectionWeekday.UseVisualStyleBackColor = true;
            this.btnElectionWeekday.Click += new System.EventHandler(this.btnElectionWeekday_Click);
            // 
            // btnMubanHelper
            // 
            this.btnMubanHelper.Location = new System.Drawing.Point(143, 177);
            this.btnMubanHelper.Name = "btnMubanHelper";
            this.btnMubanHelper.Size = new System.Drawing.Size(75, 23);
            this.btnMubanHelper.TabIndex = 54;
            this.btnMubanHelper.Text = "Muban helper";
            this.btnMubanHelper.UseVisualStyleBackColor = true;
            this.btnMubanHelper.Click += new System.EventHandler(this.btnMubanHelper_Click);
            // 
            // btnPendingElections
            // 
            this.btnPendingElections.Location = new System.Drawing.Point(266, 68);
            this.btnPendingElections.Name = "btnPendingElections";
            this.btnPendingElections.Size = new System.Drawing.Size(99, 23);
            this.btnPendingElections.TabIndex = 55;
            this.btnPendingElections.Text = "Pending elections";
            this.btnPendingElections.UseVisualStyleBackColor = true;
            this.btnPendingElections.Click += new System.EventHandler(this.btnPendingElections_Click);
            // 
            // btnWikiData
            // 
            this.btnWikiData.Location = new System.Drawing.Point(315, 361);
            this.btnWikiData.Name = "btnWikiData";
            this.btnWikiData.Size = new System.Drawing.Size(75, 23);
            this.btnWikiData.TabIndex = 56;
            this.btnWikiData.Text = "WikiData";
            this.btnWikiData.UseVisualStyleBackColor = true;
            this.btnWikiData.Click += new System.EventHandler(this.btnWikiData_Click);
            // 
            // btnElectionDates
            // 
            this.btnElectionDates.Location = new System.Drawing.Point(163, 102);
            this.btnElectionDates.Name = "btnElectionDates";
            this.btnElectionDates.Size = new System.Drawing.Size(75, 23);
            this.btnElectionDates.TabIndex = 57;
            this.btnElectionDates.Text = "Dates";
            this.btnElectionDates.UseVisualStyleBackColor = true;
            this.btnElectionDates.Click += new System.EventHandler(this.btnElectionDates_Click);
            // 
            // chkAllProvince
            // 
            this.chkAllProvince.AutoSize = true;
            this.chkAllProvince.Location = new System.Drawing.Point(266, 49);
            this.chkAllProvince.Name = "chkAllProvince";
            this.chkAllProvince.Size = new System.Drawing.Size(81, 17);
            this.chkAllProvince.TabIndex = 58;
            this.chkAllProvince.Text = "All province";
            this.chkAllProvince.UseVisualStyleBackColor = true;
            // 
            // btnNayokResign
            // 
            this.btnNayokResign.Location = new System.Drawing.Point(380, 68);
            this.btnNayokResign.Name = "btnNayokResign";
            this.btnNayokResign.Size = new System.Drawing.Size(99, 23);
            this.btnNayokResign.TabIndex = 59;
            this.btnNayokResign.Text = "Nayok resign";
            this.btnNayokResign.UseVisualStyleBackColor = true;
            this.btnNayokResign.Click += new System.EventHandler(this.btnNayokResign_Click);
            // 
            // btnShowEntityData
            // 
            this.btnShowEntityData.Location = new System.Drawing.Point(325, 10);
            this.btnShowEntityData.Name = "btnShowEntityData";
            this.btnShowEntityData.Size = new System.Drawing.Size(75, 23);
            this.btnShowEntityData.TabIndex = 60;
            this.btnShowEntityData.Text = "Show Data";
            this.btnShowEntityData.UseVisualStyleBackColor = true;
            this.btnShowEntityData.Click += new System.EventHandler(this.btnShowEntityData_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 425);
            this.Controls.Add(this.btnShowEntityData);
            this.Controls.Add(this.btnNayokResign);
            this.Controls.Add(this.chkAllProvince);
            this.Controls.Add(this.btnElectionDates);
            this.Controls.Add(this.btnWikiData);
            this.Controls.Add(this.btnPendingElections);
            this.Controls.Add(this.btnMubanHelper);
            this.Controls.Add(this.btnElectionWeekday);
            this.Controls.Add(this.btnTimeBetweenElection);
            this.Controls.Add(this.btnTermEnds);
            this.Controls.Add(this.btnChumchonStatistics);
            this.Controls.Add(this.btnMubanStatistics);
            this.Controls.Add(this.btnTambonStatistics);
            this.Controls.Add(this.btnCreationStatistics);
            this.Controls.Add(this.btnCheckTerms);
            this.Controls.Add(this.btnCheckNames);
            this.Controls.Add(this.btn_PopulationAllProvinces);
            this.Controls.Add(this.btnCreateKml);
            this.Controls.Add(this.btn_GazetteShowAll);
            this.Controls.Add(this.btn_CheckForNews);
            this.Controls.Add(this.btn_GazetteShow);
            this.Controls.Add(this.btn_GazetteLoadAll);
            this.Controls.Add(this.btn_PopulationAll);
            this.Controls.Add(this.cbxChangwat);
            this.Controls.Add(this.edtYear);
            this.Controls.Add(this.btn_Population);
            this.Controls.Add(btn_LoadGazetteXML);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edtYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCheckNames;
        private System.Windows.Forms.Button btn_PopulationAllProvinces;
        private System.Windows.Forms.Button btnCreateKml;
        private System.Windows.Forms.Button btn_GazetteShowAll;
        private System.Windows.Forms.Button btn_CheckForNews;
        private System.Windows.Forms.Button btn_GazetteShow;
        private System.Windows.Forms.Button btn_GazetteLoadAll;
        private System.Windows.Forms.Button btn_PopulationAll;
        private System.Windows.Forms.ComboBox cbxChangwat;
        private System.Windows.Forms.NumericUpDown edtYear;
        private System.Windows.Forms.Button btn_Population;
        private System.Windows.Forms.Button btnCheckTerms;
        private System.Windows.Forms.Button btnCreationStatistics;
        private System.Windows.Forms.Button btnTambonStatistics;
        private System.Windows.Forms.Button btnMubanStatistics;
        private System.Windows.Forms.Button btnChumchonStatistics;
        private System.Windows.Forms.Button btnTermEnds;
        private System.Windows.Forms.Button btnTimeBetweenElection;
        private System.Windows.Forms.Button btnElectionWeekday;
        private System.Windows.Forms.Button btnMubanHelper;
        private System.Windows.Forms.Button btnPendingElections;
        private System.Windows.Forms.Button btnWikiData;
        private System.Windows.Forms.Button btnElectionDates;
        private System.Windows.Forms.CheckBox chkAllProvince;
        private System.Windows.Forms.Button btnNayokResign;
        private System.Windows.Forms.Button btnShowEntityData;
    }
}

