namespace De.AHoerstemeier.Tambon.UI
{
    partial class EntityBrowserForm
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
            this.txtSubDivisions = new System.Windows.Forms.TextBox();
            this.listviewCentralAdministration = new System.Windows.Forms.ListView();
            this.columnEnglish = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnThai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGeocode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPopulation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeviewSelection = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // txtSubDivisions
            // 
            this.txtSubDivisions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubDivisions.Location = new System.Drawing.Point(201, 314);
            this.txtSubDivisions.Multiline = true;
            this.txtSubDivisions.Name = "txtSubDivisions";
            this.txtSubDivisions.Size = new System.Drawing.Size(406, 195);
            this.txtSubDivisions.TabIndex = 13;
            // 
            // listviewCentralAdministration
            // 
            this.listviewCentralAdministration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewCentralAdministration.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEnglish,
            this.columnThai,
            this.columnGeocode,
            this.columnPopulation});
            this.listviewCentralAdministration.Location = new System.Drawing.Point(201, 3);
            this.listviewCentralAdministration.Name = "listviewCentralAdministration";
            this.listviewCentralAdministration.Size = new System.Drawing.Size(406, 305);
            this.listviewCentralAdministration.TabIndex = 12;
            this.listviewCentralAdministration.UseCompatibleStateImageBehavior = false;
            this.listviewCentralAdministration.View = System.Windows.Forms.View.Details;
            // 
            // columnEnglish
            // 
            this.columnEnglish.Text = "Name";
            this.columnEnglish.Width = 102;
            // 
            // columnThai
            // 
            this.columnThai.Text = "Thai";
            this.columnThai.Width = 118;
            // 
            // columnGeocode
            // 
            this.columnGeocode.Text = "Geocode";
            // 
            // columnPopulation
            // 
            this.columnPopulation.Text = "Population";
            this.columnPopulation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPopulation.Width = 71;
            // 
            // treeviewSelection
            // 
            this.treeviewSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeviewSelection.Location = new System.Drawing.Point(3, 3);
            this.treeviewSelection.Name = "treeviewSelection";
            this.treeviewSelection.Size = new System.Drawing.Size(192, 506);
            this.treeviewSelection.TabIndex = 11;
            this.treeviewSelection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeviewSelection_AfterSelect);
            // 
            // EntityBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 515);
            this.Controls.Add(this.txtSubDivisions);
            this.Controls.Add(this.listviewCentralAdministration);
            this.Controls.Add(this.treeviewSelection);
            this.Name = "EntityBrowserForm";
            this.Text = "EntityBrowserForm";
            this.Load += new System.EventHandler(this.EntityBrowserForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSubDivisions;
        private System.Windows.Forms.ListView listviewCentralAdministration;
        private System.Windows.Forms.ColumnHeader columnEnglish;
        private System.Windows.Forms.ColumnHeader columnThai;
        private System.Windows.Forms.ColumnHeader columnGeocode;
        private System.Windows.Forms.TreeView treeviewSelection;
        private System.Windows.Forms.ColumnHeader columnPopulation;

    }
}