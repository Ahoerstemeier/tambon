namespace De.AHoerstemeier.Tambon
{
    partial class GeoCoordinateForm
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
            this.lbl_MGRS = new System.Windows.Forms.Label();
            this.edit_MGRS = new System.Windows.Forms.TextBox();
            this.lbl_UTM = new System.Windows.Forms.Label();
            this.edit_UTM = new System.Windows.Forms.TextBox();
            this.lbl_LatLong = new System.Windows.Forms.Label();
            this.edit_LatLong = new System.Windows.Forms.TextBox();
            this.cbx_datum = new System.Windows.Forms.ComboBox();
            this.lbl_datum = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_MGRS
            // 
            this.lbl_MGRS.AutoSize = true;
            this.lbl_MGRS.Location = new System.Drawing.Point(12, 9);
            this.lbl_MGRS.Name = "lbl_MGRS";
            this.lbl_MGRS.Size = new System.Drawing.Size(63, 13);
            this.lbl_MGRS.TabIndex = 0;
            this.lbl_MGRS.Text = "Thai MGRS";
            // 
            // edit_MGRS
            // 
            this.edit_MGRS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edit_MGRS.Location = new System.Drawing.Point(94, 6);
            this.edit_MGRS.Name = "edit_MGRS";
            this.edit_MGRS.Size = new System.Drawing.Size(186, 20);
            this.edit_MGRS.TabIndex = 1;
            this.edit_MGRS.TextChanged += new System.EventHandler(this.edit_MGRS_TextChanged);
            // 
            // lbl_UTM
            // 
            this.lbl_UTM.AutoSize = true;
            this.lbl_UTM.Location = new System.Drawing.Point(12, 34);
            this.lbl_UTM.Name = "lbl_UTM";
            this.lbl_UTM.Size = new System.Drawing.Size(31, 13);
            this.lbl_UTM.TabIndex = 2;
            this.lbl_UTM.Text = "UTM";
            // 
            // edit_UTM
            // 
            this.edit_UTM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edit_UTM.Location = new System.Drawing.Point(94, 31);
            this.edit_UTM.Name = "edit_UTM";
            this.edit_UTM.Size = new System.Drawing.Size(186, 20);
            this.edit_UTM.TabIndex = 3;
            this.edit_UTM.TextChanged += new System.EventHandler(this.edit_UTM_TextChanged);
            // 
            // lbl_LatLong
            // 
            this.lbl_LatLong.AutoSize = true;
            this.lbl_LatLong.Location = new System.Drawing.Point(12, 60);
            this.lbl_LatLong.Name = "lbl_LatLong";
            this.lbl_LatLong.Size = new System.Drawing.Size(51, 13);
            this.lbl_LatLong.TabIndex = 4;
            this.lbl_LatLong.Text = "Lat/Long";
            // 
            // edit_LatLong
            // 
            this.edit_LatLong.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edit_LatLong.Location = new System.Drawing.Point(94, 57);
            this.edit_LatLong.Name = "edit_LatLong";
            this.edit_LatLong.Size = new System.Drawing.Size(186, 20);
            this.edit_LatLong.TabIndex = 5;
            // 
            // cbx_datum
            // 
            this.cbx_datum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_datum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_datum.FormattingEnabled = true;
            this.cbx_datum.Location = new System.Drawing.Point(94, 83);
            this.cbx_datum.Name = "cbx_datum";
            this.cbx_datum.Size = new System.Drawing.Size(186, 21);
            this.cbx_datum.TabIndex = 6;
            this.cbx_datum.SelectedIndexChanged += new System.EventHandler(this.edit_MGRS_TextChanged);
            // 
            // lbl_datum
            // 
            this.lbl_datum.AutoSize = true;
            this.lbl_datum.Location = new System.Drawing.Point(12, 86);
            this.lbl_datum.Name = "lbl_datum";
            this.lbl_datum.Size = new System.Drawing.Size(38, 13);
            this.lbl_datum.TabIndex = 7;
            this.lbl_datum.Text = "Datum";
            // 
            // GeoCoordinateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.lbl_datum);
            this.Controls.Add(this.cbx_datum);
            this.Controls.Add(this.edit_LatLong);
            this.Controls.Add(this.lbl_LatLong);
            this.Controls.Add(this.edit_UTM);
            this.Controls.Add(this.lbl_UTM);
            this.Controls.Add(this.edit_MGRS);
            this.Controls.Add(this.lbl_MGRS);
            this.Name = "GeoCoordinateForm";
            this.Text = "GeoCoordinateForm";
            this.Load += new System.EventHandler(this.GeoCoordinateForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_MGRS;
        private System.Windows.Forms.TextBox edit_MGRS;
        private System.Windows.Forms.Label lbl_UTM;
        private System.Windows.Forms.TextBox edit_UTM;
        private System.Windows.Forms.Label lbl_LatLong;
        private System.Windows.Forms.TextBox edit_LatLong;
        private System.Windows.Forms.ComboBox cbx_datum;
        private System.Windows.Forms.Label lbl_datum;
    }
}