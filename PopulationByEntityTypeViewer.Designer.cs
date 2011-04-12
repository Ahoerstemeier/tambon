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
            this.rbx_AmphoeKhet = new System.Windows.Forms.RadioButton();
            this.rbx_TambonKhwaeng = new System.Windows.Forms.RadioButton();
            this.rbx_Thesaban = new System.Windows.Forms.RadioButton();
            this.chk_Amphoe = new System.Windows.Forms.CheckBox();
            this.chk_Khet = new System.Windows.Forms.CheckBox();
            this.chk_Tambon = new System.Windows.Forms.CheckBox();
            this.chk_Khwaeng = new System.Windows.Forms.CheckBox();
            this.chk_ThesabanNakhon = new System.Windows.Forms.CheckBox();
            this.chk_ThesabanMueang = new System.Windows.Forms.CheckBox();
            this.chk_ThesabanTambon = new System.Windows.Forms.CheckBox();
            this.btnExportCSV = new System.Windows.Forms.Button();
            this.grpData = new System.Windows.Forms.SplitContainer();
            this.mListviewData = new System.Windows.Forms.ListView();
            this.columnEnglish = new System.Windows.Forms.ColumnHeader();
            this.columnThai = new System.Windows.Forms.ColumnHeader();
            this.columnGeocode = new System.Windows.Forms.ColumnHeader();
            this.columnTotal = new System.Windows.Forms.ColumnHeader();
            this.txtStatistics = new System.Windows.Forms.TextBox();
            this.grpData.Panel1.SuspendLayout();
            this.grpData.Panel2.SuspendLayout();
            this.grpData.SuspendLayout();
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
            this.rbx_Changwat.CheckedChanged += new System.EventHandler(this.rbx_Entity_CheckedChanged);
            // 
            // rbx_AmphoeKhet
            // 
            this.rbx_AmphoeKhet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbx_AmphoeKhet.AutoSize = true;
            this.rbx_AmphoeKhet.Location = new System.Drawing.Point(12, 318);
            this.rbx_AmphoeKhet.Name = "rbx_AmphoeKhet";
            this.rbx_AmphoeKhet.Size = new System.Drawing.Size(57, 17);
            this.rbx_AmphoeKhet.TabIndex = 12;
            this.rbx_AmphoeKhet.Text = "District";
            this.rbx_AmphoeKhet.UseVisualStyleBackColor = true;
            this.rbx_AmphoeKhet.CheckedChanged += new System.EventHandler(this.rbx_Entity_CheckedChanged);
            // 
            // rbx_TambonKhwaeng
            // 
            this.rbx_TambonKhwaeng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbx_TambonKhwaeng.AutoSize = true;
            this.rbx_TambonKhwaeng.Location = new System.Drawing.Point(12, 341);
            this.rbx_TambonKhwaeng.Name = "rbx_TambonKhwaeng";
            this.rbx_TambonKhwaeng.Size = new System.Drawing.Size(74, 17);
            this.rbx_TambonKhwaeng.TabIndex = 13;
            this.rbx_TambonKhwaeng.Text = "Subdistrict";
            this.rbx_TambonKhwaeng.UseVisualStyleBackColor = true;
            this.rbx_TambonKhwaeng.CheckedChanged += new System.EventHandler(this.rbx_Entity_CheckedChanged);
            // 
            // rbx_Thesaban
            // 
            this.rbx_Thesaban.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbx_Thesaban.AutoSize = true;
            this.rbx_Thesaban.Location = new System.Drawing.Point(12, 364);
            this.rbx_Thesaban.Name = "rbx_Thesaban";
            this.rbx_Thesaban.Size = new System.Drawing.Size(80, 17);
            this.rbx_Thesaban.TabIndex = 14;
            this.rbx_Thesaban.Text = "Municipality";
            this.rbx_Thesaban.UseVisualStyleBackColor = true;
            this.rbx_Thesaban.CheckedChanged += new System.EventHandler(this.rbx_Entity_CheckedChanged);
            // 
            // chk_Amphoe
            // 
            this.chk_Amphoe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_Amphoe.AutoSize = true;
            this.chk_Amphoe.Checked = true;
            this.chk_Amphoe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Amphoe.Location = new System.Drawing.Point(127, 319);
            this.chk_Amphoe.Name = "chk_Amphoe";
            this.chk_Amphoe.Size = new System.Drawing.Size(65, 17);
            this.chk_Amphoe.TabIndex = 15;
            this.chk_Amphoe.Text = "Amphoe";
            this.chk_Amphoe.UseVisualStyleBackColor = true;
            this.chk_Amphoe.CheckStateChanged += new System.EventHandler(this.chk_Entity_CheckStateChanged);
            // 
            // chk_Khet
            // 
            this.chk_Khet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_Khet.AutoSize = true;
            this.chk_Khet.Checked = true;
            this.chk_Khet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Khet.Location = new System.Drawing.Point(238, 319);
            this.chk_Khet.Name = "chk_Khet";
            this.chk_Khet.Size = new System.Drawing.Size(48, 17);
            this.chk_Khet.TabIndex = 16;
            this.chk_Khet.Text = "Khet";
            this.chk_Khet.UseVisualStyleBackColor = true;
            this.chk_Khet.CheckStateChanged += new System.EventHandler(this.chk_Entity_CheckStateChanged);
            // 
            // chk_Tambon
            // 
            this.chk_Tambon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_Tambon.AutoSize = true;
            this.chk_Tambon.Checked = true;
            this.chk_Tambon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Tambon.Location = new System.Drawing.Point(127, 342);
            this.chk_Tambon.Name = "chk_Tambon";
            this.chk_Tambon.Size = new System.Drawing.Size(65, 17);
            this.chk_Tambon.TabIndex = 17;
            this.chk_Tambon.Text = "Tambon";
            this.chk_Tambon.UseVisualStyleBackColor = true;
            this.chk_Tambon.CheckStateChanged += new System.EventHandler(this.chk_Entity_CheckStateChanged);
            // 
            // chk_Khwaeng
            // 
            this.chk_Khwaeng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_Khwaeng.AutoSize = true;
            this.chk_Khwaeng.Checked = true;
            this.chk_Khwaeng.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Khwaeng.Location = new System.Drawing.Point(238, 342);
            this.chk_Khwaeng.Name = "chk_Khwaeng";
            this.chk_Khwaeng.Size = new System.Drawing.Size(71, 17);
            this.chk_Khwaeng.TabIndex = 18;
            this.chk_Khwaeng.Text = "Khwaeng";
            this.chk_Khwaeng.UseVisualStyleBackColor = true;
            this.chk_Khwaeng.CheckStateChanged += new System.EventHandler(this.chk_Entity_CheckStateChanged);
            // 
            // chk_ThesabanNakhon
            // 
            this.chk_ThesabanNakhon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_ThesabanNakhon.AutoSize = true;
            this.chk_ThesabanNakhon.Checked = true;
            this.chk_ThesabanNakhon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_ThesabanNakhon.Location = new System.Drawing.Point(127, 364);
            this.chk_ThesabanNakhon.Name = "chk_ThesabanNakhon";
            this.chk_ThesabanNakhon.Size = new System.Drawing.Size(43, 17);
            this.chk_ThesabanNakhon.TabIndex = 19;
            this.chk_ThesabanNakhon.Text = "City";
            this.chk_ThesabanNakhon.UseVisualStyleBackColor = true;
            this.chk_ThesabanNakhon.CheckStateChanged += new System.EventHandler(this.chk_Entity_CheckStateChanged);
            // 
            // chk_ThesabanMueang
            // 
            this.chk_ThesabanMueang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_ThesabanMueang.AutoSize = true;
            this.chk_ThesabanMueang.Checked = true;
            this.chk_ThesabanMueang.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_ThesabanMueang.Location = new System.Drawing.Point(238, 365);
            this.chk_ThesabanMueang.Name = "chk_ThesabanMueang";
            this.chk_ThesabanMueang.Size = new System.Drawing.Size(53, 17);
            this.chk_ThesabanMueang.TabIndex = 20;
            this.chk_ThesabanMueang.Text = "Town";
            this.chk_ThesabanMueang.UseVisualStyleBackColor = true;
            this.chk_ThesabanMueang.CheckStateChanged += new System.EventHandler(this.chk_Entity_CheckStateChanged);
            // 
            // chk_ThesabanTambon
            // 
            this.chk_ThesabanTambon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_ThesabanTambon.AutoSize = true;
            this.chk_ThesabanTambon.Checked = true;
            this.chk_ThesabanTambon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_ThesabanTambon.Location = new System.Drawing.Point(341, 365);
            this.chk_ThesabanTambon.Name = "chk_ThesabanTambon";
            this.chk_ThesabanTambon.Size = new System.Drawing.Size(132, 17);
            this.chk_ThesabanTambon.TabIndex = 21;
            this.chk_ThesabanTambon.Text = "Subdistrict municipality";
            this.chk_ThesabanTambon.UseVisualStyleBackColor = true;
            this.chk_ThesabanTambon.CheckStateChanged += new System.EventHandler(this.chk_Entity_CheckStateChanged);
            // 
            // btnExportCSV
            // 
            this.btnExportCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportCSV.Location = new System.Drawing.Point(583, 361);
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
            this.grpData.Panel1.Controls.Add(this.mListviewData);
            this.grpData.Panel1MinSize = 125;
            // 
            // grpData.Panel2
            // 
            this.grpData.Panel2.Controls.Add(this.txtStatistics);
            this.grpData.Panel2MinSize = 125;
            this.grpData.Size = new System.Drawing.Size(646, 277);
            this.grpData.SplitterDistance = 350;
            this.grpData.TabIndex = 23;
            // 
            // mListviewData
            // 
            this.mListviewData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mListviewData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEnglish,
            this.columnThai,
            this.columnGeocode,
            this.columnTotal});
            this.mListviewData.Location = new System.Drawing.Point(3, 3);
            this.mListviewData.Name = "mListviewData";
            this.mListviewData.Size = new System.Drawing.Size(344, 271);
            this.mListviewData.TabIndex = 11;
            this.mListviewData.UseCompatibleStateImageBehavior = false;
            this.mListviewData.View = System.Windows.Forms.View.Details;
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
            // txtStatistics
            // 
            this.txtStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatistics.Location = new System.Drawing.Point(3, 3);
            this.txtStatistics.Multiline = true;
            this.txtStatistics.Name = "txtStatistics";
            this.txtStatistics.ReadOnly = true;
            this.txtStatistics.Size = new System.Drawing.Size(286, 271);
            this.txtStatistics.TabIndex = 0;
            // 
            // PopulationByEntityTypeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 391);
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.btnExportCSV);
            this.Controls.Add(this.chk_ThesabanTambon);
            this.Controls.Add(this.chk_ThesabanMueang);
            this.Controls.Add(this.chk_ThesabanNakhon);
            this.Controls.Add(this.chk_Khwaeng);
            this.Controls.Add(this.chk_Tambon);
            this.Controls.Add(this.chk_Khet);
            this.Controls.Add(this.chk_Amphoe);
            this.Controls.Add(this.rbx_Thesaban);
            this.Controls.Add(this.rbx_TambonKhwaeng);
            this.Controls.Add(this.rbx_AmphoeKhet);
            this.Controls.Add(this.rbx_Changwat);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "PopulationByEntityTypeViewer";
            this.Text = "PopulationByEntityTypeViewer";
            this.grpData.Panel1.ResumeLayout(false);
            this.grpData.Panel2.ResumeLayout(false);
            this.grpData.Panel2.PerformLayout();
            this.grpData.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbx_Changwat;
        private System.Windows.Forms.RadioButton rbx_AmphoeKhet;
        private System.Windows.Forms.RadioButton rbx_TambonKhwaeng;
        private System.Windows.Forms.RadioButton rbx_Thesaban;
        private System.Windows.Forms.CheckBox chk_Amphoe;
        private System.Windows.Forms.CheckBox chk_Khet;
        private System.Windows.Forms.CheckBox chk_Tambon;
        private System.Windows.Forms.CheckBox chk_Khwaeng;
        private System.Windows.Forms.CheckBox chk_ThesabanNakhon;
        private System.Windows.Forms.CheckBox chk_ThesabanMueang;
        private System.Windows.Forms.CheckBox chk_ThesabanTambon;
        private System.Windows.Forms.Button btnExportCSV;
        private System.Windows.Forms.SplitContainer grpData;
        private System.Windows.Forms.ListView mListviewData;
        private System.Windows.Forms.ColumnHeader columnEnglish;
        private System.Windows.Forms.ColumnHeader columnThai;
        private System.Windows.Forms.ColumnHeader columnGeocode;
        private System.Windows.Forms.ColumnHeader columnTotal;
        private System.Windows.Forms.TextBox txtStatistics;
    }
}