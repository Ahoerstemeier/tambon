namespace De.AHoerstemeier.Tambon.UI
{
    partial class EntityNumbersForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.edtYear = new System.Windows.Forms.NumericUpDown();
            this.txtData = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtYear)).BeginInit();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Location = new System.Drawing.Point(168, 12);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(496, 452);
            this.chart.TabIndex = 0;
            this.chart.Text = "Entity numbers";
            // 
            // edtYear
            // 
            this.edtYear.Location = new System.Drawing.Point(12, 12);
            this.edtYear.Maximum = new decimal(new int[] {
            2008,
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
            this.edtYear.TabIndex = 37;
            this.edtYear.Value = new decimal(new int[] {
            2005,
            0,
            0,
            0});
            this.edtYear.ValueChanged += new System.EventHandler(this.edtYear_ValueChanged);
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(12, 38);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ReadOnly = true;
            this.txtData.Size = new System.Drawing.Size(150, 426);
            this.txtData.TabIndex = 38;
            // 
            // EntityNumbersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 476);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.edtYear);
            this.Controls.Add(this.chart);
            this.Name = "EntityNumbersForm";
            this.Text = "EntityNumbers";
            this.Load += new System.EventHandler(this.EntityNumbers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.NumericUpDown edtYear;
        private System.Windows.Forms.TextBox txtData;
    }
}