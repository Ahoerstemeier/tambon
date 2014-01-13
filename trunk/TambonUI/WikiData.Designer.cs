namespace De.AHoerstemeier.Tambon.UI
{
    partial class WikiData
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
            this.btnStat = new System.Windows.Forms.Button();
            this.btnCountInterwiki = new System.Windows.Forms.Button();
            this.btnTestGet = new System.Windows.Forms.Button();
            this.btnTestSet = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.chkTypes = new System.Windows.Forms.CheckedListBox();
            this.cbxActivity = new System.Windows.Forms.ComboBox();
            this.edtCollisions = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnStat
            // 
            this.btnStat.Location = new System.Drawing.Point(12, 12);
            this.btnStat.Name = "btnStat";
            this.btnStat.Size = new System.Drawing.Size(98, 23);
            this.btnStat.TabIndex = 0;
            this.btnStat.Text = "Statistics";
            this.btnStat.UseVisualStyleBackColor = true;
            this.btnStat.Click += new System.EventHandler(this.btnStatistics_Click);
            // 
            // btnCountInterwiki
            // 
            this.btnCountInterwiki.Location = new System.Drawing.Point(12, 41);
            this.btnCountInterwiki.Name = "btnCountInterwiki";
            this.btnCountInterwiki.Size = new System.Drawing.Size(98, 23);
            this.btnCountInterwiki.TabIndex = 1;
            this.btnCountInterwiki.Text = "by Language";
            this.btnCountInterwiki.UseVisualStyleBackColor = true;
            this.btnCountInterwiki.Click += new System.EventHandler(this.btnCountInterwiki_Click);
            // 
            // btnTestGet
            // 
            this.btnTestGet.Location = new System.Drawing.Point(12, 70);
            this.btnTestGet.Name = "btnTestGet";
            this.btnTestGet.Size = new System.Drawing.Size(98, 23);
            this.btnTestGet.TabIndex = 2;
            this.btnTestGet.Text = "Test Get";
            this.btnTestGet.UseVisualStyleBackColor = true;
            this.btnTestGet.Click += new System.EventHandler(this.btnTestGet_Click);
            // 
            // btnTestSet
            // 
            this.btnTestSet.Location = new System.Drawing.Point(12, 99);
            this.btnTestSet.Name = "btnTestSet";
            this.btnTestSet.Size = new System.Drawing.Size(98, 23);
            this.btnTestSet.TabIndex = 3;
            this.btnTestSet.Text = "Test Set";
            this.btnTestSet.UseVisualStyleBackColor = true;
            this.btnTestSet.Click += new System.EventHandler(this.btnTestSet_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(116, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkTypes
            // 
            this.chkTypes.FormattingEnabled = true;
            this.chkTypes.Location = new System.Drawing.Point(222, 41);
            this.chkTypes.Name = "chkTypes";
            this.chkTypes.Size = new System.Drawing.Size(120, 94);
            this.chkTypes.TabIndex = 5;
            // 
            // cbxActivity
            // 
            this.cbxActivity.FormattingEnabled = true;
            this.cbxActivity.Location = new System.Drawing.Point(222, 14);
            this.cbxActivity.Name = "cbxActivity";
            this.cbxActivity.Size = new System.Drawing.Size(120, 21);
            this.cbxActivity.TabIndex = 6;
            // 
            // edtCollisions
            // 
            this.edtCollisions.Location = new System.Drawing.Point(222, 141);
            this.edtCollisions.Multiline = true;
            this.edtCollisions.Name = "edtCollisions";
            this.edtCollisions.Size = new System.Drawing.Size(120, 108);
            this.edtCollisions.TabIndex = 7;
            // 
            // WikiData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 261);
            this.Controls.Add(this.edtCollisions);
            this.Controls.Add(this.cbxActivity);
            this.Controls.Add(this.chkTypes);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTestSet);
            this.Controls.Add(this.btnTestGet);
            this.Controls.Add(this.btnCountInterwiki);
            this.Controls.Add(this.btnStat);
            this.Name = "WikiData";
            this.Text = "WikiData";
            this.Load += new System.EventHandler(this.WikiData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStat;
        private System.Windows.Forms.Button btnCountInterwiki;
        private System.Windows.Forms.Button btnTestGet;
        private System.Windows.Forms.Button btnTestSet;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckedListBox chkTypes;
        private System.Windows.Forms.ComboBox cbxActivity;
        private System.Windows.Forms.TextBox edtCollisions;
    }
}