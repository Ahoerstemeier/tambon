namespace De.AHoerstemeier.Tambon
{
    partial class ConstituencyStatisticsViewer
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.TrvData = new System.Windows.Forms.TreeView();
            this.txtStatistics = new System.Windows.Forms.TextBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrvData
            // 
            this.TrvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrvData.Location = new System.Drawing.Point(3, 3);
            this.TrvData.Name = "TrvData";
            this.TrvData.Size = new System.Drawing.Size(161, 340);
            this.TrvData.TabIndex = 9;
            this.TrvData.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TrvData_AfterSelect);
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
            this.txtStatistics.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatistics.Size = new System.Drawing.Size(324, 340);
            this.txtStatistics.TabIndex = 10;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(1, 2);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.TrvData);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.txtStatistics);
            this.splitContainer.Size = new System.Drawing.Size(501, 346);
            this.splitContainer.SplitterDistance = 167;
            this.splitContainer.TabIndex = 11;
            // 
            // ConstituencyStatisticsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 347);
            this.Controls.Add(this.splitContainer);
            this.Name = "ConstituencyStatisticsViewer";
            this.Text = "ConstituencyStatisticsViewer";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView TrvData;
        private System.Windows.Forms.TextBox txtStatistics;
        private System.Windows.Forms.SplitContainer splitContainer;
    }
}