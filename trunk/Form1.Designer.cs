namespace Tambon
{
    partial class Form1
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
            this.btn_GazetteMirror = new System.Windows.Forms.Button();
            this.btn_Population = new System.Windows.Forms.Button();
            this.edt_year = new System.Windows.Forms.NumericUpDown();
            this.cbx_changwat = new System.Windows.Forms.ComboBox();
            this.btn_PopulationAll = new System.Windows.Forms.Button();
            this.btn_GazetteLoad = new System.Windows.Forms.Button();
            this.btn_GazetteShow = new System.Windows.Forms.Button();
            this.btn_CheckForNews = new System.Windows.Forms.Button();
            this.btn_GazetteShowAll = new System.Windows.Forms.Button();
            this.btn_GazetteSearchYear = new System.Windows.Forms.Button();
            this.btn_LoadCcaatt = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_DownloadCcaatt = new System.Windows.Forms.Button();
            this.btn_GazetteNewsSince1970 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.ButtonThesaban = new System.Windows.Forms.Button();
            this.btnSetup = new System.Windows.Forms.Button();
            this.btnNumerals = new System.Windows.Forms.Button();
            this.btnTambonFrequency = new System.Windows.Forms.Button();
            this.btnTambonCreation = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.edt_year)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_GazetteMirror
            // 
            this.btn_GazetteMirror.Enabled = false;
            this.btn_GazetteMirror.Location = new System.Drawing.Point(143, 12);
            this.btn_GazetteMirror.Name = "btn_GazetteMirror";
            this.btn_GazetteMirror.Size = new System.Drawing.Size(98, 23);
            this.btn_GazetteMirror.TabIndex = 0;
            this.btn_GazetteMirror.Text = "Gazette Cache";
            this.btn_GazetteMirror.UseVisualStyleBackColor = true;
            this.btn_GazetteMirror.Click += new System.EventHandler(this.btnGazetteMirror_Click);
            // 
            // btn_Population
            // 
            this.btn_Population.Enabled = false;
            this.btn_Population.Location = new System.Drawing.Point(25, 41);
            this.btn_Population.Name = "btn_Population";
            this.btn_Population.Size = new System.Drawing.Size(112, 23);
            this.btn_Population.TabIndex = 1;
            this.btn_Population.Text = "Population";
            this.btn_Population.UseVisualStyleBackColor = true;
            this.btn_Population.Click += new System.EventHandler(this.btnPopulation_Click);
            // 
            // edt_year
            // 
            this.edt_year.Location = new System.Drawing.Point(25, 70);
            this.edt_year.Maximum = new decimal(new int[] {
            2007,
            0,
            0,
            0});
            this.edt_year.Minimum = new decimal(new int[] {
            1993,
            0,
            0,
            0});
            this.edt_year.Name = "edt_year";
            this.edt_year.Size = new System.Drawing.Size(57, 20);
            this.edt_year.TabIndex = 3;
            this.edt_year.Value = new decimal(new int[] {
            2005,
            0,
            0,
            0});
            // 
            // cbx_changwat
            // 
            this.cbx_changwat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_changwat.FormattingEnabled = true;
            this.cbx_changwat.Location = new System.Drawing.Point(100, 70);
            this.cbx_changwat.Name = "cbx_changwat";
            this.cbx_changwat.Size = new System.Drawing.Size(226, 21);
            this.cbx_changwat.TabIndex = 4;
            this.cbx_changwat.SelectedValueChanged += new System.EventHandler(this.cbx_changwat_SelectedValueChanged);
            // 
            // btn_PopulationAll
            // 
            this.btn_PopulationAll.Enabled = false;
            this.btn_PopulationAll.Location = new System.Drawing.Point(143, 41);
            this.btn_PopulationAll.Name = "btn_PopulationAll";
            this.btn_PopulationAll.Size = new System.Drawing.Size(98, 23);
            this.btn_PopulationAll.TabIndex = 5;
            this.btn_PopulationAll.Text = "All Years";
            this.btn_PopulationAll.UseVisualStyleBackColor = true;
            this.btn_PopulationAll.Click += new System.EventHandler(this.btnPopulationDownloadAll_click);
            // 
            // btn_GazetteLoad
            // 
            this.btn_GazetteLoad.Location = new System.Drawing.Point(25, 12);
            this.btn_GazetteLoad.Name = "btn_GazetteLoad";
            this.btn_GazetteLoad.Size = new System.Drawing.Size(112, 23);
            this.btn_GazetteLoad.TabIndex = 6;
            this.btn_GazetteLoad.Text = "Load gazette";
            this.btn_GazetteLoad.UseVisualStyleBackColor = true;
            this.btn_GazetteLoad.Click += new System.EventHandler(this.btn_GazetteLoad_Click);
            // 
            // btn_GazetteShow
            // 
            this.btn_GazetteShow.Enabled = false;
            this.btn_GazetteShow.Location = new System.Drawing.Point(331, 12);
            this.btn_GazetteShow.Name = "btn_GazetteShow";
            this.btn_GazetteShow.Size = new System.Drawing.Size(79, 23);
            this.btn_GazetteShow.TabIndex = 7;
            this.btn_GazetteShow.Text = "Filtered";
            this.btn_GazetteShow.UseVisualStyleBackColor = true;
            this.btn_GazetteShow.Click += new System.EventHandler(this.btn_GazetteShow_Click);
            // 
            // btn_CheckForNews
            // 
            this.btn_CheckForNews.Location = new System.Drawing.Point(25, 108);
            this.btn_CheckForNews.Name = "btn_CheckForNews";
            this.btn_CheckForNews.Size = new System.Drawing.Size(112, 23);
            this.btn_CheckForNews.TabIndex = 8;
            this.btn_CheckForNews.Text = "Check for News";
            this.btn_CheckForNews.UseVisualStyleBackColor = true;
            this.btn_CheckForNews.Click += new System.EventHandler(this.btn_CheckForNews_Click);
            // 
            // btn_GazetteShowAll
            // 
            this.btn_GazetteShowAll.Enabled = false;
            this.btn_GazetteShowAll.Location = new System.Drawing.Point(247, 12);
            this.btn_GazetteShowAll.Name = "btn_GazetteShowAll";
            this.btn_GazetteShowAll.Size = new System.Drawing.Size(75, 23);
            this.btn_GazetteShowAll.TabIndex = 9;
            this.btn_GazetteShowAll.Text = "Show All";
            this.btn_GazetteShowAll.UseVisualStyleBackColor = true;
            this.btn_GazetteShowAll.Click += new System.EventHandler(this.btn_GazetteShowAll_Click);
            // 
            // btn_GazetteSearchYear
            // 
            this.btn_GazetteSearchYear.Location = new System.Drawing.Point(143, 108);
            this.btn_GazetteSearchYear.Name = "btn_GazetteSearchYear";
            this.btn_GazetteSearchYear.Size = new System.Drawing.Size(98, 23);
            this.btn_GazetteSearchYear.TabIndex = 10;
            this.btn_GazetteSearchYear.Text = "News from year";
            this.btn_GazetteSearchYear.UseVisualStyleBackColor = true;
            this.btn_GazetteSearchYear.Click += new System.EventHandler(this.btn_GazetteSearchYear_Click);
            // 
            // btn_LoadCcaatt
            // 
            this.btn_LoadCcaatt.Location = new System.Drawing.Point(25, 163);
            this.btn_LoadCcaatt.Name = "btn_LoadCcaatt";
            this.btn_LoadCcaatt.Size = new System.Drawing.Size(75, 23);
            this.btn_LoadCcaatt.TabIndex = 11;
            this.btn_LoadCcaatt.Text = "Load ccaatt";
            this.btn_LoadCcaatt.UseVisualStyleBackColor = true;
            this.btn_LoadCcaatt.Click += new System.EventHandler(this.btn_LoadCcaatt_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_DownloadCcaatt
            // 
            this.btn_DownloadCcaatt.Location = new System.Drawing.Point(106, 163);
            this.btn_DownloadCcaatt.Name = "btn_DownloadCcaatt";
            this.btn_DownloadCcaatt.Size = new System.Drawing.Size(75, 23);
            this.btn_DownloadCcaatt.TabIndex = 12;
            this.btn_DownloadCcaatt.Text = "ccaatt";
            this.btn_DownloadCcaatt.UseVisualStyleBackColor = true;
            this.btn_DownloadCcaatt.Click += new System.EventHandler(this.btn_DownloadCcaatt_Click);
            // 
            // btn_GazetteNewsSince1970
            // 
            this.btn_GazetteNewsSince1970.Location = new System.Drawing.Point(247, 108);
            this.btn_GazetteNewsSince1970.Name = "btn_GazetteNewsSince1970";
            this.btn_GazetteNewsSince1970.Size = new System.Drawing.Size(79, 23);
            this.btn_GazetteNewsSince1970.TabIndex = 13;
            this.btn_GazetteNewsSince1970.Text = "Search";
            this.btn_GazetteNewsSince1970.UseVisualStyleBackColor = true;
            this.btn_GazetteNewsSince1970.Click += new System.EventHandler(this.btn_GazetteSearch_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(256, 214);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "amphoe.com";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ButtonThesaban
            // 
            this.ButtonThesaban.Location = new System.Drawing.Point(175, 214);
            this.ButtonThesaban.Name = "ButtonThesaban";
            this.ButtonThesaban.Size = new System.Drawing.Size(75, 23);
            this.ButtonThesaban.TabIndex = 15;
            this.ButtonThesaban.Text = "Thesaban";
            this.ButtonThesaban.UseVisualStyleBackColor = true;
            this.ButtonThesaban.Click += new System.EventHandler(this.ButtonThesaban_Click);
            // 
            // btnSetup
            // 
            this.btnSetup.Location = new System.Drawing.Point(25, 214);
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size(75, 23);
            this.btnSetup.TabIndex = 16;
            this.btnSetup.Text = "Setup";
            this.btnSetup.UseVisualStyleBackColor = true;
            this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
            // 
            // btnNumerals
            // 
            this.btnNumerals.Location = new System.Drawing.Point(25, 259);
            this.btnNumerals.Name = "btnNumerals";
            this.btnNumerals.Size = new System.Drawing.Size(75, 23);
            this.btnNumerals.TabIndex = 17;
            this.btnNumerals.Text = "Numerals";
            this.btnNumerals.UseVisualStyleBackColor = true;
            this.btnNumerals.Click += new System.EventHandler(this.btnNumerals_Click);
            // 
            // btnTambonFrequency
            // 
            this.btnTambonFrequency.Location = new System.Drawing.Point(261, 258);
            this.btnTambonFrequency.Name = "btnTambonFrequency";
            this.btnTambonFrequency.Size = new System.Drawing.Size(99, 23);
            this.btnTambonFrequency.TabIndex = 18;
            this.btnTambonFrequency.Text = "TambonNames";
            this.btnTambonFrequency.UseVisualStyleBackColor = true;
            this.btnTambonFrequency.Click += new System.EventHandler(this.btnTambonFrequency_Click);
            // 
            // btnTambonCreation
            // 
            this.btnTambonCreation.Location = new System.Drawing.Point(261, 287);
            this.btnTambonCreation.Name = "btnTambonCreation";
            this.btnTambonCreation.Size = new System.Drawing.Size(97, 23);
            this.btnTambonCreation.TabIndex = 19;
            this.btnTambonCreation.Text = "TambonCreation";
            this.btnTambonCreation.UseVisualStyleBackColor = true;
            this.btnTambonCreation.Click += new System.EventHandler(this.btnTambonCreation_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 503);
            this.Controls.Add(this.btnTambonCreation);
            this.Controls.Add(this.btnTambonFrequency);
            this.Controls.Add(this.btnNumerals);
            this.Controls.Add(this.btnSetup);
            this.Controls.Add(this.ButtonThesaban);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_GazetteNewsSince1970);
            this.Controls.Add(this.btn_DownloadCcaatt);
            this.Controls.Add(this.btn_LoadCcaatt);
            this.Controls.Add(this.btn_GazetteSearchYear);
            this.Controls.Add(this.btn_GazetteShowAll);
            this.Controls.Add(this.btn_CheckForNews);
            this.Controls.Add(this.btn_GazetteShow);
            this.Controls.Add(this.btn_GazetteLoad);
            this.Controls.Add(this.btn_PopulationAll);
            this.Controls.Add(this.cbx_changwat);
            this.Controls.Add(this.edt_year);
            this.Controls.Add(this.btn_Population);
            this.Controls.Add(this.btn_GazetteMirror);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edt_year)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_GazetteMirror;
        private System.Windows.Forms.Button btn_Population;
        private System.Windows.Forms.NumericUpDown edt_year;
        private System.Windows.Forms.ComboBox cbx_changwat;
        private System.Windows.Forms.Button btn_PopulationAll;
        private System.Windows.Forms.Button btn_GazetteLoad;
        private System.Windows.Forms.Button btn_GazetteShow;
        private System.Windows.Forms.Button btn_CheckForNews;
        private System.Windows.Forms.Button btn_GazetteShowAll;
        private System.Windows.Forms.Button btn_GazetteSearchYear;
        private System.Windows.Forms.Button btn_LoadCcaatt;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_DownloadCcaatt;
        private System.Windows.Forms.Button btn_GazetteNewsSince1970;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button ButtonThesaban;
        private System.Windows.Forms.Button btnSetup;
        private System.Windows.Forms.Button btnNumerals;
        private System.Windows.Forms.Button btnTambonFrequency;
        private System.Windows.Forms.Button btnTambonCreation;
    }
}

