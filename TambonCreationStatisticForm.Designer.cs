namespace De.AHoerstemeier.Tambon
{
    partial class TambonCreationStatisticForm
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
            this.edtYearEnd = new System.Windows.Forms.NumericUpDown();
            this.edtYearStart = new System.Windows.Forms.NumericUpDown();
            this.lblStartYear = new System.Windows.Forms.Label();
            this.lblEndYear = new System.Windows.Forms.Label();
            this.btnCalc = new System.Windows.Forms.Button();
            this.edtData = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.edtYearEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtYearStart)).BeginInit();
            this.SuspendLayout();
            // 
            // edtYearEnd
            // 
            this.edtYearEnd.Location = new System.Drawing.Point(108, 38);
            this.edtYearEnd.Maximum = new decimal(new int[] {
            2008,
            0,
            0,
            0});
            this.edtYearEnd.Minimum = new decimal(new int[] {
            1883,
            0,
            0,
            0});
            this.edtYearEnd.Name = "edtYearEnd";
            this.edtYearEnd.Size = new System.Drawing.Size(55, 20);
            this.edtYearEnd.TabIndex = 8;
            this.edtYearEnd.Value = new decimal(new int[] {
            2008,
            0,
            0,
            0});
            // 
            // edtYearStart
            // 
            this.edtYearStart.Location = new System.Drawing.Point(108, 12);
            this.edtYearStart.Maximum = new decimal(new int[] {
            2008,
            0,
            0,
            0});
            this.edtYearStart.Minimum = new decimal(new int[] {
            1883,
            0,
            0,
            0});
            this.edtYearStart.Name = "edtYearStart";
            this.edtYearStart.Size = new System.Drawing.Size(55, 20);
            this.edtYearStart.TabIndex = 7;
            this.edtYearStart.Value = new decimal(new int[] {
            1990,
            0,
            0,
            0});
            // 
            // lblStartYear
            // 
            this.lblStartYear.AutoSize = true;
            this.lblStartYear.Location = new System.Drawing.Point(12, 14);
            this.lblStartYear.Name = "lblStartYear";
            this.lblStartYear.Size = new System.Drawing.Size(54, 13);
            this.lblStartYear.TabIndex = 9;
            this.lblStartYear.Text = "Start Year";
            // 
            // lblEndYear
            // 
            this.lblEndYear.AutoSize = true;
            this.lblEndYear.Location = new System.Drawing.Point(12, 40);
            this.lblEndYear.Name = "lblEndYear";
            this.lblEndYear.Size = new System.Drawing.Size(51, 13);
            this.lblEndYear.TabIndex = 10;
            this.lblEndYear.Text = "End Year";
            // 
            // btnCalc
            // 
            this.btnCalc.Location = new System.Drawing.Point(205, 14);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(75, 23);
            this.btnCalc.TabIndex = 11;
            this.btnCalc.Text = "Calc";
            this.btnCalc.UseVisualStyleBackColor = true;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // edtData
            // 
            this.edtData.Location = new System.Drawing.Point(15, 79);
            this.edtData.Multiline = true;
            this.edtData.Name = "edtData";
            this.edtData.Size = new System.Drawing.Size(265, 182);
            this.edtData.TabIndex = 12;
            // 
            // TambonCreationStatisticForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.edtData);
            this.Controls.Add(this.btnCalc);
            this.Controls.Add(this.lblEndYear);
            this.Controls.Add(this.lblStartYear);
            this.Controls.Add(this.edtYearEnd);
            this.Controls.Add(this.edtYearStart);
            this.Name = "TambonCreationStatisticForm";
            this.Text = "TambonCreationStatisticForm";
            ((System.ComponentModel.ISupportInitialize)(this.edtYearEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtYearStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown edtYearEnd;
        private System.Windows.Forms.NumericUpDown edtYearStart;
        private System.Windows.Forms.Label lblStartYear;
        private System.Windows.Forms.Label lblEndYear;
        private System.Windows.Forms.Button btnCalc;
        private System.Windows.Forms.TextBox edtData;
    }
}