namespace De.AHoerstemeier.Tambon
{
    partial class Setup
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
            this.lblHTMLCacheDir = new System.Windows.Forms.Label();
            this.edtHTMLCacheDir = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblXMLOutputDir = new System.Windows.Forms.Label();
            this.edtXMLOutputDir = new System.Windows.Forms.TextBox();
            this.lblPDFDir = new System.Windows.Forms.Label();
            this.edtPDFDir = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblHTMLCacheDir
            // 
            this.lblHTMLCacheDir.AutoSize = true;
            this.lblHTMLCacheDir.Location = new System.Drawing.Point(2, 26);
            this.lblHTMLCacheDir.Name = "lblHTMLCacheDir";
            this.lblHTMLCacheDir.Size = new System.Drawing.Size(116, 13);
            this.lblHTMLCacheDir.TabIndex = 0;
            this.lblHTMLCacheDir.Text = "HTML Cache Directory";
            // 
            // edtHTMLCacheDir
            // 
            this.edtHTMLCacheDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edtHTMLCacheDir.Location = new System.Drawing.Point(150, 23);
            this.edtHTMLCacheDir.Name = "edtHTMLCacheDir";
            this.edtHTMLCacheDir.Size = new System.Drawing.Size(285, 20);
            this.edtHTMLCacheDir.TabIndex = 1;
            this.edtHTMLCacheDir.TextChanged += new System.EventHandler(this.edtHTMLCacheDir_TextChanged);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(360, 111);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblXMLOutputDir
            // 
            this.lblXMLOutputDir.AutoSize = true;
            this.lblXMLOutputDir.Location = new System.Drawing.Point(2, 54);
            this.lblXMLOutputDir.Name = "lblXMLOutputDir";
            this.lblXMLOutputDir.Size = new System.Drawing.Size(105, 13);
            this.lblXMLOutputDir.TabIndex = 3;
            this.lblXMLOutputDir.Text = "XML output directory";
            // 
            // edtXMLOutputDir
            // 
            this.edtXMLOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edtXMLOutputDir.Location = new System.Drawing.Point(150, 51);
            this.edtXMLOutputDir.Name = "edtXMLOutputDir";
            this.edtXMLOutputDir.Size = new System.Drawing.Size(285, 20);
            this.edtXMLOutputDir.TabIndex = 4;
            this.edtXMLOutputDir.TextChanged += new System.EventHandler(this.edtXMLOutputDir_TextChanged);
            // 
            // lblPDFDir
            // 
            this.lblPDFDir.AutoSize = true;
            this.lblPDFDir.Location = new System.Drawing.Point(2, 80);
            this.lblPDFDir.Name = "lblPDFDir";
            this.lblPDFDir.Size = new System.Drawing.Size(66, 13);
            this.lblPDFDir.TabIndex = 5;
            this.lblPDFDir.Text = "PDF storage";
            // 
            // edtPDFDir
            // 
            this.edtPDFDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edtPDFDir.Location = new System.Drawing.Point(150, 77);
            this.edtPDFDir.Name = "edtPDFDir";
            this.edtPDFDir.Size = new System.Drawing.Size(285, 20);
            this.edtPDFDir.TabIndex = 6;
            this.edtPDFDir.TextChanged += new System.EventHandler(this.edtPDFDir_TextChanged);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 146);
            this.Controls.Add(this.edtPDFDir);
            this.Controls.Add(this.lblPDFDir);
            this.Controls.Add(this.edtXMLOutputDir);
            this.Controls.Add(this.lblXMLOutputDir);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.edtHTMLCacheDir);
            this.Controls.Add(this.lblHTMLCacheDir);
            this.Name = "Setup";
            this.Text = "Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHTMLCacheDir;
        private System.Windows.Forms.TextBox edtHTMLCacheDir;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblXMLOutputDir;
        private System.Windows.Forms.TextBox edtXMLOutputDir;
        private System.Windows.Forms.Label lblPDFDir;
        private System.Windows.Forms.TextBox edtPDFDir;
    }
}