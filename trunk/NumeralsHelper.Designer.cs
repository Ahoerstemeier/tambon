namespace De.AHoerstemeier.Tambon
{
    partial class NumeralsTambonHelper
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
            this.btnDoConvert = new System.Windows.Forms.Button();
            this.boxText = new System.Windows.Forms.TextBox();
            this.btnEncoding = new System.Windows.Forms.Button();
            this.btnMonths = new System.Windows.Forms.Button();
            this.btnTitles = new System.Windows.Forms.Button();
            this.btnInvert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDoConvert
            // 
            this.btnDoConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoConvert.Location = new System.Drawing.Point(316, 250);
            this.btnDoConvert.Name = "btnDoConvert";
            this.btnDoConvert.Size = new System.Drawing.Size(75, 23);
            this.btnDoConvert.TabIndex = 0;
            this.btnDoConvert.Text = "Numerals";
            this.btnDoConvert.UseVisualStyleBackColor = true;
            this.btnDoConvert.Click += new System.EventHandler(this.btnDoConvert_Click);
            // 
            // boxText
            // 
            this.boxText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.boxText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boxText.Location = new System.Drawing.Point(3, 2);
            this.boxText.Multiline = true;
            this.boxText.Name = "boxText";
            this.boxText.Size = new System.Drawing.Size(388, 242);
            this.boxText.TabIndex = 1;
            // 
            // btnEncoding
            // 
            this.btnEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEncoding.Location = new System.Drawing.Point(235, 250);
            this.btnEncoding.Name = "btnEncoding";
            this.btnEncoding.Size = new System.Drawing.Size(75, 23);
            this.btnEncoding.TabIndex = 2;
            this.btnEncoding.Text = "Encoding";
            this.btnEncoding.UseVisualStyleBackColor = true;
            this.btnEncoding.Click += new System.EventHandler(this.btnEncoding_Click);
            // 
            // btnMonths
            // 
            this.btnMonths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMonths.Location = new System.Drawing.Point(154, 250);
            this.btnMonths.Name = "btnMonths";
            this.btnMonths.Size = new System.Drawing.Size(75, 23);
            this.btnMonths.TabIndex = 3;
            this.btnMonths.Text = "Months";
            this.btnMonths.UseVisualStyleBackColor = true;
            this.btnMonths.Click += new System.EventHandler(this.btnMonths_Click);
            // 
            // btnTitles
            // 
            this.btnTitles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTitles.Location = new System.Drawing.Point(73, 250);
            this.btnTitles.Name = "btnTitles";
            this.btnTitles.Size = new System.Drawing.Size(75, 23);
            this.btnTitles.TabIndex = 4;
            this.btnTitles.Text = "Titles";
            this.btnTitles.UseVisualStyleBackColor = true;
            this.btnTitles.Click += new System.EventHandler(this.btnTitles_Click);
            // 
            // btnInvert
            // 
            this.btnInvert.Location = new System.Drawing.Point(3, 250);
            this.btnInvert.Name = "btnInvert";
            this.btnInvert.Size = new System.Drawing.Size(64, 23);
            this.btnInvert.TabIndex = 5;
            this.btnInvert.Text = "Invert";
            this.btnInvert.UseVisualStyleBackColor = true;
            this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
            // 
            // NumeralsTambonHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 273);
            this.Controls.Add(this.btnInvert);
            this.Controls.Add(this.btnTitles);
            this.Controls.Add(this.btnMonths);
            this.Controls.Add(this.btnEncoding);
            this.Controls.Add(this.boxText);
            this.Controls.Add(this.btnDoConvert);
            this.Name = "NumeralsTambonHelper";
            this.Text = "NumeralsTambonHelper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDoConvert;
        private System.Windows.Forms.TextBox boxText;
        private System.Windows.Forms.Button btnEncoding;
        private System.Windows.Forms.Button btnMonths;
        private System.Windows.Forms.Button btnTitles;
        private System.Windows.Forms.Button btnInvert;
    }
}