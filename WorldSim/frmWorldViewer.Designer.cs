namespace WorldSim
{
    partial class frmWorldViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWorldViewer));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditGo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditStop = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditStep = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAddObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditDeploy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditWatcher = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdMain = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.sfdMain = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSimulation = new System.Windows.Forms.TabPage();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.lstLeaders = new System.Windows.Forms.ListBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.tabSelected = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.coverageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showResourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPolledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom20ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom50ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom75ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_world = new WorldSim.WorldControl();
            this.mnuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSimulation.SuspendLayout();
            this.tabSelected.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.toolStripMenuItem1,
            this.mnuHelp});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(653, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.toolStripSeparator1,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.toolStripSeparator3,
            this.mnuFilePrint,
            this.toolStripSeparator4,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileNew.Image")));
            this.mnuFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.Size = new System.Drawing.Size(140, 22);
            this.mnuFileNew.Text = "&New";
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileOpen.Image")));
            this.mnuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Size = new System.Drawing.Size(140, 22);
            this.mnuFileOpen.Text = "&Open";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(137, 6);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileSave.Image")));
            this.mnuFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.Size = new System.Drawing.Size(140, 22);
            this.mnuFileSave.Text = "&Save";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            this.mnuFileSaveAs.Size = new System.Drawing.Size(140, 22);
            this.mnuFileSaveAs.Text = "Save &As";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(137, 6);
            // 
            // mnuFilePrint
            // 
            this.mnuFilePrint.Enabled = false;
            this.mnuFilePrint.Image = ((System.Drawing.Image)(resources.GetObject("mnuFilePrint.Image")));
            this.mnuFilePrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFilePrint.Name = "mnuFilePrint";
            this.mnuFilePrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuFilePrint.Size = new System.Drawing.Size(140, 22);
            this.mnuFilePrint.Text = "&Print";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(137, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(140, 22);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator5,
            this.mnuEditGo,
            this.mnuEditStop,
            this.mnuEditStep,
            this.toolStripSeparator2,
            this.mnuAddObjects,
            this.mnuEditDeploy,
            this.mnuEditWatcher});
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(39, 20);
            this.mnuEdit.Text = "&Edit";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(146, 6);
            // 
            // mnuEditGo
            // 
            this.mnuEditGo.Name = "mnuEditGo";
            this.mnuEditGo.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuEditGo.Size = new System.Drawing.Size(149, 22);
            this.mnuEditGo.Text = "&Go";
            this.mnuEditGo.Click += new System.EventHandler(this.mnuEditGo_Click);
            // 
            // mnuEditStop
            // 
            this.mnuEditStop.Name = "mnuEditStop";
            this.mnuEditStop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.mnuEditStop.Size = new System.Drawing.Size(149, 22);
            this.mnuEditStop.Text = "&Stop";
            this.mnuEditStop.Click += new System.EventHandler(this.mnuEditStop_Click);
            // 
            // mnuEditStep
            // 
            this.mnuEditStep.Name = "mnuEditStep";
            this.mnuEditStep.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.mnuEditStep.Size = new System.Drawing.Size(149, 22);
            this.mnuEditStep.Text = "S&tep";
            this.mnuEditStep.Click += new System.EventHandler(this.mnuEditStep_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(146, 6);
            // 
            // mnuAddObjects
            // 
            this.mnuAddObjects.Name = "mnuAddObjects";
            this.mnuAddObjects.Size = new System.Drawing.Size(149, 22);
            this.mnuAddObjects.Text = "&Add Objects...";
            this.mnuAddObjects.Click += new System.EventHandler(this.mnuAddObjects_Click);
            // 
            // mnuEditDeploy
            // 
            this.mnuEditDeploy.Name = "mnuEditDeploy";
            this.mnuEditDeploy.Size = new System.Drawing.Size(149, 22);
            this.mnuEditDeploy.Text = "&Deploy";
            this.mnuEditDeploy.Click += new System.EventHandler(this.mnuEditDeploy_Click);
            // 
            // mnuEditWatcher
            // 
            this.mnuEditWatcher.Name = "mnuEditWatcher";
            this.mnuEditWatcher.Size = new System.Drawing.Size(149, 22);
            this.mnuEditWatcher.Text = "&Watcher";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.coverageToolStripMenuItem,
            this.zoomToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem1.Text = "&View";
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(152, 22);
            this.mnuHelpAbout.Text = "&About...";
            // 
            // ofdMain
            // 
            this.ofdMain.FileName = "openFileDialog1";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.m_world);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(653, 436);
            this.splitContainer1.SplitterDistance = 426;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSimulation);
            this.tabControl1.Controls.Add(this.tabSelected);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(223, 436);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSimulation
            // 
            this.tabSimulation.Controls.Add(this.progressBar1);
            this.tabSimulation.Controls.Add(this.btnSaveLog);
            this.tabSimulation.Controls.Add(this.btnQuery);
            this.tabSimulation.Controls.Add(this.lstLeaders);
            this.tabSimulation.Controls.Add(this.btnPlay);
            this.tabSimulation.Location = new System.Drawing.Point(4, 22);
            this.tabSimulation.Name = "tabSimulation";
            this.tabSimulation.Padding = new System.Windows.Forms.Padding(3);
            this.tabSimulation.Size = new System.Drawing.Size(215, 410);
            this.tabSimulation.TabIndex = 0;
            this.tabSimulation.Text = "Simulation";
            this.tabSimulation.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(147, 7);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(60, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Location = new System.Drawing.Point(65, 7);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(75, 23);
            this.btnSaveLog.TabIndex = 3;
            this.btnSaveLog.Text = "Save";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(36, 7);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(23, 23);
            this.btnQuery.TabIndex = 2;
            this.btnQuery.Text = "?";
            this.btnQuery.UseVisualStyleBackColor = true;
            // 
            // lstLeaders
            // 
            this.lstLeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLeaders.Font = new System.Drawing.Font("SimSun", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstLeaders.FormattingEnabled = true;
            this.lstLeaders.IntegralHeight = false;
            this.lstLeaders.ItemHeight = 11;
            this.lstLeaders.Location = new System.Drawing.Point(7, 36);
            this.lstLeaders.Name = "lstLeaders";
            this.lstLeaders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstLeaders.Size = new System.Drawing.Size(202, 366);
            this.lstLeaders.TabIndex = 1;
            // 
            // btnPlay
            // 
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.Location = new System.Drawing.Point(7, 7);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(23, 23);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.mnuEditGo_Click);
            // 
            // tabSelected
            // 
            this.tabSelected.Controls.Add(this.propertyGrid1);
            this.tabSelected.Location = new System.Drawing.Point(4, 22);
            this.tabSelected.Name = "tabSelected";
            this.tabSelected.Padding = new System.Windows.Forms.Padding(3);
            this.tabSelected.Size = new System.Drawing.Size(215, 410);
            this.tabSelected.TabIndex = 1;
            this.tabSelected.Text = "Selected";
            this.tabSelected.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(209, 404);
            this.propertyGrid1.TabIndex = 0;
            // 
            // coverageToolStripMenuItem
            // 
            this.coverageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showResourcesToolStripMenuItem,
            this.showStaleToolStripMenuItem,
            this.showPolledToolStripMenuItem,
            this.showNoneToolStripMenuItem});
            this.coverageToolStripMenuItem.Name = "coverageToolStripMenuItem";
            this.coverageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.coverageToolStripMenuItem.Text = "&Background";
            // 
            // showResourcesToolStripMenuItem
            // 
            this.showResourcesToolStripMenuItem.Name = "showResourcesToolStripMenuItem";
            this.showResourcesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showResourcesToolStripMenuItem.Text = "Show &Resources";
            this.showResourcesToolStripMenuItem.Click += new System.EventHandler(this.showCoverageToolStripMenuItem_Click);
            // 
            // showStaleToolStripMenuItem
            // 
            this.showStaleToolStripMenuItem.CheckOnClick = true;
            this.showStaleToolStripMenuItem.Name = "showStaleToolStripMenuItem";
            this.showStaleToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showStaleToolStripMenuItem.Text = "Show &Stale";
            this.showStaleToolStripMenuItem.Click += new System.EventHandler(this.showCoverageToolStripMenuItem_Click);
            // 
            // showPolledToolStripMenuItem
            // 
            this.showPolledToolStripMenuItem.Name = "showPolledToolStripMenuItem";
            this.showPolledToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showPolledToolStripMenuItem.Text = "Show &Polled";
            this.showPolledToolStripMenuItem.Click += new System.EventHandler(this.showCoverageToolStripMenuItem_Click);
            // 
            // showNoneToolStripMenuItem
            // 
            this.showNoneToolStripMenuItem.Checked = true;
            this.showNoneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showNoneToolStripMenuItem.Name = "showNoneToolStripMenuItem";
            this.showNoneToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showNoneToolStripMenuItem.Text = "Show &None";
            this.showNoneToolStripMenuItem.Click += new System.EventHandler(this.showCoverageToolStripMenuItem_Click);
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoom10ToolStripMenuItem,
            this.zoom20ToolStripMenuItem,
            this.zoom50ToolStripMenuItem,
            this.zoom75ToolStripMenuItem,
            this.zoom100ToolStripMenuItem});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoomToolStripMenuItem.Text = "&Zoom";
            // 
            // zoom10ToolStripMenuItem
            // 
            this.zoom10ToolStripMenuItem.Name = "zoom10ToolStripMenuItem";
            this.zoom10ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoom10ToolStripMenuItem.Text = "Zoom &10%";
            this.zoom10ToolStripMenuItem.Click += new System.EventHandler(this.showZoomToolStripMenuItem_Click);
            // 
            // zoom20ToolStripMenuItem
            // 
            this.zoom20ToolStripMenuItem.Name = "zoom20ToolStripMenuItem";
            this.zoom20ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoom20ToolStripMenuItem.Text = "Zoom &25%";
            this.zoom20ToolStripMenuItem.Click += new System.EventHandler(this.showZoomToolStripMenuItem_Click);
            // 
            // zoom50ToolStripMenuItem
            // 
            this.zoom50ToolStripMenuItem.Name = "zoom50ToolStripMenuItem";
            this.zoom50ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoom50ToolStripMenuItem.Text = "Zoom &50%";
            this.zoom50ToolStripMenuItem.Click += new System.EventHandler(this.showZoomToolStripMenuItem_Click);
            // 
            // zoom75ToolStripMenuItem
            // 
            this.zoom75ToolStripMenuItem.Name = "zoom75ToolStripMenuItem";
            this.zoom75ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoom75ToolStripMenuItem.Text = "Zoom &75%";
            this.zoom75ToolStripMenuItem.Click += new System.EventHandler(this.showZoomToolStripMenuItem_Click);
            // 
            // zoom100ToolStripMenuItem
            // 
            this.zoom100ToolStripMenuItem.Name = "zoom100ToolStripMenuItem";
            this.zoom100ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoom100ToolStripMenuItem.Text = "Zoom 1&00%";
            this.zoom100ToolStripMenuItem.Click += new System.EventHandler(this.showZoomToolStripMenuItem_Click);
            // 
            // m_world
            // 
            this.m_world.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_world.Location = new System.Drawing.Point(12, 13);
            this.m_world.MinimumSize = new System.Drawing.Size(100, 100);
            this.m_world.Name = "m_world";
            this.m_world.Origin = new System.Drawing.Point(0, 0);
            this.m_world.RewardScaleInhabitantProximity = 0D;
            this.m_world.RewardScaleP_e = 1D;
            this.m_world.RewardScaleP_s = 1D;
            this.m_world.Size = new System.Drawing.Size(411, 411);
            this.m_world.TabIndex = 0;
            this.m_world.TileHeight = 100;
            this.m_world.TilesHeight = 10;
            this.m_world.TilesWidth = 10;
            this.m_world.TileWidth = 100;
            this.m_world.Trial = 0;
            this.m_world.Zoom = 0.75F;
            this.m_world.ObjectSelectedEvent += new WorldSim.WorldControl.ObjectSelectedEventDelegate(this.m_world_ObjectSelectedEvent);
            // 
            // frmWorldViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 460);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mnuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.Name = "frmWorldViewer";
            this.Text = "World Simulator";
            this.Load += new System.EventHandler(this.frmWorldViewer_Load);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.zoomWheel);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabSimulation.ResumeLayout(false);
            this.tabSelected.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.OpenFileDialog ofdMain;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private WorldControl m_world;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSimulation;
        private System.Windows.Forms.TabPage tabSelected;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mnuEditGo;
        private System.Windows.Forms.ToolStripMenuItem mnuEditStop;
        private System.Windows.Forms.ToolStripMenuItem mnuEditStep;
        private System.Windows.Forms.ListBox lstLeaders;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnSaveLog;
        private System.Windows.Forms.SaveFileDialog sfdMain;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuAddObjects;
        private System.Windows.Forms.ToolStripMenuItem mnuEditDeploy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuEditWatcher;
        private System.Windows.Forms.ToolStripMenuItem coverageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showResourcesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPolledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoom10ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoom20ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoom50ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoom75ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoom100ToolStripMenuItem;
    }
}

