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
            this.edt_MGRS = new System.Windows.Forms.TextBox();
            this.lbl_UTM = new System.Windows.Forms.Label();
            this.edt_UTM = new System.Windows.Forms.TextBox();
            this.lbl_LatLong = new System.Windows.Forms.Label();
            this.edt_LatLong = new System.Windows.Forms.TextBox();
            this.cbx_datum = new System.Windows.Forms.ComboBox();
            this.lbl_datum = new System.Windows.Forms.Label();
            this.lbl_geohash = new System.Windows.Forms.Label();
            this.edt_geohash = new System.Windows.Forms.TextBox();
            this.btnFlyTo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
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
            // edt_MGRS
            // 
            this.edt_MGRS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edt_MGRS.Location = new System.Drawing.Point(94, 6);
            this.edt_MGRS.Name = "edt_MGRS";
            this.edt_MGRS.Size = new System.Drawing.Size(186, 20);
            this.edt_MGRS.TabIndex = 1;
            this.edt_MGRS.TextChanged += new System.EventHandler(this.edit_MGRS_TextChanged);
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
            // edt_UTM
            // 
            this.edt_UTM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edt_UTM.Location = new System.Drawing.Point(94, 31);
            this.edt_UTM.Name = "edt_UTM";
            this.edt_UTM.Size = new System.Drawing.Size(186, 20);
            this.edt_UTM.TabIndex = 3;
            this.edt_UTM.TextChanged += new System.EventHandler(this.edit_UTM_TextChanged);
            // 
            // lbl_LatLong
            // 
            this.lbl_LatLong.AutoSize = true;
            this.lbl_LatLong.Location = new System.Drawing.Point(12, 84);
            this.lbl_LatLong.Name = "lbl_LatLong";
            this.lbl_LatLong.Size = new System.Drawing.Size(51, 13);
            this.lbl_LatLong.TabIndex = 4;
            this.lbl_LatLong.Text = "Lat/Long";
            // 
            // edt_LatLong
            // 
            this.edt_LatLong.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edt_LatLong.Location = new System.Drawing.Point(94, 81);
            this.edt_LatLong.Name = "edt_LatLong";
            this.edt_LatLong.Size = new System.Drawing.Size(186, 20);
            this.edt_LatLong.TabIndex = 5;
            this.edt_LatLong.TextChanged += new System.EventHandler(this.edt_LatLong_TextChanged);
            // 
            // cbx_datum
            // 
            this.cbx_datum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_datum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_datum.FormattingEnabled = true;
            this.cbx_datum.Location = new System.Drawing.Point(94, 106);
            this.cbx_datum.Name = "cbx_datum";
            this.cbx_datum.Size = new System.Drawing.Size(186, 21);
            this.cbx_datum.TabIndex = 6;
            this.cbx_datum.SelectedIndexChanged += new System.EventHandler(this.edit_MGRS_TextChanged);
            // 
            // lbl_datum
            // 
            this.lbl_datum.AutoSize = true;
            this.lbl_datum.Location = new System.Drawing.Point(12, 109);
            this.lbl_datum.Name = "lbl_datum";
            this.lbl_datum.Size = new System.Drawing.Size(38, 13);
            this.lbl_datum.TabIndex = 7;
            this.lbl_datum.Text = "Datum";
            // 
            // lbl_geohash
            // 
            this.lbl_geohash.AutoSize = true;
            this.lbl_geohash.Location = new System.Drawing.Point(12, 59);
            this.lbl_geohash.Name = "lbl_geohash";
            this.lbl_geohash.Size = new System.Drawing.Size(50, 13);
            this.lbl_geohash.TabIndex = 8;
            this.lbl_geohash.Text = "Geohash";
            // 
            // edt_geohash
            // 
            this.edt_geohash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edt_geohash.Location = new System.Drawing.Point(94, 56);
            this.edt_geohash.Name = "edt_geohash";
            this.edt_geohash.Size = new System.Drawing.Size(186, 20);
            this.edt_geohash.TabIndex = 9;
            this.edt_geohash.TextChanged += new System.EventHandler(this.edt_geohash_TextChanged);
            // 
            // btnFlyTo
            // 
            this.btnFlyTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlyTo.Location = new System.Drawing.Point(205, 147);
            this.btnFlyTo.Name = "btnFlyTo";
            this.btnFlyTo.Size = new System.Drawing.Size(75, 23);
            this.btnFlyTo.TabIndex = 10;
            this.btnFlyTo.Text = "Fly To";
            this.btnFlyTo.UseVisualStyleBackColor = true;
            this.btnFlyTo.Click += new System.EventHandler(this.btnFlyTo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "label1";
            // 
            // GeoCoordinateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 185);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFlyTo);
            this.Controls.Add(this.edt_geohash);
            this.Controls.Add(this.lbl_geohash);
            this.Controls.Add(this.lbl_datum);
            this.Controls.Add(this.cbx_datum);
            this.Controls.Add(this.edt_LatLong);
            this.Controls.Add(this.lbl_LatLong);
            this.Controls.Add(this.edt_UTM);
            this.Controls.Add(this.lbl_UTM);
            this.Controls.Add(this.edt_MGRS);
            this.Controls.Add(this.lbl_MGRS);
            this.Name = "GeoCoordinateForm";
            this.Text = "GeoCoordinateForm";
            this.Load += new System.EventHandler(this.GeoCoordinateForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_MGRS;
        private System.Windows.Forms.TextBox edt_MGRS;
        private System.Windows.Forms.Label lbl_UTM;
        private System.Windows.Forms.TextBox edt_UTM;
        private System.Windows.Forms.Label lbl_LatLong;
        private System.Windows.Forms.TextBox edt_LatLong;
        private System.Windows.Forms.ComboBox cbx_datum;
        private System.Windows.Forms.Label lbl_datum;
        private System.Windows.Forms.Label lbl_geohash;
        private System.Windows.Forms.TextBox edt_geohash;
        private System.Windows.Forms.Button btnFlyTo;
        private System.Windows.Forms.Label label1;
    }
}