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
            this.components = new System.ComponentModel.Container();
            this.txtSubDivisions = new System.Windows.Forms.TextBox();
            this.treeviewSelection = new System.Windows.Forms.TreeView();
            this.tabSubdivisions = new System.Windows.Forms.TabControl();
            this.tabPageCentral = new System.Windows.Forms.TabPage();
            this.listviewCentralAdministration = new System.Windows.Forms.ListView();
            this.columnEnglish = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnThai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGeocode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPopulation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageLocal = new System.Windows.Forms.TabPage();
            this.listviewLocalAdministration = new System.Windows.Forms.ListView();
            this.columnLocalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalThai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalGeocode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalDolaCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalPopulation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabSubdivisionData = new System.Windows.Forms.TabControl();
            this.tabSubdivision = new System.Windows.Forms.TabPage();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.tabElection = new System.Windows.Forms.TabPage();
            this.txtElections = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuWikipediaGerman = new System.Windows.Forms.ToolStripMenuItem();
            this.tabSubdivisions.SuspendLayout();
            this.tabPageCentral.SuspendLayout();
            this.tabPageLocal.SuspendLayout();
            this.tabSubdivisionData.SuspendLayout();
            this.tabSubdivision.SuspendLayout();
            this.tabErrors.SuspendLayout();
            this.tabElection.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSubDivisions
            // 
            this.txtSubDivisions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubDivisions.Location = new System.Drawing.Point(6, 6);
            this.txtSubDivisions.Multiline = true;
            this.txtSubDivisions.Name = "txtSubDivisions";
            this.txtSubDivisions.Size = new System.Drawing.Size(386, 120);
            this.txtSubDivisions.TabIndex = 13;
            // 
            // treeviewSelection
            // 
            this.treeviewSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeviewSelection.Location = new System.Drawing.Point(3, 3);
            this.treeviewSelection.Name = "treeviewSelection";
            this.treeviewSelection.Size = new System.Drawing.Size(192, 506);
            this.treeviewSelection.TabIndex = 11;
            this.treeviewSelection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeviewSelection_AfterSelect);
            this.treeviewSelection.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeviewSelection_MouseUp);
            // 
            // tabSubdivisions
            // 
            this.tabSubdivisions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSubdivisions.Controls.Add(this.tabPageCentral);
            this.tabSubdivisions.Controls.Add(this.tabPageLocal);
            this.tabSubdivisions.Location = new System.Drawing.Point(201, 3);
            this.tabSubdivisions.Name = "tabSubdivisions";
            this.tabSubdivisions.SelectedIndex = 0;
            this.tabSubdivisions.Size = new System.Drawing.Size(406, 342);
            this.tabSubdivisions.TabIndex = 15;
            // 
            // tabPageCentral
            // 
            this.tabPageCentral.Controls.Add(this.listviewCentralAdministration);
            this.tabPageCentral.Location = new System.Drawing.Point(4, 22);
            this.tabPageCentral.Name = "tabPageCentral";
            this.tabPageCentral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCentral.Size = new System.Drawing.Size(398, 316);
            this.tabPageCentral.TabIndex = 0;
            this.tabPageCentral.Text = "Central";
            this.tabPageCentral.UseVisualStyleBackColor = true;
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
            this.listviewCentralAdministration.Location = new System.Drawing.Point(-1, 3);
            this.listviewCentralAdministration.Name = "listviewCentralAdministration";
            this.listviewCentralAdministration.Size = new System.Drawing.Size(399, 310);
            this.listviewCentralAdministration.TabIndex = 13;
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
            // tabPageLocal
            // 
            this.tabPageLocal.Controls.Add(this.listviewLocalAdministration);
            this.tabPageLocal.Location = new System.Drawing.Point(4, 22);
            this.tabPageLocal.Name = "tabPageLocal";
            this.tabPageLocal.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLocal.Size = new System.Drawing.Size(398, 316);
            this.tabPageLocal.TabIndex = 1;
            this.tabPageLocal.Text = "Local";
            this.tabPageLocal.UseVisualStyleBackColor = true;
            // 
            // listviewLocalAdministration
            // 
            this.listviewLocalAdministration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewLocalAdministration.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLocalName,
            this.columnLocalThai,
            this.columnLocalType,
            this.columnLocalGeocode,
            this.columnLocalDolaCode,
            this.columnLocalPopulation});
            this.listviewLocalAdministration.Location = new System.Drawing.Point(-4, 6);
            this.listviewLocalAdministration.Name = "listviewLocalAdministration";
            this.listviewLocalAdministration.Size = new System.Drawing.Size(402, 310);
            this.listviewLocalAdministration.TabIndex = 15;
            this.listviewLocalAdministration.UseCompatibleStateImageBehavior = false;
            this.listviewLocalAdministration.View = System.Windows.Forms.View.Details;
            // 
            // columnLocalName
            // 
            this.columnLocalName.Text = "Name";
            this.columnLocalName.Width = 102;
            // 
            // columnLocalThai
            // 
            this.columnLocalThai.Text = "Thai";
            this.columnLocalThai.Width = 118;
            // 
            // columnLocalType
            // 
            this.columnLocalType.DisplayIndex = 3;
            this.columnLocalType.Text = "Type";
            // 
            // columnLocalGeocode
            // 
            this.columnLocalGeocode.DisplayIndex = 2;
            this.columnLocalGeocode.Text = "Geocode";
            // 
            // columnLocalDolaCode
            // 
            this.columnLocalDolaCode.Text = "DOLA code";
            // 
            // columnLocalPopulation
            // 
            this.columnLocalPopulation.Text = "Population";
            this.columnLocalPopulation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnLocalPopulation.Width = 71;
            // 
            // tabSubdivisionData
            // 
            this.tabSubdivisionData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSubdivisionData.Controls.Add(this.tabSubdivision);
            this.tabSubdivisionData.Controls.Add(this.tabErrors);
            this.tabSubdivisionData.Controls.Add(this.tabElection);
            this.tabSubdivisionData.Location = new System.Drawing.Point(201, 351);
            this.tabSubdivisionData.Name = "tabSubdivisionData";
            this.tabSubdivisionData.SelectedIndex = 0;
            this.tabSubdivisionData.Size = new System.Drawing.Size(406, 158);
            this.tabSubdivisionData.TabIndex = 16;
            // 
            // tabSubdivision
            // 
            this.tabSubdivision.Controls.Add(this.txtSubDivisions);
            this.tabSubdivision.Location = new System.Drawing.Point(4, 22);
            this.tabSubdivision.Name = "tabSubdivision";
            this.tabSubdivision.Padding = new System.Windows.Forms.Padding(3);
            this.tabSubdivision.Size = new System.Drawing.Size(398, 132);
            this.tabSubdivision.TabIndex = 0;
            this.tabSubdivision.Text = "Subdivisions";
            this.tabSubdivision.UseVisualStyleBackColor = true;
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.txtErrors);
            this.tabErrors.Location = new System.Drawing.Point(4, 22);
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tabErrors.Size = new System.Drawing.Size(398, 132);
            this.tabErrors.TabIndex = 1;
            this.tabErrors.Text = "Invalid data";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // txtErrors
            // 
            this.txtErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrors.Location = new System.Drawing.Point(6, 6);
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.Size = new System.Drawing.Size(386, 120);
            this.txtErrors.TabIndex = 14;
            // 
            // tabElection
            // 
            this.tabElection.Controls.Add(this.txtElections);
            this.tabElection.Location = new System.Drawing.Point(4, 22);
            this.tabElection.Name = "tabElection";
            this.tabElection.Padding = new System.Windows.Forms.Padding(3);
            this.tabElection.Size = new System.Drawing.Size(398, 132);
            this.tabElection.TabIndex = 2;
            this.tabElection.Text = "Election";
            this.tabElection.UseVisualStyleBackColor = true;
            // 
            // txtElections
            // 
            this.txtElections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtElections.Location = new System.Drawing.Point(6, 6);
            this.txtElections.Multiline = true;
            this.txtElections.Name = "txtElections";
            this.txtElections.Size = new System.Drawing.Size(386, 120);
            this.txtElections.TabIndex = 14;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuWikipediaGerman});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(151, 26);
            // 
            // mnuWikipediaGerman
            // 
            this.mnuWikipediaGerman.Name = "mnuWikipediaGerman";
            this.mnuWikipediaGerman.Size = new System.Drawing.Size(150, 22);
            this.mnuWikipediaGerman.Text = "Wikipedia (de)";
            this.mnuWikipediaGerman.Click += new System.EventHandler(this.mnuWikipediaGerman_Click);
            // 
            // EntityBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 515);
            this.Controls.Add(this.tabSubdivisionData);
            this.Controls.Add(this.treeviewSelection);
            this.Controls.Add(this.tabSubdivisions);
            this.Name = "EntityBrowserForm";
            this.Text = "EntityBrowserForm";
            this.Load += new System.EventHandler(this.EntityBrowserForm_Load);
            this.tabSubdivisions.ResumeLayout(false);
            this.tabPageCentral.ResumeLayout(false);
            this.tabPageLocal.ResumeLayout(false);
            this.tabSubdivisionData.ResumeLayout(false);
            this.tabSubdivision.ResumeLayout(false);
            this.tabSubdivision.PerformLayout();
            this.tabErrors.ResumeLayout(false);
            this.tabErrors.PerformLayout();
            this.tabElection.ResumeLayout(false);
            this.tabElection.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtSubDivisions;
        private System.Windows.Forms.TreeView treeviewSelection;
        private System.Windows.Forms.TabControl tabSubdivisions;
        private System.Windows.Forms.TabPage tabPageCentral;
        private System.Windows.Forms.ListView listviewCentralAdministration;
        private System.Windows.Forms.ColumnHeader columnEnglish;
        private System.Windows.Forms.ColumnHeader columnThai;
        private System.Windows.Forms.ColumnHeader columnGeocode;
        private System.Windows.Forms.ColumnHeader columnPopulation;
        private System.Windows.Forms.TabPage tabPageLocal;
        private System.Windows.Forms.ListView listviewLocalAdministration;
        private System.Windows.Forms.ColumnHeader columnLocalName;
        private System.Windows.Forms.ColumnHeader columnLocalThai;
        private System.Windows.Forms.ColumnHeader columnLocalGeocode;
        private System.Windows.Forms.ColumnHeader columnLocalDolaCode;
        private System.Windows.Forms.ColumnHeader columnLocalPopulation;
        private System.Windows.Forms.ColumnHeader columnLocalType;
        private System.Windows.Forms.TabControl tabSubdivisionData;
        private System.Windows.Forms.TabPage tabSubdivision;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.TextBox txtErrors;
        private System.Windows.Forms.TabPage tabElection;
        private System.Windows.Forms.TextBox txtElections;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuWikipediaGerman;

    }
}