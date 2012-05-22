namespace De.AHoerstemeier.Tambon
{
    partial class PopulationByEntityTypeViewer
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
            this.rbx_Changwat = new System.Windows.Forms.RadioButton();
            this.rbxAmphoeKhet = new System.Windows.Forms.RadioButton();
            this.rbxTambonKhwaeng = new System.Windows.Forms.RadioButton();
            this.rbxThesaban = new System.Windows.Forms.RadioButton();
            this.chkAmphoe = new System.Windows.Forms.CheckBox();
            this.chkKhet = new System.Windows.Forms.CheckBox();
            this.chkTambon = new System.Windows.Forms.CheckBox();
            this.chkKhwaeng = new System.Windows.Forms.CheckBox();
            this.chkThesabanNakhon = new System.Windows.Forms.CheckBox();
            this.chkThesabanMueang = new System.Windows.Forms.CheckBox();
            this.chkThesabanTambon = new System.Windows.Forms.CheckBox();
            this.btnExportCSV = new System.Windows.Forms.Button();
            this.grpData = new System.Windows.Forms.SplitContainer();
            this.lvData = new System.Windows.Forms.ListView();
            this.columnEnglish = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnThai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGeocode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTotal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnChange = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnChangePercent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtStatistics = new System.Windows.Forms.TextBox();
            this.chkCompare = new System.Windows.Forms.CheckBox();
            this.edtCompareYear = new System.Windows.Forms.NumericUpDown();
            this.btnCsvAllYears = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.Panel1.SuspendLayout();
            this.grpData.Panel2.SuspendLayout();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edtCompareYear)).BeginInit();
            this.SuspendLayout();
            // 
            // rbx_Changwat
            // 
            this.rbx_Changwat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbx_Changwat.AutoSize = true;
            this.rbx_Changwat.Checked = true;
            this.rbx_Changwat.Location = new System.Drawing.Point(12, 295);
            this.rbx_Changwat.Name = "rbx_Changwat";
            this.rbx_Changwat.Size = new System.Drawing.Size(67, 17);
            this.rbx_Changwat.TabIndex = 11;
            this.rbx_Changwat.TabStop = true;
            this.rbx_Changwat.Text = "Province";
            this.rbx_Changwat.UseVisualStyleBackColor = true;
            this.rbx_Changwat.CheckedChanged += new System.EventHandler(this.rbxEntity_CheckedChanged);
            // 
            // rbxAmphoeKhet
            // 
            this.rbxAmphoeKhet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbxAmphoeKhet.AutoSize = true;
            this.rbxAmphoeKhet.Location = new System.Drawing.Point(12, 318);
            this.rbxAmphoeKhet.Name = "rbxAmphoeKhet";
            this.rbxAmphoeKhet.Size = new System.Drawing.Size(57, 17);
            this.rbxAmphoeKhet.TabIndex = 12;
            this.rbxAmphoeKhet.Text = "District";
            this.rbxAmphoeKhet.UseVisualStyleBackColor = true;
            this.rbxAmphoeKhet.CheckedChanged += new System.EventHandler(this.rbxEntity_CheckedChanged);
            // 
            // rbxTambonKhwaeng
            // 
            this.rbxTambonKhwaeng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbxTambonKhwaeng.AutoSize = true;
            this.rbxTambonKhwaeng.Location = new System.Drawing.Point(12, 341);
            this.rbxTambonKhwaeng.Name = "rbxTambonKhwaeng";
            this.rbxTambonKhwaeng.Size = new System.Drawing.Size(74, 17);
            this.rbxTambonKhwaeng.TabIndex = 13;
            this.rbxTambonKhwaeng.Text = "Subdistrict";
            this.rbxTambonKhwaeng.UseVisualStyleBackColor = true;
            this.rbxTambonKhwaeng.CheckedChanged += new System.EventHandler(this.rbxEntity_CheckedChanged);
            // 
            // rbxThesaban
            // 
            this.rbxThesaban.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbxThesaban.AutoSize = true;
            this.rbxThesaban.Location = new System.Drawing.Point(12, 364);
            this.rbxThesaban.Name = "rbxThesaban";
            this.rbxThesaban.Size = new System.Drawing.Size(80, 17);
            this.rbxThesaban.TabIndex = 14;
            this.rbxThesaban.Text = "Municipality";
            this.rbxThesaban.UseVisualStyleBackColor = true;
            this.rbxThesaban.CheckedChanged += new System.EventHandler(this.rbxEntity_CheckedChanged);
            // 
            // chkAmphoe
            // 
            this.chkAmphoe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAmphoe.AutoSize = true;
            this.chkAmphoe.Checked = true;
            this.chkAmphoe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAmphoe.Location = new System.Drawing.Point(127, 319);
            this.chkAmphoe.Name = "chkAmphoe";
            this.chkAmphoe.Size = new System.Drawing.Size(65, 17);
            this.chkAmphoe.TabIndex = 15;
            this.chkAmphoe.Text = "Amphoe";
            this.chkAmphoe.UseVisualStyleBackColor = true;
            this.chkAmphoe.CheckStateChanged += new System.EventHandler(this.chkEntity_CheckStateChanged);
            // 
            // chkKhet
            // 
            this.chkKhet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkKhet.AutoSize = true;
            this.chkKhet.Checked = true;
            this.chkKhet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKhet.Location = new System.Drawing.Point(238, 319);
            this.chkKhet.Name = "chkKhet";
            this.chkKhet.Size = new System.Drawing.Size(48, 17);
            this.chkKhet.TabIndex = 16;
            this.chkKhet.Text = "Khet";
            this.chkKhet.UseVisualStyleBackColor = true;
            this.chkKhet.CheckStateChanged += new System.EventHandler(this.chkEntity_CheckStateChanged);
            // 
            // chkTambon
            // 
            this.chkTambon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkTambon.AutoSize = true;
            this.chkTambon.Checked = true;
            this.chkTambon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTambon.Location = new System.Drawing.Point(127, 342);
            this.chkTambon.Name = "chkTambon";
            this.chkTambon.Size = new System.Drawing.Size(65, 17);
            this.chkTambon.TabIndex = 17;
            this.chkTambon.Text = "Tambon";
            this.chkTambon.UseVisualStyleBackColor = true;
            this.chkTambon.CheckStateChanged += new System.EventHandler(this.chkEntity_CheckStateChanged);
            // 
            // chkKhwaeng
            // 
            this.chkKhwaeng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkKhwaeng.AutoSize = true;
            this.chkKhwaeng.Checked = true;
            this.chkKhwaeng.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKhwaeng.Location = new System.Drawing.Point(238, 342);
            this.chkKhwaeng.Name = "chkKhwaeng";
            this.chkKhwaeng.Size = new System.Drawing.Size(71, 17);
            this.chkKhwaeng.TabIndex = 18;
            this.chkKhwaeng.Text = "Khwaeng";
            this.chkKhwaeng.UseVisualStyleBackColor = true;
            this.chkKhwaeng.CheckStateChanged += new System.EventHandler(this.chkEntity_CheckStateChanged);
            // 
            // chkThesabanNakhon
            // 
            this.chkThesabanNakhon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkThesabanNakhon.AutoSize = true;
            this.chkThesabanNakhon.Checked = true;
            this.chkThesabanNakhon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkThesabanNakhon.Location = new System.Drawing.Point(127, 364);
            this.chkThesabanNakhon.Name = "chkThesabanNakhon";
            this.chkThesabanNakhon.Size = new System.Drawing.Size(43, 17);
            this.chkThesabanNakhon.TabIndex = 19;
            this.chkThesabanNakhon.Text = "City";
            this.chkThesabanNakhon.UseVisualStyleBackColor = true;
            this.chkThesabanNakhon.CheckStateChanged += new System.EventHandler(this.chkEntity_CheckStateChanged);
            // 
            // chkThesabanMueang
            // 
            this.chkThesabanMueang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkThesabanMueang.AutoSize = true;
            this.chkThesabanMueang.Checked = true;
            this.chkThesabanMueang.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkThesabanMueang.Location = new System.Drawing.Point(238, 365);
            this.chkThesabanMueang.Name = "chkThesabanMueang";
            this.chkThesabanMueang.Size = new System.Drawing.Size(53, 17);
            this.chkThesabanMueang.TabIndex = 20;
            this.chkThesabanMueang.Text = "Town";
            this.chkThesabanMueang.UseVisualStyleBackColor = true;
            this.chkThesabanMueang.CheckStateChanged += new System.EventHandler(this.chkEntity_CheckStateChanged);
            // 
            // chkThesabanTambon
            // 
            this.chkThesabanTambon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkThesabanTambon.AutoSize = true;
            this.chkThesabanTambon.Checked = true;
            this.chkThesabanTambon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkThesabanTambon.Location = new System.Drawing.Point(341, 365);
            this.chkThesabanTambon.Name = "chkThesabanTambon";
            this.chkThesabanTambon.Size = new System.Drawing.Size(132, 17);
            this.chkThesabanTambon.TabIndex = 21;
            this.chkThesabanTambon.Text = "Subdistrict municipality";
            this.chkThesabanTambon.UseVisualStyleBackColor = true;
            this.chkThesabanTambon.CheckStateChanged += new System.EventHandler(this.chkEntity_CheckStateChanged);
            // 
            // btnExportCSV
            // 
            this.btnExportCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportCSV.Location = new System.Drawing.Point(652, 361);
            this.btnExportCSV.Name = "btnExportCSV";
            this.btnExportCSV.Size = new System.Drawing.Size(75, 23);
            this.btnExportCSV.TabIndex = 22;
            this.btnExportCSV.Text = "Export CSV";
            this.btnExportCSV.UseVisualStyleBackColor = true;
            this.btnExportCSV.Click += new System.EventHandler(this.btnExportCSV_Click);
            // 
            // grpData
            // 
            this.grpData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpData.Location = new System.Drawing.Point(12, 12);
            this.grpData.Name = "grpData";
            // 
            // grpData.Panel1
            // 
            this.grpData.Panel1.Controls.Add(this.lvData);
            this.grpData.Panel1MinSize = 125;
            // 
            // grpData.Panel2
            // 
            this.grpData.Panel2.Controls.Add(this.txtStatistics);
            this.grpData.Panel2MinSize = 125;
            this.grpData.Size = new System.Drawing.Size(715, 277);
            this.grpData.SplitterDistance = 456;
            this.grpData.TabIndex = 23;
            // 
            // lvData
            // 
            this.lvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEnglish,
            this.columnThai,
            this.columnGeocode,
            this.columnTotal,
            this.columnChange,
            this.columnChangePercent});
            this.lvData.Location = new System.Drawing.Point(3, 3);
            this.lvData.Name = "lvData";
            this.lvData.Size = new System.Drawing.Size(450, 271);
            this.lvData.TabIndex = 11;
            this.lvData.UseCompatibleStateImageBehavior = false;
            this.lvData.View = System.Windows.Forms.View.Details;
            // 
            // columnEnglish
            // 
            this.columnEnglish.Text = "Name";
            this.columnEnglish.Width = 107;
            // 
            // columnThai
            // 
            this.columnThai.Text = "Thai";
            this.columnThai.Width = 95;
            // 
            // columnGeocode
            // 
            this.columnGeocode.Text = "Geocode";
            // 
            // columnTotal
            // 
            this.columnTotal.Text = "Total";
            this.columnTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnChange
            // 
            this.columnChange.Text = "Change";
            // 
            // columnChangePercent
            // 
            this.columnChangePercent.Text = "Percentage";
            // 
            // txtStatistics
            // 
            this.txtStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatistics.Location = new System.Drawing.Point(3, 3);
            this.txtStatistics.Multiline = true;
            this.txtStatistics.Name = "txtStatistics";
            this.txtStatistics.ReadOnly = true;
            this.txtStatistics.Size = new System.Drawing.Size(249, 271);
            this.txtStatistics.TabIndex = 0;
            // 
            // chkCompare
            // 
            this.chkCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCompare.AutoSize = true;
            this.chkCompare.Location = new System.Drawing.Point(555, 296);
            this.chkCompare.Name = "chkCompare";
            this.chkCompare.Size = new System.Drawing.Size(68, 17);
            this.chkCompare.TabIndex = 24;
            this.chkCompare.Text = "Compare";
            this.chkCompare.UseVisualStyleBackColor = true;
            this.chkCompare.CheckedChanged += new System.EventHandler(this.chkCompare_CheckedChanged);
            // 
            // edtCompareYear
            // 
            this.edtCompareYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.edtCompareYear.Location = new System.Drawing.Point(652, 295);
            this.edtCompareYear.Name = "edtCompareYear";
            this.edtCompareYear.Size = new System.Drawing.Size(75, 20);
            this.edtCompareYear.TabIndex = 25;
            this.edtCompareYear.ValueChanged += new System.EventHandler(this.edtCompareYear_ValueChanged);
            // 
            // btnCsvAllYears
            // 
            this.btnCsvAllYears.Location = new System.Drawing.Point(555, 361);
            this.btnCsvAllYears.Name = "btnCsvAllYears";
            this.btnCsvAllYears.Size = new System.Drawing.Size(91, 23);
            this.btnCsvAllYears.TabIndex = 26;
            this.btnCsvAllYears.Text = "CSV All Years";
            this.btnCsvAllYears.UseVisualStyleBackColor = true;
            this.btnCsvAllYears.Click += new System.EventHandler(this.btnCsvAllYears_Click);
            // 
            // PopulationByEntityTypeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 391);
            this.Controls.Add(this.btnCsvAllYears);
            this.Controls.Add(this.edtCompareYear);
            this.Controls.Add(this.chkCompare);
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.btnExportCSV);
            this.Controls.Add(this.chkThesabanTambon);
            this.Controls.Add(this.chkThesabanMueang);
            this.Controls.Add(this.chkThesabanNakhon);
            this.Controls.Add(this.chkKhwaeng);
            this.Controls.Add(this.chkTambon);
            this.Controls.Add(this.chkKhet);
            this.Controls.Add(this.chkAmphoe);
            this.Controls.Add(this.rbxThesaban);
            this.Controls.Add(this.rbxTambonKhwaeng);
            this.Controls.Add(this.rbxAmphoeKhet);
            this.Controls.Add(this.rbx_Changwat);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "PopulationByEntityTypeViewer";
            this.Text = "PopulationByEntityTypeViewer";
            this.Load += new System.EventHandler(this.PopulationByEntityTypeViewer_Load);
            this.grpData.Panel1.ResumeLayout(false);
            this.grpData.Panel2.ResumeLayout(false);
            this.grpData.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.edtCompareYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbx_Changwat;
        private System.Windows.Forms.RadioButton rbxAmphoeKhet;
        private System.Windows.Forms.RadioButton rbxTambonKhwaeng;
        private System.Windows.Forms.RadioButton rbxThesaban;
        private System.Windows.Forms.CheckBox chkAmphoe;
        private System.Windows.Forms.CheckBox chkKhet;
        private System.Windows.Forms.CheckBox chkTambon;
        private System.Windows.Forms.CheckBox chkKhwaeng;
        private System.Windows.Forms.CheckBox chkThesabanNakhon;
        private System.Windows.Forms.CheckBox chkThesabanMueang;
        private System.Windows.Forms.CheckBox chkThesabanTambon;
        private System.Windows.Forms.Button btnExportCSV;
        private System.Windows.Forms.SplitContainer grpData;
        private System.Windows.Forms.ListView lvData;
        private System.Windows.Forms.ColumnHeader columnEnglish;
        private System.Windows.Forms.ColumnHeader columnThai;
        private System.Windows.Forms.ColumnHeader columnGeocode;
        private System.Windows.Forms.ColumnHeader columnTotal;
        private System.Windows.Forms.TextBox txtStatistics;
        private System.Windows.Forms.ColumnHeader columnChange;
        private System.Windows.Forms.CheckBox chkCompare;
        private System.Windows.Forms.NumericUpDown edtCompareYear;
        private System.Windows.Forms.ColumnHeader columnChangePercent;
        private System.Windows.Forms.Button btnCsvAllYears;
    }
}