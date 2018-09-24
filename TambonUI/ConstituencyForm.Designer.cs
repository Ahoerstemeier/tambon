namespace De.AHoerstemeier.Tambon
{
    partial class ConstituencyForm
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
            this.lblYear = new System.Windows.Forms.Label();
            this.edtYear = new System.Windows.Forms.NumericUpDown();
            this.lblNumberOfConstituencies = new System.Windows.Forms.Label();
            this.edtNumberOfConstituencies = new System.Windows.Forms.NumericUpDown();
            this.rbxNational = new System.Windows.Forms.RadioButton();
            this.rbxProvince = new System.Windows.Forms.RadioButton();
            this.cbxProvince = new System.Windows.Forms.ComboBox();
            this.btnCalc = new System.Windows.Forms.Button();
            this.txtData = new System.Windows.Forms.TextBox();
            this.btnSaveCsv = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.edtYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtNumberOfConstituencies)).BeginInit();
            this.SuspendLayout();
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(12, 9);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(82, 13);
            this.lblYear.TabIndex = 0;
            this.lblYear.Text = "Reference Year";
            // 
            // edtYear
            // 
            this.edtYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtYear.Location = new System.Drawing.Point(223, 7);
            this.edtYear.Maximum = new decimal(new int[] {
            2018,
            0,
            0,
            0});
            this.edtYear.Minimum = new decimal(new int[] {
            1993,
            0,
            0,
            0});
            this.edtYear.Name = "edtYear";
            this.edtYear.Size = new System.Drawing.Size(57, 20);
            this.edtYear.TabIndex = 4;
            this.edtYear.Value = new decimal(new int[] {
            2018,
            0,
            0,
            0});
            // 
            // lblNumberOfConstituencies
            // 
            this.lblNumberOfConstituencies.AutoSize = true;
            this.lblNumberOfConstituencies.Location = new System.Drawing.Point(12, 31);
            this.lblNumberOfConstituencies.Name = "lblNumberOfConstituencies";
            this.lblNumberOfConstituencies.Size = new System.Drawing.Size(128, 13);
            this.lblNumberOfConstituencies.TabIndex = 5;
            this.lblNumberOfConstituencies.Text = "Number of Constituencies";
            // 
            // edtNumberOfConstituencies
            // 
            this.edtNumberOfConstituencies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edtNumberOfConstituencies.Location = new System.Drawing.Point(223, 31);
            this.edtNumberOfConstituencies.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.edtNumberOfConstituencies.Minimum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.edtNumberOfConstituencies.Name = "edtNumberOfConstituencies";
            this.edtNumberOfConstituencies.Size = new System.Drawing.Size(57, 20);
            this.edtNumberOfConstituencies.TabIndex = 6;
            this.edtNumberOfConstituencies.Value = new decimal(new int[] {
            350,
            0,
            0,
            0});
            // 
            // rbxNational
            // 
            this.rbxNational.AutoSize = true;
            this.rbxNational.Checked = true;
            this.rbxNational.Location = new System.Drawing.Point(15, 57);
            this.rbxNational.Name = "rbxNational";
            this.rbxNational.Size = new System.Drawing.Size(78, 17);
            this.rbxNational.TabIndex = 7;
            this.rbxNational.TabStop = true;
            this.rbxNational.Text = "Nationwide";
            this.rbxNational.UseVisualStyleBackColor = true;
            // 
            // rbxProvince
            // 
            this.rbxProvince.AutoSize = true;
            this.rbxProvince.Location = new System.Drawing.Point(15, 80);
            this.rbxProvince.Name = "rbxProvince";
            this.rbxProvince.Size = new System.Drawing.Size(67, 17);
            this.rbxProvince.TabIndex = 8;
            this.rbxProvince.TabStop = true;
            this.rbxProvince.Text = "Province";
            this.rbxProvince.UseVisualStyleBackColor = true;
            // 
            // cbxProvince
            // 
            this.cbxProvince.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxProvince.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProvince.FormattingEnabled = true;
            this.cbxProvince.Location = new System.Drawing.Point(159, 80);
            this.cbxProvince.Name = "cbxProvince";
            this.cbxProvince.Size = new System.Drawing.Size(121, 21);
            this.cbxProvince.TabIndex = 9;
            // 
            // btnCalc
            // 
            this.btnCalc.Location = new System.Drawing.Point(7, 137);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(75, 23);
            this.btnCalc.TabIndex = 10;
            this.btnCalc.Text = "Calculate";
            this.btnCalc.UseVisualStyleBackColor = true;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.Location = new System.Drawing.Point(7, 166);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(273, 179);
            this.txtData.TabIndex = 11;
            // 
            // btnSaveCsv
            // 
            this.btnSaveCsv.Location = new System.Drawing.Point(205, 137);
            this.btnSaveCsv.Name = "btnSaveCsv";
            this.btnSaveCsv.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCsv.TabIndex = 15;
            this.btnSaveCsv.Text = "Save as CSV";
            this.btnSaveCsv.UseVisualStyleBackColor = true;
            this.btnSaveCsv.Click += new System.EventHandler(this.btnSaveCsv_Click);
            // 
            // ConstituencyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 357);
            this.Controls.Add(this.btnSaveCsv);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.btnCalc);
            this.Controls.Add(this.cbxProvince);
            this.Controls.Add(this.rbxProvince);
            this.Controls.Add(this.rbxNational);
            this.Controls.Add(this.edtNumberOfConstituencies);
            this.Controls.Add(this.lblNumberOfConstituencies);
            this.Controls.Add(this.edtYear);
            this.Controls.Add(this.lblYear);
            this.Name = "ConstituencyForm";
            this.Text = "ConstituencyForm";
            this.Load += new System.EventHandler(this.ConstituencyForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edtYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtNumberOfConstituencies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.NumericUpDown edtYear;
        private System.Windows.Forms.Label lblNumberOfConstituencies;
        private System.Windows.Forms.NumericUpDown edtNumberOfConstituencies;
        private System.Windows.Forms.RadioButton rbxNational;
        private System.Windows.Forms.RadioButton rbxProvince;
        private System.Windows.Forms.ComboBox cbxProvince;
        private System.Windows.Forms.Button btnCalc;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.Button btnSaveCsv;
    }
}