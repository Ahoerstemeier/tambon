﻿namespace De.AHoerstemeier.Tambon.UI
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
            this.popupTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuWikipediaGerman = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWikipediaEnglish = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWikipediaTambonEnglish = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMubanDefinitions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConstituency = new System.Windows.Forms.ToolStripMenuItem();
            this.tabSubdivisions = new System.Windows.Forms.TabControl();
            this.tabPageCentral = new System.Windows.Forms.TabPage();
            this.listviewCentralAdministration = new System.Windows.Forms.ListView();
            this.columnEnglish = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnThai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGeocode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPopulation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnCreation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.popupListviewCentral = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuHistoryCentral = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageLocal = new System.Windows.Forms.TabPage();
            this.listviewLocalAdministration = new System.Windows.Forms.ListView();
            this.columnLocalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalThai = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalGeocode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalPopulation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalCreation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalMayorTerm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalCouncilTerm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocalDolaCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.popupListviewLocal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuHistoryLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdminInfoPage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGeneralInfoPage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGoogleSearchLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWikidataLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWebsite = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.tabSubdivisionData = new System.Windows.Forms.TabControl();
            this.tabSubdivision = new System.Windows.Forms.TabPage();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.tabElection = new System.Windows.Forms.TabPage();
            this.txtElections = new System.Windows.Forms.TextBox();
            this.tabMuban = new System.Windows.Forms.TabPage();
            this.txtMuban = new System.Windows.Forms.TextBox();
            this.tabLocalGoverment = new System.Windows.Forms.TabPage();
            this.txtLocalGovernment = new System.Windows.Forms.TextBox();
            this.tabConstituency = new System.Windows.Forms.TabPage();
            this.txtConstituency = new System.Windows.Forms.TextBox();
            this.mnuWikidataCentral = new System.Windows.Forms.ToolStripMenuItem();
            this.popupTree.SuspendLayout();
            this.tabSubdivisions.SuspendLayout();
            this.tabPageCentral.SuspendLayout();
            this.popupListviewCentral.SuspendLayout();
            this.tabPageLocal.SuspendLayout();
            this.popupListviewLocal.SuspendLayout();
            this.tabSubdivisionData.SuspendLayout();
            this.tabSubdivision.SuspendLayout();
            this.tabErrors.SuspendLayout();
            this.tabElection.SuspendLayout();
            this.tabMuban.SuspendLayout();
            this.tabLocalGoverment.SuspendLayout();
            this.tabConstituency.SuspendLayout();
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
            this.treeviewSelection.ContextMenuStrip = this.popupTree;
            this.treeviewSelection.Location = new System.Drawing.Point(3, 3);
            this.treeviewSelection.Name = "treeviewSelection";
            this.treeviewSelection.Size = new System.Drawing.Size(192, 506);
            this.treeviewSelection.TabIndex = 11;
            this.treeviewSelection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeviewSelection_AfterSelect);
            this.treeviewSelection.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeviewSelection_MouseUp);
            // 
            // popupTree
            // 
            this.popupTree.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.popupTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuWikipediaGerman,
            this.mnuWikipediaEnglish,
            this.mnuWikipediaTambonEnglish,
            this.mnuMubanDefinitions,
            this.mnuHistory,
            this.mnuConstituency});
            this.popupTree.Name = "contextMenuStrip1";
            this.popupTree.Size = new System.Drawing.Size(174, 136);
            // 
            // mnuWikipediaGerman
            // 
            this.mnuWikipediaGerman.Name = "mnuWikipediaGerman";
            this.mnuWikipediaGerman.Size = new System.Drawing.Size(173, 22);
            this.mnuWikipediaGerman.Text = "Wikipedia (de)";
            this.mnuWikipediaGerman.Click += new System.EventHandler(this.mnuWikipediaGerman_Click);
            // 
            // mnuWikipediaEnglish
            // 
            this.mnuWikipediaEnglish.Name = "mnuWikipediaEnglish";
            this.mnuWikipediaEnglish.Size = new System.Drawing.Size(173, 22);
            this.mnuWikipediaEnglish.Text = "Wikipedia (en)";
            this.mnuWikipediaEnglish.Click += new System.EventHandler(this.mnuWikipediaEnglish_Click);
            // 
            // mnuWikipediaTambonEnglish
            // 
            this.mnuWikipediaTambonEnglish.Name = "mnuWikipediaTambonEnglish";
            this.mnuWikipediaTambonEnglish.Size = new System.Drawing.Size(173, 22);
            this.mnuWikipediaTambonEnglish.Text = "Tambon WP article";
            this.mnuWikipediaTambonEnglish.Click += new System.EventHandler(this.mnuWikipediaTambonEnglish_Click);
            // 
            // mnuMubanDefinitions
            // 
            this.mnuMubanDefinitions.Name = "mnuMubanDefinitions";
            this.mnuMubanDefinitions.Size = new System.Drawing.Size(173, 22);
            this.mnuMubanDefinitions.Text = "Muban definitions";
            this.mnuMubanDefinitions.Click += new System.EventHandler(this.mnuMubanDefinitions_Click);
            // 
            // mnuHistory
            // 
            this.mnuHistory.Name = "mnuHistory";
            this.mnuHistory.Size = new System.Drawing.Size(173, 22);
            this.mnuHistory.Text = "History XML";
            this.mnuHistory.Click += new System.EventHandler(this.mnuHistory_Click);
            // 
            // mnuConstituency
            // 
            this.mnuConstituency.Name = "mnuConstituency";
            this.mnuConstituency.Size = new System.Drawing.Size(173, 22);
            this.mnuConstituency.Text = "Constituency";
            this.mnuConstituency.Click += new System.EventHandler(this.mnuConstituency_Click);
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
            this.tabPageCentral.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
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
            this.columnPopulation,
            this.columnCreation});
            this.listviewCentralAdministration.ContextMenuStrip = this.popupListviewCentral;
            this.listviewCentralAdministration.HideSelection = false;
            this.listviewCentralAdministration.Location = new System.Drawing.Point(-1, 3);
            this.listviewCentralAdministration.MultiSelect = false;
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
            // columnCreation
            // 
            this.columnCreation.Text = "Creation";
            // 
            // popupListviewCentral
            // 
            this.popupListviewCentral.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.popupListviewCentral.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHistoryCentral,
            this.mnuWikidataCentral});
            this.popupListviewCentral.Name = "popupListviewCentral";
            this.popupListviewCentral.Size = new System.Drawing.Size(181, 70);
            this.popupListviewCentral.Opening += new System.ComponentModel.CancelEventHandler(this.popupListviewCentral_Opening);
            // 
            // mnuHistoryCentral
            // 
            this.mnuHistoryCentral.Name = "mnuHistoryCentral";
            this.mnuHistoryCentral.Size = new System.Drawing.Size(180, 22);
            this.mnuHistoryCentral.Text = "History";
            this.mnuHistoryCentral.Click += new System.EventHandler(this.mnuHistoryCentral_Click);
            // 
            // tabPageLocal
            // 
            this.tabPageLocal.Controls.Add(this.listviewLocalAdministration);
            this.tabPageLocal.Location = new System.Drawing.Point(4, 22);
            this.tabPageLocal.Name = "tabPageLocal";
            this.tabPageLocal.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
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
            this.columnLocalPopulation,
            this.columnLocalCreation,
            this.columnLocalMayorTerm,
            this.columnLocalCouncilTerm,
            this.columnLocalDolaCode});
            this.listviewLocalAdministration.ContextMenuStrip = this.popupListviewLocal;
            this.listviewLocalAdministration.HideSelection = false;
            this.listviewLocalAdministration.Location = new System.Drawing.Point(-4, 6);
            this.listviewLocalAdministration.MultiSelect = false;
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
            this.columnLocalType.Text = "Type";
            // 
            // columnLocalGeocode
            // 
            this.columnLocalGeocode.Text = "Geocode";
            // 
            // columnLocalPopulation
            // 
            this.columnLocalPopulation.Text = "Population";
            this.columnLocalPopulation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnLocalPopulation.Width = 71;
            // 
            // columnLocalCreation
            // 
            this.columnLocalCreation.Text = "Creation";
            this.columnLocalCreation.Width = 80;
            // 
            // columnLocalMayorTerm
            // 
            this.columnLocalMayorTerm.Text = "Term mayor";
            this.columnLocalMayorTerm.Width = 80;
            // 
            // columnLocalCouncilTerm
            // 
            this.columnLocalCouncilTerm.Text = "Council";
            this.columnLocalCouncilTerm.Width = 80;
            // 
            // columnLocalDolaCode
            // 
            this.columnLocalDolaCode.Text = "DOLA code";
            // 
            // popupListviewLocal
            // 
            this.popupListviewLocal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.popupListviewLocal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHistoryLocal,
            this.mnuAdminInfoPage,
            this.mnuGeneralInfoPage,
            this.mnuGoogleSearchLocal,
            this.mnuWikidataLocal,
            this.mnuWebsite,
            this.mnuLocation});
            this.popupListviewLocal.Name = "popupListviewLocal";
            this.popupListviewLocal.Size = new System.Drawing.Size(168, 158);
            this.popupListviewLocal.Opening += new System.ComponentModel.CancelEventHandler(this.popupListviewLocal_Opening);
            // 
            // mnuHistoryLocal
            // 
            this.mnuHistoryLocal.Name = "mnuHistoryLocal";
            this.mnuHistoryLocal.Size = new System.Drawing.Size(167, 22);
            this.mnuHistoryLocal.Text = "History";
            this.mnuHistoryLocal.Click += new System.EventHandler(this.mnuHistoryLocal_Click);
            // 
            // mnuAdminInfoPage
            // 
            this.mnuAdminInfoPage.Name = "mnuAdminInfoPage";
            this.mnuAdminInfoPage.Size = new System.Drawing.Size(167, 22);
            this.mnuAdminInfoPage.Text = "Admin info page";
            this.mnuAdminInfoPage.Click += new System.EventHandler(this.mnuAdminInfoPage_Click);
            // 
            // mnuGeneralInfoPage
            // 
            this.mnuGeneralInfoPage.Name = "mnuGeneralInfoPage";
            this.mnuGeneralInfoPage.Size = new System.Drawing.Size(167, 22);
            this.mnuGeneralInfoPage.Text = "General info page";
            this.mnuGeneralInfoPage.Click += new System.EventHandler(this.mnuGeneralInfoPage_Click);
            // 
            // mnuGoogleSearchLocal
            // 
            this.mnuGoogleSearchLocal.Name = "mnuGoogleSearchLocal";
            this.mnuGoogleSearchLocal.Size = new System.Drawing.Size(167, 22);
            this.mnuGoogleSearchLocal.Text = "Google search";
            this.mnuGoogleSearchLocal.Click += new System.EventHandler(this.mnuGoogleSearchLocal_Click);
            // 
            // mnuWikidataLocal
            // 
            this.mnuWikidataLocal.Name = "mnuWikidataLocal";
            this.mnuWikidataLocal.Size = new System.Drawing.Size(167, 22);
            this.mnuWikidataLocal.Text = "Wikidata";
            this.mnuWikidataLocal.Click += new System.EventHandler(this.mnuWikidataLocal_Click);
            // 
            // mnuWebsite
            // 
            this.mnuWebsite.Name = "mnuWebsite";
            this.mnuWebsite.Size = new System.Drawing.Size(167, 22);
            this.mnuWebsite.Text = "Website";
            this.mnuWebsite.Click += new System.EventHandler(this.mnuWebsite_Click);
            // 
            // mnuLocation
            // 
            this.mnuLocation.Name = "mnuLocation";
            this.mnuLocation.Size = new System.Drawing.Size(167, 22);
            this.mnuLocation.Text = "Location";
            this.mnuLocation.Click += new System.EventHandler(this.mnuLocation_Click);
            // 
            // tabSubdivisionData
            // 
            this.tabSubdivisionData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSubdivisionData.Controls.Add(this.tabSubdivision);
            this.tabSubdivisionData.Controls.Add(this.tabErrors);
            this.tabSubdivisionData.Controls.Add(this.tabElection);
            this.tabSubdivisionData.Controls.Add(this.tabMuban);
            this.tabSubdivisionData.Controls.Add(this.tabLocalGoverment);
            this.tabSubdivisionData.Controls.Add(this.tabConstituency);
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
            this.tabSubdivision.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
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
            this.tabErrors.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
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
            this.tabElection.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
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
            // tabMuban
            // 
            this.tabMuban.Controls.Add(this.txtMuban);
            this.tabMuban.Location = new System.Drawing.Point(4, 22);
            this.tabMuban.Name = "tabMuban";
            this.tabMuban.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabMuban.Size = new System.Drawing.Size(398, 132);
            this.tabMuban.TabIndex = 3;
            this.tabMuban.Text = "Muban";
            this.tabMuban.UseVisualStyleBackColor = true;
            // 
            // txtMuban
            // 
            this.txtMuban.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMuban.Location = new System.Drawing.Point(6, 6);
            this.txtMuban.Multiline = true;
            this.txtMuban.Name = "txtMuban";
            this.txtMuban.Size = new System.Drawing.Size(386, 120);
            this.txtMuban.TabIndex = 15;
            // 
            // tabLocalGoverment
            // 
            this.tabLocalGoverment.Controls.Add(this.txtLocalGovernment);
            this.tabLocalGoverment.Location = new System.Drawing.Point(4, 22);
            this.tabLocalGoverment.Name = "tabLocalGoverment";
            this.tabLocalGoverment.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabLocalGoverment.Size = new System.Drawing.Size(398, 132);
            this.tabLocalGoverment.TabIndex = 4;
            this.tabLocalGoverment.Text = "LAO";
            this.tabLocalGoverment.UseVisualStyleBackColor = true;
            // 
            // txtLocalGovernment
            // 
            this.txtLocalGovernment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalGovernment.Location = new System.Drawing.Point(6, 6);
            this.txtLocalGovernment.Multiline = true;
            this.txtLocalGovernment.Name = "txtLocalGovernment";
            this.txtLocalGovernment.Size = new System.Drawing.Size(386, 120);
            this.txtLocalGovernment.TabIndex = 16;
            // 
            // tabConstituency
            // 
            this.tabConstituency.Controls.Add(this.txtConstituency);
            this.tabConstituency.Location = new System.Drawing.Point(4, 22);
            this.tabConstituency.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabConstituency.Name = "tabConstituency";
            this.tabConstituency.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabConstituency.Size = new System.Drawing.Size(398, 132);
            this.tabConstituency.TabIndex = 5;
            this.tabConstituency.Text = "Constituency";
            this.tabConstituency.UseVisualStyleBackColor = true;
            // 
            // txtConstituency
            // 
            this.txtConstituency.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConstituency.Location = new System.Drawing.Point(8, 7);
            this.txtConstituency.Multiline = true;
            this.txtConstituency.Name = "txtConstituency";
            this.txtConstituency.Size = new System.Drawing.Size(386, 120);
            this.txtConstituency.TabIndex = 17;
            // 
            // mnuWikidataCentral
            // 
            this.mnuWikidataCentral.Name = "mnuWikidataCentral";
            this.mnuWikidataCentral.Size = new System.Drawing.Size(180, 22);
            this.mnuWikidataCentral.Text = "Wikidata";
            this.mnuWikidataCentral.Click += new System.EventHandler(this.mnuWikidataCentral_Click);
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
            this.popupTree.ResumeLayout(false);
            this.tabSubdivisions.ResumeLayout(false);
            this.tabPageCentral.ResumeLayout(false);
            this.popupListviewCentral.ResumeLayout(false);
            this.tabPageLocal.ResumeLayout(false);
            this.popupListviewLocal.ResumeLayout(false);
            this.tabSubdivisionData.ResumeLayout(false);
            this.tabSubdivision.ResumeLayout(false);
            this.tabSubdivision.PerformLayout();
            this.tabErrors.ResumeLayout(false);
            this.tabErrors.PerformLayout();
            this.tabElection.ResumeLayout(false);
            this.tabElection.PerformLayout();
            this.tabMuban.ResumeLayout(false);
            this.tabMuban.PerformLayout();
            this.tabLocalGoverment.ResumeLayout(false);
            this.tabLocalGoverment.PerformLayout();
            this.tabConstituency.ResumeLayout(false);
            this.tabConstituency.PerformLayout();
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
        private System.Windows.Forms.ContextMenuStrip popupTree;
        private System.Windows.Forms.ToolStripMenuItem mnuWikipediaGerman;
        private System.Windows.Forms.TabPage tabMuban;
        private System.Windows.Forms.TextBox txtMuban;
        private System.Windows.Forms.ToolStripMenuItem mnuMubanDefinitions;
        private System.Windows.Forms.ToolStripMenuItem mnuWikipediaEnglish;
        private System.Windows.Forms.TabPage tabLocalGoverment;
        private System.Windows.Forms.TextBox txtLocalGovernment;
        private System.Windows.Forms.TabPage tabConstituency;
        private System.Windows.Forms.TextBox txtConstituency;
        private System.Windows.Forms.ToolStripMenuItem mnuWikipediaTambonEnglish;
        private System.Windows.Forms.ToolStripMenuItem mnuHistory;
        private System.Windows.Forms.ContextMenuStrip popupListviewCentral;
        private System.Windows.Forms.ToolStripMenuItem mnuHistoryCentral;
        private System.Windows.Forms.ContextMenuStrip popupListviewLocal;
        private System.Windows.Forms.ToolStripMenuItem mnuHistoryLocal;
        private System.Windows.Forms.ColumnHeader columnCreation;
        private System.Windows.Forms.ColumnHeader columnLocalCreation;
        private System.Windows.Forms.ToolStripMenuItem mnuAdminInfoPage;
        private System.Windows.Forms.ToolStripMenuItem mnuGeneralInfoPage;
        private System.Windows.Forms.ToolStripMenuItem mnuGoogleSearchLocal;
        private System.Windows.Forms.ToolStripMenuItem mnuConstituency;
        private System.Windows.Forms.ToolStripMenuItem mnuWikidataLocal;
        private System.Windows.Forms.ToolStripMenuItem mnuWebsite;
        private System.Windows.Forms.ColumnHeader columnLocalMayorTerm;
        private System.Windows.Forms.ColumnHeader columnLocalCouncilTerm;
        private System.Windows.Forms.ToolStripMenuItem mnuLocation;
        private System.Windows.Forms.ToolStripMenuItem mnuWikidataCentral;
    }
}