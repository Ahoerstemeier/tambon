namespace De.AHoerstemeier.Tambon
{
    partial class RoyalGazetteSearch
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbxSearchKey = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkTAO = new System.Windows.Forms.CheckBox();
            this.chkTambon = new System.Windows.Forms.CheckBox();
            this.chkSukhaphiban = new System.Windows.Forms.CheckBox();
            this.chkThesaban = new System.Windows.Forms.CheckBox();
            this.chkAmphoe = new System.Windows.Forms.CheckBox();
            this.chkChangwat = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkArea = new System.Windows.Forms.CheckBox();
            this.chkStatus = new System.Windows.Forms.CheckBox();
            this.chkRename = new System.Windows.Forms.CheckBox();
            this.chkAbolishment = new System.Windows.Forms.CheckBox();
            this.chkCreation = new System.Windows.Forms.CheckBox();
            this.edtYearStart = new System.Windows.Forms.NumericUpDown();
            this.edtYearEnd = new System.Windows.Forms.NumericUpDown();
            this.cbx_AllYears = new System.Windows.Forms.CheckBox();
            this.chkTambonCouncil = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edtYearStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtYearEnd)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search text";
            // 
            // cbxSearchKey
            // 
            this.cbxSearchKey.FormattingEnabled = true;
            this.cbxSearchKey.Location = new System.Drawing.Point(9, 31);
            this.cbxSearchKey.Name = "cbxSearchKey";
            this.cbxSearchKey.Size = new System.Drawing.Size(303, 21);
            this.cbxSearchKey.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(237, 326);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkTambonCouncil);
            this.groupBox1.Controls.Add(this.chkTAO);
            this.groupBox1.Controls.Add(this.chkTambon);
            this.groupBox1.Controls.Add(this.chkSukhaphiban);
            this.groupBox1.Controls.Add(this.chkThesaban);
            this.groupBox1.Controls.Add(this.chkAmphoe);
            this.groupBox1.Controls.Add(this.chkChangwat);
            this.groupBox1.Location = new System.Drawing.Point(9, 121);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(160, 201);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Entities";
            // 
            // chkTAO
            // 
            this.chkTAO.AutoSize = true;
            this.chkTAO.Location = new System.Drawing.Point(9, 136);
            this.chkTAO.Name = "chkTAO";
            this.chkTAO.Size = new System.Drawing.Size(48, 17);
            this.chkTAO.TabIndex = 5;
            this.chkTAO.Text = "TAO";
            this.chkTAO.UseVisualStyleBackColor = true;
            // 
            // chkTambon
            // 
            this.chkTambon.AutoSize = true;
            this.chkTambon.Location = new System.Drawing.Point(9, 65);
            this.chkTambon.Name = "chkTambon";
            this.chkTambon.Size = new System.Drawing.Size(65, 17);
            this.chkTambon.TabIndex = 4;
            this.chkTambon.Text = "Tambon";
            this.chkTambon.UseVisualStyleBackColor = true;
            // 
            // chkSukhaphiban
            // 
            this.chkSukhaphiban.AutoSize = true;
            this.chkSukhaphiban.Location = new System.Drawing.Point(9, 113);
            this.chkSukhaphiban.Name = "chkSukhaphiban";
            this.chkSukhaphiban.Size = new System.Drawing.Size(89, 17);
            this.chkSukhaphiban.TabIndex = 3;
            this.chkSukhaphiban.Text = "Sukhaphiban";
            this.chkSukhaphiban.UseVisualStyleBackColor = true;
            // 
            // chkThesaban
            // 
            this.chkThesaban.AutoSize = true;
            this.chkThesaban.Location = new System.Drawing.Point(9, 90);
            this.chkThesaban.Name = "chkThesaban";
            this.chkThesaban.Size = new System.Drawing.Size(74, 17);
            this.chkThesaban.TabIndex = 2;
            this.chkThesaban.Text = "Thesaban";
            this.chkThesaban.UseVisualStyleBackColor = true;
            // 
            // chkAmphoe
            // 
            this.chkAmphoe.AutoSize = true;
            this.chkAmphoe.Location = new System.Drawing.Point(9, 42);
            this.chkAmphoe.Name = "chkAmphoe";
            this.chkAmphoe.Size = new System.Drawing.Size(133, 17);
            this.chkAmphoe.TabIndex = 1;
            this.chkAmphoe.Text = "Amphoe/King Amphoe";
            this.chkAmphoe.UseVisualStyleBackColor = true;
            // 
            // chkChangwat
            // 
            this.chkChangwat.AutoSize = true;
            this.chkChangwat.Location = new System.Drawing.Point(9, 19);
            this.chkChangwat.Name = "chkChangwat";
            this.chkChangwat.Size = new System.Drawing.Size(74, 17);
            this.chkChangwat.TabIndex = 0;
            this.chkChangwat.Text = "Changwat";
            this.chkChangwat.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkArea);
            this.groupBox2.Controls.Add(this.chkStatus);
            this.groupBox2.Controls.Add(this.chkRename);
            this.groupBox2.Controls.Add(this.chkAbolishment);
            this.groupBox2.Controls.Add(this.chkCreation);
            this.groupBox2.Location = new System.Drawing.Point(175, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 201);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Events";
            // 
            // chkArea
            // 
            this.chkArea.AutoSize = true;
            this.chkArea.Location = new System.Drawing.Point(12, 108);
            this.chkArea.Name = "chkArea";
            this.chkArea.Size = new System.Drawing.Size(87, 17);
            this.chkArea.TabIndex = 4;
            this.chkArea.Text = "Area change";
            this.chkArea.UseVisualStyleBackColor = true;
            // 
            // chkStatus
            // 
            this.chkStatus.AutoSize = true;
            this.chkStatus.Location = new System.Drawing.Point(12, 85);
            this.chkStatus.Name = "chkStatus";
            this.chkStatus.Size = new System.Drawing.Size(95, 17);
            this.chkStatus.TabIndex = 3;
            this.chkStatus.Text = "Status change";
            this.chkStatus.UseVisualStyleBackColor = true;
            // 
            // chkRename
            // 
            this.chkRename.AutoSize = true;
            this.chkRename.Location = new System.Drawing.Point(12, 62);
            this.chkRename.Name = "chkRename";
            this.chkRename.Size = new System.Drawing.Size(66, 17);
            this.chkRename.TabIndex = 2;
            this.chkRename.Text = "Rename";
            this.chkRename.UseVisualStyleBackColor = true;
            // 
            // chkAbolishment
            // 
            this.chkAbolishment.AutoSize = true;
            this.chkAbolishment.Location = new System.Drawing.Point(12, 39);
            this.chkAbolishment.Name = "chkAbolishment";
            this.chkAbolishment.Size = new System.Drawing.Size(83, 17);
            this.chkAbolishment.TabIndex = 1;
            this.chkAbolishment.Text = "Abolishment";
            this.chkAbolishment.UseVisualStyleBackColor = true;
            // 
            // chkCreation
            // 
            this.chkCreation.AutoSize = true;
            this.chkCreation.Location = new System.Drawing.Point(12, 16);
            this.chkCreation.Name = "chkCreation";
            this.chkCreation.Size = new System.Drawing.Size(65, 17);
            this.chkCreation.TabIndex = 0;
            this.chkCreation.Text = "Creation";
            this.chkCreation.UseVisualStyleBackColor = true;
            // 
            // edtYearStart
            // 
            this.edtYearStart.Location = new System.Drawing.Point(105, 83);
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
            this.edtYearStart.TabIndex = 5;
            this.edtYearStart.Value = new decimal(new int[] {
            1883,
            0,
            0,
            0});
            // 
            // edtYearEnd
            // 
            this.edtYearEnd.Location = new System.Drawing.Point(175, 83);
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
            this.edtYearEnd.Size = new System.Drawing.Size(53, 20);
            this.edtYearEnd.TabIndex = 6;
            this.edtYearEnd.Value = new decimal(new int[] {
            2008,
            0,
            0,
            0});
            // 
            // cbx_AllYears
            // 
            this.cbx_AllYears.AutoSize = true;
            this.cbx_AllYears.Location = new System.Drawing.Point(12, 84);
            this.cbx_AllYears.Name = "cbx_AllYears";
            this.cbx_AllYears.Size = new System.Drawing.Size(65, 17);
            this.cbx_AllYears.TabIndex = 7;
            this.cbx_AllYears.Text = "All years";
            this.cbx_AllYears.UseVisualStyleBackColor = true;
            this.cbx_AllYears.CheckedChanged += new System.EventHandler(this.cbx_AllYears_CheckedChanged);
            // 
            // chk_TC
            // 
            this.chkTambonCouncil.AutoSize = true;
            this.chkTambonCouncil.Location = new System.Drawing.Point(9, 159);
            this.chkTambonCouncil.Name = "chk_TC";
            this.chkTambonCouncil.Size = new System.Drawing.Size(103, 17);
            this.chkTambonCouncil.TabIndex = 5;
            this.chkTambonCouncil.Text = "Tambon Council";
            this.chkTambonCouncil.UseVisualStyleBackColor = true;
            // 
            // RoyalGazetteSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 361);
            this.Controls.Add(this.cbx_AllYears);
            this.Controls.Add(this.edtYearEnd);
            this.Controls.Add(this.edtYearStart);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbxSearchKey);
            this.Controls.Add(this.label1);
            this.Name = "RoyalGazetteSearch";
            this.Text = "Search in Royal Gazette";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edtYearStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtYearEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxSearchKey;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkTambon;
        private System.Windows.Forms.CheckBox chkSukhaphiban;
        private System.Windows.Forms.CheckBox chkThesaban;
        private System.Windows.Forms.CheckBox chkAmphoe;
        private System.Windows.Forms.CheckBox chkChangwat;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkArea;
        private System.Windows.Forms.CheckBox chkStatus;
        private System.Windows.Forms.CheckBox chkRename;
        private System.Windows.Forms.CheckBox chkAbolishment;
        private System.Windows.Forms.CheckBox chkCreation;
        private System.Windows.Forms.NumericUpDown edtYearStart;
        private System.Windows.Forms.NumericUpDown edtYearEnd;
        private System.Windows.Forms.CheckBox cbx_AllYears;
        private System.Windows.Forms.CheckBox chkTAO;
        private System.Windows.Forms.CheckBox chkTambonCouncil;
    }
}