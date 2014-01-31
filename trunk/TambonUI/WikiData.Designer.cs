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
            this.btnRun = new System.Windows.Forms.Button();
            this.chkTypes = new System.Windows.Forms.CheckedListBox();
            this.cbxActivity = new System.Windows.Forms.ComboBox();
            this.edtCollisions = new System.Windows.Forms.TextBox();
            this.chkOverride = new System.Windows.Forms.CheckBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
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
            this.btnCountInterwiki.Enabled = false;
            this.btnCountInterwiki.Location = new System.Drawing.Point(12, 41);
            this.btnCountInterwiki.Name = "btnCountInterwiki";
            this.btnCountInterwiki.Size = new System.Drawing.Size(98, 23);
            this.btnCountInterwiki.TabIndex = 1;
            this.btnCountInterwiki.Text = "by Language";
            this.btnCountInterwiki.UseVisualStyleBackColor = true;
            this.btnCountInterwiki.Click += new System.EventHandler(this.btnCountInterwiki_Click);
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(124, 41);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(92, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // chkTypes
            // 
            this.chkTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTypes.FormattingEnabled = true;
            this.chkTypes.Location = new System.Drawing.Point(222, 64);
            this.chkTypes.Name = "chkTypes";
            this.chkTypes.Size = new System.Drawing.Size(348, 94);
            this.chkTypes.TabIndex = 5;
            // 
            // cbxActivity
            // 
            this.cbxActivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxActivity.FormattingEnabled = true;
            this.cbxActivity.Location = new System.Drawing.Point(222, 14);
            this.cbxActivity.Name = "cbxActivity";
            this.cbxActivity.Size = new System.Drawing.Size(348, 21);
            this.cbxActivity.TabIndex = 6;
            // 
            // edtCollisions
            // 
            this.edtCollisions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtCollisions.Location = new System.Drawing.Point(222, 164);
            this.edtCollisions.Multiline = true;
            this.edtCollisions.Name = "edtCollisions";
            this.edtCollisions.Size = new System.Drawing.Size(348, 258);
            this.edtCollisions.TabIndex = 7;
            // 
            // chkOverride
            // 
            this.chkOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOverride.AutoSize = true;
            this.chkOverride.Location = new System.Drawing.Point(222, 41);
            this.chkOverride.Name = "chkOverride";
            this.chkOverride.Size = new System.Drawing.Size(66, 17);
            this.chkOverride.TabIndex = 8;
            this.chkOverride.Text = "Override";
            this.chkOverride.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(124, 12);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(92, 23);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Enabled = false;
            this.btnLogout.Location = new System.Drawing.Point(124, 70);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(92, 25);
            this.btnLogout.TabIndex = 10;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(12, 105);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(98, 23);
            this.btnTest.TabIndex = 11;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // WikiData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 434);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.chkOverride);
            this.Controls.Add(this.edtCollisions);
            this.Controls.Add(this.cbxActivity);
            this.Controls.Add(this.chkTypes);
            this.Controls.Add(this.btnRun);
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
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.CheckedListBox chkTypes;
        private System.Windows.Forms.ComboBox cbxActivity;
        private System.Windows.Forms.TextBox edtCollisions;
        private System.Windows.Forms.CheckBox chkOverride;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnTest;
    }
}