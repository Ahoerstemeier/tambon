namespace De.AHoerstemeier.Tambon
{
    partial class RoyalGazetteViewer
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
            this.components = new System.ComponentModel.Container();
            this.grid = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuShow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMirror = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeletePDF = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCitation = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToOrderColumns = true;
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.Size = new System.Drawing.Size(755, 359);
            this.grid.TabIndex = 0;
            this.grid.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellDoubleClick);
            this.grid.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.grid_CellContextMenuStripNeeded);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShow,
            this.mnuMirror,
            this.mnuDeletePDF,
            this.toolStripMenuItem2,
            this.mnuCitation});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(128, 98);
            // 
            // mnuShow
            // 
            this.mnuShow.Name = "mnuShow";
            this.mnuShow.Size = new System.Drawing.Size(127, 22);
            this.mnuShow.Text = "Show";
            this.mnuShow.Click += new System.EventHandler(this.mnuShow_Click);
            // 
            // mnuMirror
            // 
            this.mnuMirror.Name = "mnuMirror";
            this.mnuMirror.Size = new System.Drawing.Size(127, 22);
            this.mnuMirror.Text = "Mirror";
            this.mnuMirror.Click += new System.EventHandler(this.mnuMirror_Click);
            // 
            // mnuDeletePDF
            // 
            this.mnuDeletePDF.Name = "mnuDeletePDF";
            this.mnuDeletePDF.Size = new System.Drawing.Size(127, 22);
            this.mnuDeletePDF.Text = "Delete PDF";
            this.mnuDeletePDF.Click += new System.EventHandler(this.mnuDeletePDF_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(124, 6);
            // 
            // mnuCitation
            // 
            this.mnuCitation.Name = "mnuCitation";
            this.mnuCitation.Size = new System.Drawing.Size(127, 22);
            this.mnuCitation.Text = "Citation";
            this.mnuCitation.Click += new System.EventHandler(this.mnuCitation_Click);
            // 
            // btn_save
            // 
            this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save.Location = new System.Drawing.Point(676, 365);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "SaveAs";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // RoyalGazetteViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 390);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.btn_save);
            this.Name = "RoyalGazetteViewer";
            this.Text = "RoyalGazetteViewer";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuShow;
        private System.Windows.Forms.ToolStripMenuItem mnuCitation;
        private System.Windows.Forms.ToolStripMenuItem mnuMirror;
        private System.Windows.Forms.ToolStripMenuItem mnuDeletePDF;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.Button btn_save;
    }
}