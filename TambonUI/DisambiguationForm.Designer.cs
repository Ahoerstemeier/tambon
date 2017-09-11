namespace De.AHoerstemeier.Tambon.UI
{
    partial class DisambiguationForm
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
            this.cbxEntityType = new System.Windows.Forms.ComboBox();
            this.lbxNames = new System.Windows.Forms.ListBox();
            this.btnThaiWikipedia = new System.Windows.Forms.Button();
            this.cbxProvinces = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cbxEntityType
            // 
            this.cbxEntityType.FormattingEnabled = true;
            this.cbxEntityType.Location = new System.Drawing.Point(12, 12);
            this.cbxEntityType.Name = "cbxEntityType";
            this.cbxEntityType.Size = new System.Drawing.Size(260, 21);
            this.cbxEntityType.TabIndex = 0;
            this.cbxEntityType.SelectedValueChanged += new System.EventHandler(this.cbxEntityType_SelectedValueChanged);
            // 
            // lbxNames
            // 
            this.lbxNames.FormattingEnabled = true;
            this.lbxNames.Location = new System.Drawing.Point(16, 77);
            this.lbxNames.Name = "lbxNames";
            this.lbxNames.Size = new System.Drawing.Size(256, 147);
            this.lbxNames.TabIndex = 1;
            // 
            // btnThaiWikipedia
            // 
            this.btnThaiWikipedia.Location = new System.Drawing.Point(176, 230);
            this.btnThaiWikipedia.Name = "btnThaiWikipedia";
            this.btnThaiWikipedia.Size = new System.Drawing.Size(96, 23);
            this.btnThaiWikipedia.TabIndex = 2;
            this.btnThaiWikipedia.Text = "Thai Wikipedia";
            this.btnThaiWikipedia.UseVisualStyleBackColor = true;
            this.btnThaiWikipedia.Click += new System.EventHandler(this.btnThaiWikipedia_Click);
            // 
            // cbxProvinces
            // 
            this.cbxProvinces.FormattingEnabled = true;
            this.cbxProvinces.Location = new System.Drawing.Point(12, 39);
            this.cbxProvinces.Name = "cbxProvinces";
            this.cbxProvinces.Size = new System.Drawing.Size(260, 21);
            this.cbxProvinces.TabIndex = 3;
            this.cbxProvinces.SelectedValueChanged += new System.EventHandler(this.cbxProvinces_SelectedValueChanged);
            // 
            // DisambiguationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.cbxProvinces);
            this.Controls.Add(this.btnThaiWikipedia);
            this.Controls.Add(this.lbxNames);
            this.Controls.Add(this.cbxEntityType);
            this.Name = "DisambiguationForm";
            this.Text = "DisambiguationForm";
            this.Load += new System.EventHandler(this.DisambiguationForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxEntityType;
        private System.Windows.Forms.ListBox lbxNames;
        private System.Windows.Forms.Button btnThaiWikipedia;
        private System.Windows.Forms.ComboBox cbxProvinces;
    }
}