namespace De.AHoerstemeier.Tambon.UI
{
    partial class MubanHelperForm
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
            this.chkStripBefore = new System.Windows.Forms.CheckBox();
            this.chkStripAfter = new System.Windows.Forms.CheckBox();
            this.edtGeocode = new System.Windows.Forms.NumericUpDown();
            this.lblGeocode = new System.Windows.Forms.Label();
            this.edtText = new System.Windows.Forms.TextBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnAddBan = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.edtGeocode)).BeginInit();
            this.SuspendLayout();
            // 
            // chkStripBefore
            // 
            this.chkStripBefore.AutoSize = true;
            this.chkStripBefore.Checked = true;
            this.chkStripBefore.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStripBefore.Location = new System.Drawing.Point(12, 12);
            this.chkStripBefore.Name = "chkStripBefore";
            this.chkStripBefore.Size = new System.Drawing.Size(129, 17);
            this.chkStripBefore.TabIndex = 0;
            this.chkStripBefore.Text = "Strip text before name";
            this.chkStripBefore.UseVisualStyleBackColor = true;
            // 
            // chkStripAfter
            // 
            this.chkStripAfter.AutoSize = true;
            this.chkStripAfter.Checked = true;
            this.chkStripAfter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStripAfter.Location = new System.Drawing.Point(12, 35);
            this.chkStripAfter.Name = "chkStripAfter";
            this.chkStripAfter.Size = new System.Drawing.Size(120, 17);
            this.chkStripAfter.TabIndex = 1;
            this.chkStripAfter.Text = "Strip text after name";
            this.chkStripAfter.UseVisualStyleBackColor = true;
            // 
            // edtGeocode
            // 
            this.edtGeocode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtGeocode.Location = new System.Drawing.Point(132, 57);
            this.edtGeocode.Maximum = new decimal(new int[] {
            969900,
            0,
            0,
            0});
            this.edtGeocode.Name = "edtGeocode";
            this.edtGeocode.Size = new System.Drawing.Size(217, 20);
            this.edtGeocode.TabIndex = 2;
            // 
            // lblGeocode
            // 
            this.lblGeocode.AutoSize = true;
            this.lblGeocode.Location = new System.Drawing.Point(9, 59);
            this.lblGeocode.Name = "lblGeocode";
            this.lblGeocode.Size = new System.Drawing.Size(78, 13);
            this.lblGeocode.TabIndex = 3;
            this.lblGeocode.Text = "Base Geocode";
            // 
            // edtText
            // 
            this.edtText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtText.Location = new System.Drawing.Point(12, 86);
            this.edtText.Multiline = true;
            this.edtText.Name = "edtText";
            this.edtText.Size = new System.Drawing.Size(337, 163);
            this.edtText.TabIndex = 4;
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(177, 12);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 5;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // btnAddBan
            // 
            this.btnAddBan.Location = new System.Drawing.Point(258, 12);
            this.btnAddBan.Name = "btnAddBan";
            this.btnAddBan.Size = new System.Drawing.Size(75, 23);
            this.btnAddBan.TabIndex = 6;
            this.btnAddBan.Text = "Add บ้าน";
            this.btnAddBan.UseVisualStyleBackColor = true;
            this.btnAddBan.Click += new System.EventHandler(this.btnAddBan_Click);
            // 
            // MubanHelperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 261);
            this.Controls.Add(this.btnAddBan);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.edtText);
            this.Controls.Add(this.lblGeocode);
            this.Controls.Add(this.edtGeocode);
            this.Controls.Add(this.chkStripAfter);
            this.Controls.Add(this.chkStripBefore);
            this.Name = "MubanHelperForm";
            this.Text = "MubanHelperForm";
            ((System.ComponentModel.ISupportInitialize)(this.edtGeocode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkStripBefore;
        private System.Windows.Forms.CheckBox chkStripAfter;
        private System.Windows.Forms.NumericUpDown edtGeocode;
        private System.Windows.Forms.Label lblGeocode;
        private System.Windows.Forms.TextBox edtText;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnAddBan;
    }
}