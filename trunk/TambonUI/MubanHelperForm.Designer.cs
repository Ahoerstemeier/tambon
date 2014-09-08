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
            this.cbxAmphoe = new System.Windows.Forms.ComboBox();
            this.cbxChangwat = new System.Windows.Forms.ComboBox();
            this.lblChangwat = new System.Windows.Forms.Label();
            this.lblAmphoe = new System.Windows.Forms.Label();
            this.lblTambon = new System.Windows.Forms.Label();
            this.cbxTambon = new System.Windows.Forms.ComboBox();
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
            this.edtGeocode.Location = new System.Drawing.Point(129, 57);
            this.edtGeocode.Maximum = new decimal(new int[] {
            969900,
            0,
            0,
            0});
            this.edtGeocode.Name = "edtGeocode";
            this.edtGeocode.Size = new System.Drawing.Size(220, 20);
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
            this.edtText.Location = new System.Drawing.Point(12, 163);
            this.edtText.Multiline = true;
            this.edtText.Name = "edtText";
            this.edtText.Size = new System.Drawing.Size(337, 181);
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
            // cbxAmphoe
            // 
            this.cbxAmphoe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxAmphoe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAmphoe.FormattingEnabled = true;
            this.cbxAmphoe.Location = new System.Drawing.Point(129, 110);
            this.cbxAmphoe.Name = "cbxAmphoe";
            this.cbxAmphoe.Size = new System.Drawing.Size(220, 21);
            this.cbxAmphoe.TabIndex = 19;
            this.cbxAmphoe.SelectedValueChanged += new System.EventHandler(this.cbxAmphoe_SelectedValueChanged);
            // 
            // cbxChangwat
            // 
            this.cbxChangwat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxChangwat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxChangwat.FormattingEnabled = true;
            this.cbxChangwat.Location = new System.Drawing.Point(129, 83);
            this.cbxChangwat.Name = "cbxChangwat";
            this.cbxChangwat.Size = new System.Drawing.Size(220, 21);
            this.cbxChangwat.TabIndex = 18;
            this.cbxChangwat.SelectedValueChanged += new System.EventHandler(this.cbxChangwat_SelectedValueChanged);
            // 
            // lblChangwat
            // 
            this.lblChangwat.AutoSize = true;
            this.lblChangwat.Location = new System.Drawing.Point(9, 86);
            this.lblChangwat.Name = "lblChangwat";
            this.lblChangwat.Size = new System.Drawing.Size(49, 13);
            this.lblChangwat.TabIndex = 20;
            this.lblChangwat.Text = "Province";
            // 
            // lblAmphoe
            // 
            this.lblAmphoe.AutoSize = true;
            this.lblAmphoe.Location = new System.Drawing.Point(9, 113);
            this.lblAmphoe.Name = "lblAmphoe";
            this.lblAmphoe.Size = new System.Drawing.Size(39, 13);
            this.lblAmphoe.TabIndex = 21;
            this.lblAmphoe.Text = "District";
            // 
            // lblTambon
            // 
            this.lblTambon.AutoSize = true;
            this.lblTambon.Location = new System.Drawing.Point(9, 140);
            this.lblTambon.Name = "lblTambon";
            this.lblTambon.Size = new System.Drawing.Size(56, 13);
            this.lblTambon.TabIndex = 23;
            this.lblTambon.Text = "Subdistrict";
            // 
            // cbxTambon
            // 
            this.cbxTambon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxTambon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTambon.FormattingEnabled = true;
            this.cbxTambon.Location = new System.Drawing.Point(129, 137);
            this.cbxTambon.Name = "cbxTambon";
            this.cbxTambon.Size = new System.Drawing.Size(220, 21);
            this.cbxTambon.TabIndex = 22;
            this.cbxTambon.SelectedValueChanged += new System.EventHandler(this.cbxTambon_SelectedValueChanged);
            // 
            // MubanHelperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 356);
            this.Controls.Add(this.lblTambon);
            this.Controls.Add(this.cbxTambon);
            this.Controls.Add(this.lblAmphoe);
            this.Controls.Add(this.lblChangwat);
            this.Controls.Add(this.cbxAmphoe);
            this.Controls.Add(this.cbxChangwat);
            this.Controls.Add(this.btnAddBan);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.edtText);
            this.Controls.Add(this.lblGeocode);
            this.Controls.Add(this.edtGeocode);
            this.Controls.Add(this.chkStripAfter);
            this.Controls.Add(this.chkStripBefore);
            this.Name = "MubanHelperForm";
            this.Text = "MubanHelperForm";
            this.Load += new System.EventHandler(this.MubanHelperForm_Load);
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
        private System.Windows.Forms.ComboBox cbxAmphoe;
        private System.Windows.Forms.ComboBox cbxChangwat;
        private System.Windows.Forms.Label lblChangwat;
        private System.Windows.Forms.Label lblAmphoe;
        private System.Windows.Forms.Label lblTambon;
        private System.Windows.Forms.ComboBox cbxTambon;
    }
}