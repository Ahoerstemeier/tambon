namespace De.AHoerstemeier.Tambon
{
    partial class UnitConverterForm
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
            this.edtRaiInteger = new System.Windows.Forms.NumericUpDown();
            this.edtNganInteger = new System.Windows.Forms.NumericUpDown();
            this.edtTarangWaInteger = new System.Windows.Forms.NumericUpDown();
            this.edtRaiDecimal = new System.Windows.Forms.NumericUpDown();
            this.edtSquareKilometer = new System.Windows.Forms.NumericUpDown();
            this.lblSquareKilometer = new System.Windows.Forms.Label();
            this.lblRaiDecimal = new System.Windows.Forms.Label();
            this.lblRaiInteger = new System.Windows.Forms.Label();
            this.lblNganInteger = new System.Windows.Forms.Label();
            this.lblTarangWa = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.edtRaiInteger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtNganInteger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtTarangWaInteger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtRaiDecimal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtSquareKilometer)).BeginInit();
            this.SuspendLayout();
            // 
            // edtRaiInteger
            // 
            this.edtRaiInteger.Location = new System.Drawing.Point(12, 12);
            this.edtRaiInteger.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.edtRaiInteger.Name = "edtRaiInteger";
            this.edtRaiInteger.Size = new System.Drawing.Size(47, 20);
            this.edtRaiInteger.TabIndex = 0;
            this.edtRaiInteger.ValueChanged += new System.EventHandler(this.edtInteger_ValueChanged);
            // 
            // edtNganInteger
            // 
            this.edtNganInteger.Location = new System.Drawing.Point(99, 12);
            this.edtNganInteger.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.edtNganInteger.Name = "edtNganInteger";
            this.edtNganInteger.Size = new System.Drawing.Size(47, 20);
            this.edtNganInteger.TabIndex = 1;
            this.edtNganInteger.ValueChanged += new System.EventHandler(this.edtInteger_ValueChanged);
            // 
            // edtTarangWaInteger
            // 
            this.edtTarangWaInteger.Location = new System.Drawing.Point(181, 12);
            this.edtTarangWaInteger.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.edtTarangWaInteger.Name = "edtTarangWaInteger";
            this.edtTarangWaInteger.Size = new System.Drawing.Size(47, 20);
            this.edtTarangWaInteger.TabIndex = 2;
            this.edtTarangWaInteger.ValueChanged += new System.EventHandler(this.edtInteger_ValueChanged);
            // 
            // edtRaiDecimal
            // 
            this.edtRaiDecimal.DecimalPlaces = 4;
            this.edtRaiDecimal.Location = new System.Drawing.Point(12, 61);
            this.edtRaiDecimal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.edtRaiDecimal.Name = "edtRaiDecimal";
            this.edtRaiDecimal.Size = new System.Drawing.Size(84, 20);
            this.edtRaiDecimal.TabIndex = 3;
            this.edtRaiDecimal.ValueChanged += new System.EventHandler(this.edtRaiDecimal_ValueChanged);
            // 
            // edtSquareKilometer
            // 
            this.edtSquareKilometer.DecimalPlaces = 6;
            this.edtSquareKilometer.Location = new System.Drawing.Point(12, 110);
            this.edtSquareKilometer.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.edtSquareKilometer.Name = "edtSquareKilometer";
            this.edtSquareKilometer.Size = new System.Drawing.Size(84, 20);
            this.edtSquareKilometer.TabIndex = 4;
            this.edtSquareKilometer.ValueChanged += new System.EventHandler(this.edtSquareKilometer_ValueChanged);
            // 
            // lblSquareKilometer
            // 
            this.lblSquareKilometer.AutoSize = true;
            this.lblSquareKilometer.Location = new System.Drawing.Point(111, 112);
            this.lblSquareKilometer.Name = "lblSquareKilometer";
            this.lblSquareKilometer.Size = new System.Drawing.Size(24, 13);
            this.lblSquareKilometer.TabIndex = 5;
            this.lblSquareKilometer.Text = "km²";
            // 
            // lblRaiDecimal
            // 
            this.lblRaiDecimal.AutoSize = true;
            this.lblRaiDecimal.Location = new System.Drawing.Point(111, 63);
            this.lblRaiDecimal.Name = "lblRaiDecimal";
            this.lblRaiDecimal.Size = new System.Drawing.Size(19, 13);
            this.lblRaiDecimal.TabIndex = 6;
            this.lblRaiDecimal.Text = "ไร่";
            // 
            // lblRaiInteger
            // 
            this.lblRaiInteger.AutoSize = true;
            this.lblRaiInteger.Location = new System.Drawing.Point(65, 14);
            this.lblRaiInteger.Name = "lblRaiInteger";
            this.lblRaiInteger.Size = new System.Drawing.Size(19, 13);
            this.lblRaiInteger.TabIndex = 7;
            this.lblRaiInteger.Text = "ไร่";
            // 
            // lblNganInteger
            // 
            this.lblNganInteger.AutoSize = true;
            this.lblNganInteger.Location = new System.Drawing.Point(152, 14);
            this.lblNganInteger.Name = "lblNganInteger";
            this.lblNganInteger.Size = new System.Drawing.Size(25, 13);
            this.lblNganInteger.TabIndex = 8;
            this.lblNganInteger.Text = "งาน";
            // 
            // lblTarangWa
            // 
            this.lblTarangWa.AutoSize = true;
            this.lblTarangWa.Location = new System.Drawing.Point(234, 14);
            this.lblTarangWa.Name = "lblTarangWa";
            this.lblTarangWa.Size = new System.Drawing.Size(46, 13);
            this.lblTarangWa.TabIndex = 9;
            this.lblTarangWa.Text = "ตารางวา";
            // 
            // UnitConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.lblTarangWa);
            this.Controls.Add(this.lblNganInteger);
            this.Controls.Add(this.lblRaiInteger);
            this.Controls.Add(this.lblRaiDecimal);
            this.Controls.Add(this.lblSquareKilometer);
            this.Controls.Add(this.edtSquareKilometer);
            this.Controls.Add(this.edtRaiDecimal);
            this.Controls.Add(this.edtTarangWaInteger);
            this.Controls.Add(this.edtNganInteger);
            this.Controls.Add(this.edtRaiInteger);
            this.Name = "UnitConverterForm";
            this.Text = "UnitConverterForm";
            ((System.ComponentModel.ISupportInitialize)(this.edtRaiInteger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtNganInteger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtTarangWaInteger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtRaiDecimal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtSquareKilometer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown edtRaiInteger;
        private System.Windows.Forms.NumericUpDown edtNganInteger;
        private System.Windows.Forms.NumericUpDown edtTarangWaInteger;
        private System.Windows.Forms.NumericUpDown edtRaiDecimal;
        private System.Windows.Forms.NumericUpDown edtSquareKilometer;
        private System.Windows.Forms.Label lblSquareKilometer;
        private System.Windows.Forms.Label lblRaiDecimal;
        private System.Windows.Forms.Label lblRaiInteger;
        private System.Windows.Forms.Label lblNganInteger;
        private System.Windows.Forms.Label lblTarangWa;
    }
}