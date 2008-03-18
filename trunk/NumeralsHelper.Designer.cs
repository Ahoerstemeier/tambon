namespace De.AHoerstemeier.Tambon
{
    partial class NumeralsHelper
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
            this.SuspendLayout();
            // 
            // btnDoConvert
            // 
            this.btnDoConvert.Location = new System.Drawing.Point(217, 250);
            this.btnDoConvert.Name = "btnDoConvert";
            this.btnDoConvert.Size = new System.Drawing.Size(75, 23);
            this.btnDoConvert.TabIndex = 0;
            this.btnDoConvert.Text = "Go";
            this.btnDoConvert.UseVisualStyleBackColor = true;
            this.btnDoConvert.Click += new System.EventHandler(this.btnDoConvert_Click);
            // 
            // boxText
            // 
            this.boxText.Location = new System.Drawing.Point(3, 2);
            this.boxText.Multiline = true;
            this.boxText.Name = "boxText";
            this.boxText.Size = new System.Drawing.Size(289, 242);
            this.boxText.TabIndex = 1;
            // 
            // NumeralsHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.boxText);
            this.Controls.Add(this.btnDoConvert);
            this.Name = "NumeralsHelper";
            this.Text = "NumeralsHelper";
            this.Load += new System.EventHandler(this.NumeralsHelper_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDoConvert;
        private System.Windows.Forms.TextBox boxText;
    }
}