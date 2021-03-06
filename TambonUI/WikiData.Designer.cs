﻿namespace De.AHoerstemeier.Tambon.UI
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
            this.btnCheckCommonsCategory = new System.Windows.Forms.Button();
            this.btnCategory = new System.Windows.Forms.Button();
            this.edtStartingItemId = new System.Windows.Forms.TextBox();
            this.lblStartingItemId = new System.Windows.Forms.Label();
            this.btnAllItems = new System.Windows.Forms.Button();
            this.cbxChangwat = new System.Windows.Forms.ComboBox();
            this.cbxAmphoe = new System.Windows.Forms.ComboBox();
            this.lblTambonInfo = new System.Windows.Forms.Label();
            this.btnCreateTambon = new System.Windows.Forms.Button();
            this.cbxLocalGovernments = new System.Windows.Forms.ComboBox();
            this.btnCreateLocalGovernment = new System.Windows.Forms.Button();
            this.lblSpecificItemId = new System.Windows.Forms.Label();
            this.edtSpecificItemId = new System.Windows.Forms.TextBox();
            this.btnTambonList = new System.Windows.Forms.Button();
            this.btnLaoList = new System.Windows.Forms.Button();
            this.btnMap = new System.Windows.Forms.Button();
            this.btnAmphoeCategory = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblLaoInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStat
            // 
            this.btnStat.Location = new System.Drawing.Point(16, 15);
            this.btnStat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStat.Name = "btnStat";
            this.btnStat.Size = new System.Drawing.Size(131, 28);
            this.btnStat.TabIndex = 0;
            this.btnStat.Text = "Statistics";
            this.btnStat.UseVisualStyleBackColor = true;
            this.btnStat.Click += new System.EventHandler(this.btnStatistics_Click);
            // 
            // btnCountInterwiki
            // 
            this.btnCountInterwiki.Enabled = false;
            this.btnCountInterwiki.Location = new System.Drawing.Point(16, 50);
            this.btnCountInterwiki.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCountInterwiki.Name = "btnCountInterwiki";
            this.btnCountInterwiki.Size = new System.Drawing.Size(131, 28);
            this.btnCountInterwiki.TabIndex = 1;
            this.btnCountInterwiki.Text = "by Language";
            this.btnCountInterwiki.UseVisualStyleBackColor = true;
            this.btnCountInterwiki.Click += new System.EventHandler(this.btnCountInterwiki_Click);
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(165, 50);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(123, 28);
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
            this.chkTypes.Location = new System.Drawing.Point(296, 79);
            this.chkTypes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkTypes.Name = "chkTypes";
            this.chkTypes.Size = new System.Drawing.Size(463, 106);
            this.chkTypes.TabIndex = 5;
            // 
            // cbxActivity
            // 
            this.cbxActivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxActivity.FormattingEnabled = true;
            this.cbxActivity.Location = new System.Drawing.Point(296, 17);
            this.cbxActivity.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxActivity.Name = "cbxActivity";
            this.cbxActivity.Size = new System.Drawing.Size(463, 24);
            this.cbxActivity.TabIndex = 6;
            // 
            // edtCollisions
            // 
            this.edtCollisions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtCollisions.Location = new System.Drawing.Point(296, 202);
            this.edtCollisions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.edtCollisions.Multiline = true;
            this.edtCollisions.Name = "edtCollisions";
            this.edtCollisions.Size = new System.Drawing.Size(463, 317);
            this.edtCollisions.TabIndex = 7;
            // 
            // chkOverride
            // 
            this.chkOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOverride.AutoSize = true;
            this.chkOverride.Location = new System.Drawing.Point(296, 50);
            this.chkOverride.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkOverride.Name = "chkOverride";
            this.chkOverride.Size = new System.Drawing.Size(85, 21);
            this.chkOverride.TabIndex = 8;
            this.chkOverride.Text = "Override";
            this.chkOverride.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(165, 15);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(123, 28);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Enabled = false;
            this.btnLogout.Location = new System.Drawing.Point(165, 86);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(123, 31);
            this.btnLogout.TabIndex = 10;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnCheckCommonsCategory
            // 
            this.btnCheckCommonsCategory.Enabled = false;
            this.btnCheckCommonsCategory.Location = new System.Drawing.Point(16, 129);
            this.btnCheckCommonsCategory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCheckCommonsCategory.Name = "btnCheckCommonsCategory";
            this.btnCheckCommonsCategory.Size = new System.Drawing.Size(131, 28);
            this.btnCheckCommonsCategory.TabIndex = 11;
            this.btnCheckCommonsCategory.Text = "Check Commons";
            this.btnCheckCommonsCategory.UseVisualStyleBackColor = true;
            this.btnCheckCommonsCategory.Click += new System.EventHandler(this.btnCheckCommonsCategory_Click);
            // 
            // btnCategory
            // 
            this.btnCategory.Location = new System.Drawing.Point(16, 165);
            this.btnCategory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCategory.Name = "btnCategory";
            this.btnCategory.Size = new System.Drawing.Size(131, 28);
            this.btnCategory.TabIndex = 12;
            this.btnCategory.Text = "Category";
            this.btnCategory.UseVisualStyleBackColor = true;
            this.btnCategory.Click += new System.EventHandler(this.btnCategory_Click);
            // 
            // edtStartingItemId
            // 
            this.edtStartingItemId.Location = new System.Drawing.Point(16, 228);
            this.edtStartingItemId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.edtStartingItemId.Name = "edtStartingItemId";
            this.edtStartingItemId.Size = new System.Drawing.Size(129, 22);
            this.edtStartingItemId.TabIndex = 13;
            this.edtStartingItemId.Text = "17000000";
            // 
            // lblStartingItemId
            // 
            this.lblStartingItemId.AutoSize = true;
            this.lblStartingItemId.Location = new System.Drawing.Point(16, 208);
            this.lblStartingItemId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStartingItemId.Name = "lblStartingItemId";
            this.lblStartingItemId.Size = new System.Drawing.Size(102, 17);
            this.lblStartingItemId.TabIndex = 14;
            this.lblStartingItemId.Text = "Starting Item Id";
            // 
            // btnAllItems
            // 
            this.btnAllItems.Location = new System.Drawing.Point(16, 86);
            this.btnAllItems.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAllItems.Name = "btnAllItems";
            this.btnAllItems.Size = new System.Drawing.Size(131, 28);
            this.btnAllItems.TabIndex = 15;
            this.btnAllItems.Text = "All Items";
            this.btnAllItems.UseVisualStyleBackColor = true;
            this.btnAllItems.Click += new System.EventHandler(this.btnAllItems_Click);
            // 
            // cbxChangwat
            // 
            this.cbxChangwat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxChangwat.FormattingEnabled = true;
            this.cbxChangwat.Location = new System.Drawing.Point(16, 277);
            this.cbxChangwat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxChangwat.Name = "cbxChangwat";
            this.cbxChangwat.Size = new System.Drawing.Size(271, 24);
            this.cbxChangwat.TabIndex = 16;
            this.cbxChangwat.SelectedValueChanged += new System.EventHandler(this.cbxChangwat_SelectedValueChanged);
            // 
            // cbxAmphoe
            // 
            this.cbxAmphoe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAmphoe.FormattingEnabled = true;
            this.cbxAmphoe.Location = new System.Drawing.Point(16, 310);
            this.cbxAmphoe.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxAmphoe.Name = "cbxAmphoe";
            this.cbxAmphoe.Size = new System.Drawing.Size(271, 24);
            this.cbxAmphoe.TabIndex = 17;
            this.cbxAmphoe.SelectedValueChanged += new System.EventHandler(this.cbxAmphoe_SelectedValueChanged);
            // 
            // lblTambonInfo
            // 
            this.lblTambonInfo.AutoSize = true;
            this.lblTambonInfo.Location = new System.Drawing.Point(16, 340);
            this.lblTambonInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTambonInfo.Name = "lblTambonInfo";
            this.lblTambonInfo.Size = new System.Drawing.Size(83, 17);
            this.lblTambonInfo.TabIndex = 18;
            this.lblTambonInfo.Text = "TambonInfo";
            // 
            // btnCreateTambon
            // 
            this.btnCreateTambon.Enabled = false;
            this.btnCreateTambon.Location = new System.Drawing.Point(165, 359);
            this.btnCreateTambon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCreateTambon.Name = "btnCreateTambon";
            this.btnCreateTambon.Size = new System.Drawing.Size(123, 28);
            this.btnCreateTambon.TabIndex = 19;
            this.btnCreateTambon.Text = "Create";
            this.btnCreateTambon.UseVisualStyleBackColor = true;
            this.btnCreateTambon.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // cbxLocalGovernments
            // 
            this.cbxLocalGovernments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLocalGovernments.FormattingEnabled = true;
            this.cbxLocalGovernments.Location = new System.Drawing.Point(16, 395);
            this.cbxLocalGovernments.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxLocalGovernments.Name = "cbxLocalGovernments";
            this.cbxLocalGovernments.Size = new System.Drawing.Size(271, 24);
            this.cbxLocalGovernments.TabIndex = 20;
            this.cbxLocalGovernments.SelectedValueChanged += new System.EventHandler(this.cbxLocalGovernments_SelectedValueChanged);
            // 
            // btnCreateLocalGovernment
            // 
            this.btnCreateLocalGovernment.Enabled = false;
            this.btnCreateLocalGovernment.Location = new System.Drawing.Point(165, 428);
            this.btnCreateLocalGovernment.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCreateLocalGovernment.Name = "btnCreateLocalGovernment";
            this.btnCreateLocalGovernment.Size = new System.Drawing.Size(123, 28);
            this.btnCreateLocalGovernment.TabIndex = 21;
            this.btnCreateLocalGovernment.Text = "Create";
            this.btnCreateLocalGovernment.UseVisualStyleBackColor = true;
            this.btnCreateLocalGovernment.Click += new System.EventHandler(this.btnCreateLocalGovernment_Click);
            // 
            // lblSpecificItemId
            // 
            this.lblSpecificItemId.AutoSize = true;
            this.lblSpecificItemId.Location = new System.Drawing.Point(161, 208);
            this.lblSpecificItemId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpecificItemId.Name = "lblSpecificItemId";
            this.lblSpecificItemId.Size = new System.Drawing.Size(102, 17);
            this.lblSpecificItemId.TabIndex = 23;
            this.lblSpecificItemId.Text = "Specific Item Id";
            // 
            // edtSpecificItemId
            // 
            this.edtSpecificItemId.Location = new System.Drawing.Point(161, 228);
            this.edtSpecificItemId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.edtSpecificItemId.Name = "edtSpecificItemId";
            this.edtSpecificItemId.Size = new System.Drawing.Size(129, 22);
            this.edtSpecificItemId.TabIndex = 22;
            // 
            // btnTambonList
            // 
            this.btnTambonList.Location = new System.Drawing.Point(16, 491);
            this.btnTambonList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTambonList.Name = "btnTambonList";
            this.btnTambonList.Size = new System.Drawing.Size(131, 28);
            this.btnTambonList.TabIndex = 24;
            this.btnTambonList.Text = "Tambon List";
            this.btnTambonList.UseVisualStyleBackColor = true;
            this.btnTambonList.Click += new System.EventHandler(this.btnTambonList_Click);
            // 
            // btnLaoList
            // 
            this.btnLaoList.Location = new System.Drawing.Point(165, 491);
            this.btnLaoList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLaoList.Name = "btnLaoList";
            this.btnLaoList.Size = new System.Drawing.Size(123, 28);
            this.btnLaoList.TabIndex = 25;
            this.btnLaoList.Text = "LAO List";
            this.btnLaoList.UseVisualStyleBackColor = true;
            this.btnLaoList.Click += new System.EventHandler(this.btnLaoList_Click);
            // 
            // btnMap
            // 
            this.btnMap.Location = new System.Drawing.Point(16, 449);
            this.btnMap.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMap.Name = "btnMap";
            this.btnMap.Size = new System.Drawing.Size(131, 28);
            this.btnMap.TabIndex = 26;
            this.btnMap.Text = "Map";
            this.btnMap.UseVisualStyleBackColor = true;
            this.btnMap.Visible = false;
            this.btnMap.Click += new System.EventHandler(this.btnMap_Click);
            // 
            // btnAmphoeCategory
            // 
            this.btnAmphoeCategory.Enabled = false;
            this.btnAmphoeCategory.Location = new System.Drawing.Point(16, 359);
            this.btnAmphoeCategory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAmphoeCategory.Name = "btnAmphoeCategory";
            this.btnAmphoeCategory.Size = new System.Drawing.Size(137, 28);
            this.btnAmphoeCategory.TabIndex = 28;
            this.btnAmphoeCategory.Text = "Amphoe Category";
            this.btnAmphoeCategory.UseVisualStyleBackColor = true;
            this.btnAmphoeCategory.Click += new System.EventHandler(this.btnAmphoeCategory_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(165, 129);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(123, 28);
            this.btnTest.TabIndex = 29;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblLaoInfo
            // 
            this.lblLaoInfo.AutoSize = true;
            this.lblLaoInfo.Location = new System.Drawing.Point(16, 428);
            this.lblLaoInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLaoInfo.Name = "lblLaoInfo";
            this.lblLaoInfo.Size = new System.Drawing.Size(59, 17);
            this.lblLaoInfo.TabIndex = 30;
            this.lblLaoInfo.Text = "LAOInfo";
            // 
            // WikiData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 534);
            this.Controls.Add(this.lblLaoInfo);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnAmphoeCategory);
            this.Controls.Add(this.btnMap);
            this.Controls.Add(this.btnLaoList);
            this.Controls.Add(this.btnTambonList);
            this.Controls.Add(this.lblSpecificItemId);
            this.Controls.Add(this.edtSpecificItemId);
            this.Controls.Add(this.btnCreateLocalGovernment);
            this.Controls.Add(this.cbxLocalGovernments);
            this.Controls.Add(this.btnCreateTambon);
            this.Controls.Add(this.lblTambonInfo);
            this.Controls.Add(this.cbxAmphoe);
            this.Controls.Add(this.cbxChangwat);
            this.Controls.Add(this.btnAllItems);
            this.Controls.Add(this.lblStartingItemId);
            this.Controls.Add(this.edtStartingItemId);
            this.Controls.Add(this.btnCategory);
            this.Controls.Add(this.btnCheckCommonsCategory);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.chkOverride);
            this.Controls.Add(this.edtCollisions);
            this.Controls.Add(this.cbxActivity);
            this.Controls.Add(this.chkTypes);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnCountInterwiki);
            this.Controls.Add(this.btnStat);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
        private System.Windows.Forms.Button btnCheckCommonsCategory;
        private System.Windows.Forms.Button btnCategory;
        private System.Windows.Forms.TextBox edtStartingItemId;
        private System.Windows.Forms.Label lblStartingItemId;
        private System.Windows.Forms.Button btnAllItems;
        private System.Windows.Forms.ComboBox cbxChangwat;
        private System.Windows.Forms.ComboBox cbxAmphoe;
        private System.Windows.Forms.Label lblTambonInfo;
        private System.Windows.Forms.Button btnCreateTambon;
        private System.Windows.Forms.ComboBox cbxLocalGovernments;
        private System.Windows.Forms.Button btnCreateLocalGovernment;
        private System.Windows.Forms.Label lblSpecificItemId;
        private System.Windows.Forms.TextBox edtSpecificItemId;
        private System.Windows.Forms.Button btnTambonList;
        private System.Windows.Forms.Button btnLaoList;
        private System.Windows.Forms.Button btnMap;
        private System.Windows.Forms.Button btnAmphoeCategory;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblLaoInfo;
    }
}