namespace De.AHoerstemeier.Tambon
{
    partial class PopulationDataView
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.mTreeviewData = new System.Windows.Forms.TreeView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.mListviewData = new System.Windows.Forms.ListView();
            this.columnEnglish = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnThai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGeocode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTotal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnThesaban = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSaveXML = new System.Windows.Forms.Button();
            this.btnClipboardAmphoe = new System.Windows.Forms.Button();
            this.btnGazette = new System.Windows.Forms.Button();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(3, 5);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.mTreeviewData);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.textBox1);
            this.splitContainer.Panel2.Controls.Add(this.mListviewData);
            this.splitContainer.Size = new System.Drawing.Size(471, 444);
            this.splitContainer.SplitterDistance = 157;
            this.splitContainer.TabIndex = 9;
            // 
            // mTreeviewData
            // 
            this.mTreeviewData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mTreeviewData.Location = new System.Drawing.Point(0, 0);
            this.mTreeviewData.Name = "mTreeviewData";
            this.mTreeviewData.Size = new System.Drawing.Size(154, 444);
            this.mTreeviewData.TabIndex = 8;
            this.mTreeviewData.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_data_AfterSelect);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 311);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(310, 133);
            this.textBox1.TabIndex = 10;
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
            this.columnTotal,
            this.columnThesaban});
            this.mListviewData.Location = new System.Drawing.Point(-1, 0);
            this.mListviewData.Name = "mListviewData";
            this.mListviewData.Size = new System.Drawing.Size(311, 305);
            this.mListviewData.TabIndex = 9;
            this.mListviewData.UseCompatibleStateImageBehavior = false;
            this.mListviewData.View = System.Windows.Forms.View.Details;
            // 
            // columnEnglish
            // 
            this.columnEnglish.Text = "Name";
            // 
            // columnThai
            // 
            this.columnThai.Text = "Thai";
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
            // columnThesaban
            // 
            this.columnThesaban.Text = "Thesaban";
            // 
            // btnSaveXML
            // 
            this.btnSaveXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveXML.Location = new System.Drawing.Point(3, 458);
            this.btnSaveXML.Name = "btnSaveXML";
            this.btnSaveXML.Size = new System.Drawing.Size(75, 23);
            this.btnSaveXML.TabIndex = 10;
            this.btnSaveXML.Text = "Save XML";
            this.btnSaveXML.UseVisualStyleBackColor = true;
            this.btnSaveXML.Click += new System.EventHandler(this.btnSaveXML_Click);
            // 
            // btnClipboardAmphoe
            // 
            this.btnClipboardAmphoe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClipboardAmphoe.Enabled = false;
            this.btnClipboardAmphoe.Location = new System.Drawing.Point(163, 458);
            this.btnClipboardAmphoe.Name = "btnClipboardAmphoe";
            this.btnClipboardAmphoe.Size = new System.Drawing.Size(97, 23);
            this.btnClipboardAmphoe.TabIndex = 11;
            this.btnClipboardAmphoe.Text = "Export WikiTable";
            this.btnClipboardAmphoe.UseVisualStyleBackColor = true;
            this.btnClipboardAmphoe.Click += new System.EventHandler(this.btnClipboardAmphoe_Click);
            // 
            // btnGazette
            // 
            this.btnGazette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGazette.Location = new System.Drawing.Point(84, 458);
            this.btnGazette.Name = "btnGazette";
            this.btnGazette.Size = new System.Drawing.Size(75, 23);
            this.btnGazette.TabIndex = 12;
            this.btnGazette.Text = "Gazette";
            this.btnGazette.UseVisualStyleBackColor = true;
            this.btnGazette.Click += new System.EventHandler(this.btnGazette_Click);
            // 
            // PopulationDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 493);
            this.Controls.Add(this.btnGazette);
            this.Controls.Add(this.btnClipboardAmphoe);
            this.Controls.Add(this.btnSaveXML);
            this.Controls.Add(this.splitContainer);
            this.Name = "PopulationDataView";
            this.Text = "PopulationDataView";
            this.Activated += new System.EventHandler(this.PopulationDataView_Enter);
            this.Load += new System.EventHandler(this.PopulationDataView_Enter);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView mTreeviewData;
        private System.Windows.Forms.ListView mListviewData;
        private System.Windows.Forms.ColumnHeader columnEnglish;
        private System.Windows.Forms.ColumnHeader columnThai;
        private System.Windows.Forms.ColumnHeader columnGeocode;
        private System.Windows.Forms.ColumnHeader columnTotal;
        private System.Windows.Forms.Button btnSaveXML;
        private System.Windows.Forms.Button btnClipboardAmphoe;
        private System.Windows.Forms.ColumnHeader columnThesaban;
        private System.Windows.Forms.Button btnGazette;
        private System.Windows.Forms.TextBox textBox1;
    }
}