namespace De.AHoerstemeier.Tambon
{
    partial class CreationStatisticForm
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
            this.btnCalcTambon = new System.Windows.Forms.Button();
            this.edtData = new System.Windows.Forms.TextBox();
            this.btnCalcMuban = new System.Windows.Forms.Button();
            this.btnDates = new System.Windows.Forms.Button();
            this.btnCalcAmphoe = new System.Windows.Forms.Button();
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
            // btnCalcTambon
            // 
            this.btnCalcTambon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalcTambon.Location = new System.Drawing.Point(232, 33);
            this.btnCalcTambon.Name = "btnCalcTambon";
            this.btnCalcTambon.Size = new System.Drawing.Size(75, 23);
            this.btnCalcTambon.TabIndex = 11;
            this.btnCalcTambon.Text = "Tambon";
            this.btnCalcTambon.UseVisualStyleBackColor = true;
            this.btnCalcTambon.Click += new System.EventHandler(this.btnCalcTambon_Click);
            // 
            // edtData
            // 
            this.edtData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edtData.Location = new System.Drawing.Point(15, 132);
            this.edtData.Multiline = true;
            this.edtData.Name = "edtData";
            this.edtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.edtData.Size = new System.Drawing.Size(292, 305);
            this.edtData.TabIndex = 12;
            // 
            // btnCalcMuban
            // 
            this.btnCalcMuban.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalcMuban.Location = new System.Drawing.Point(232, 64);
            this.btnCalcMuban.Name = "btnCalcMuban";
            this.btnCalcMuban.Size = new System.Drawing.Size(75, 23);
            this.btnCalcMuban.TabIndex = 13;
            this.btnCalcMuban.Text = "Muban";
            this.btnCalcMuban.UseVisualStyleBackColor = true;
            this.btnCalcMuban.Click += new System.EventHandler(this.btnCalcMuban_Click);
            // 
            // btnDates
            // 
            this.btnDates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDates.Location = new System.Drawing.Point(232, 103);
            this.btnDates.Name = "btnDates";
            this.btnDates.Size = new System.Drawing.Size(75, 23);
            this.btnDates.TabIndex = 14;
            this.btnDates.Text = "Dates";
            this.btnDates.UseVisualStyleBackColor = true;
            this.btnDates.Click += new System.EventHandler(this.btnDates_Click);
            // 
            // btnCalcAmphoe
            // 
            this.btnCalcAmphoe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalcAmphoe.Location = new System.Drawing.Point(232, 4);
            this.btnCalcAmphoe.Name = "btnCalcAmphoe";
            this.btnCalcAmphoe.Size = new System.Drawing.Size(75, 23);
            this.btnCalcAmphoe.TabIndex = 15;
            this.btnCalcAmphoe.Text = "Amphoe";
            this.btnCalcAmphoe.UseVisualStyleBackColor = true;
            this.btnCalcAmphoe.Click += new System.EventHandler(this.btnCalcAmphoe_Click);
            // 
            // CreationStatisticForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 449);
            this.Controls.Add(this.btnCalcAmphoe);
            this.Controls.Add(this.btnDates);
            this.Controls.Add(this.btnCalcMuban);
            this.Controls.Add(this.edtData);
            this.Controls.Add(this.btnCalcTambon);
            this.Controls.Add(this.lblEndYear);
            this.Controls.Add(this.lblStartYear);
            this.Controls.Add(this.edtYearEnd);
            this.Controls.Add(this.edtYearStart);
            this.Name = "CreationStatisticForm";
            this.Text = "TambonCreationStatisticForm";
            this.Load += new System.EventHandler(this.CreationStatisticForm_Load);
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
        private System.Windows.Forms.Button btnCalcTambon;
        private System.Windows.Forms.TextBox edtData;
        private System.Windows.Forms.Button btnCalcMuban;
        private System.Windows.Forms.Button btnDates;
        private System.Windows.Forms.Button btnCalcAmphoe;
    }
}