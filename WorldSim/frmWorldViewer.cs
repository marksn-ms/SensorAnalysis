using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using WorldSim.Interface;

namespace WorldSim
{
    /// <summary>
    /// This class represents the main form for the application.
    /// </summary>
    public partial class frmWorldViewer : Form
    {
        private TestSettings m_testSettings;
        //private Random m_random;

        private int m_nTicks; // counts how many ticks have elapsed in current simulation
        private int m_nRepeat; // counts how many test runs have elapsed in current simulation
        private bool m_bGoOnStart; // start test run on startup, after form initialized
        private bool m_bExitOnFinish; // exit program following test run completion
        private string m_strLogFolder;       // folder in which to store log files
        private string m_strConfigFilename;

        private Dictionary<string, Type> m_agentTypes;
        private Dictionary<string, Type> m_deployerTypes;
        private Dictionary<string, Type> m_watcherTypes;
        private List<Watcher> m_watcherActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="frmWorldViewer"/> class.
        /// </summary>
        public frmWorldViewer()
        {
            //m_random = new Random();
            InitializeComponent();
            m_nTicks = 0;
            m_nRepeat = 0;
            m_testSettings = new TestSettings();

            m_agentTypes = ConfigurationManager.GetSection("Agents") as Dictionary<string, Type>;
            if (m_agentTypes == null)
                m_agentTypes = new Dictionary<string, Type>();
            //m_agentTypes.Add("Static", typeof(WorldSim.Interface.Inhabitant));
            //m_agentTypes.Add("Random", typeof(WorldSim.Interface.RandomInhabitant));

            m_watcherActive = new List<Watcher>();
            m_watcherTypes = ConfigurationManager.GetSection("Watchers") as Dictionary<string, Type>;
            if (m_watcherTypes == null)
                m_watcherTypes = new Dictionary<string, Type>();
            
            m_deployerTypes = ConfigurationManager.GetSection("Deployers") as Dictionary<string, Type>;
            if (m_deployerTypes == null)
                m_deployerTypes = new Dictionary<string, Type>();
            
            // process command line parameters
            string[] args = Environment.GetCommandLineArgs();
            try
            {
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i].ToString().ToLower())
                    {
                        case "/options": 
                            m_strConfigFilename = args[++i];
                            if (!File.Exists(m_strConfigFilename))
                                throw new ApplicationException("Invalid config filename: '" + m_strConfigFilename + "'.");
                            XmlSerializer SerializerObj = new XmlSerializer(typeof(TestSettings));
                            FileStream ReadFileStream = new FileStream(m_strConfigFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                            m_testSettings = (TestSettings)SerializerObj.Deserialize(ReadFileStream);
                            ReadFileStream.Close(); 
                            break;
                        case "/logfolder": 
                            m_strLogFolder = args[++i];
                            if (!Directory.Exists(m_strLogFolder))
                                throw new ApplicationException("Invalid log folder: '" + m_strLogFolder + "'.");    
                            break;
                        case "/go": m_bGoOnStart = true; break;
                        case "/exit": m_bExitOnFinish = true; break;
                        
                        default: throw new ApplicationException("Invalid argument: '" + args[i] + "'.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error processing command line arguments.");
                this.Close();
            }

            m_world.TilesHeight = m_testSettings.Tiles.Height;
            m_world.TilesWidth = m_testSettings.Tiles.Width;
        }

        /// <summary>
        /// Splits the int32.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="p_2">The P_2.</param>
        /// <returns></returns>
        private int[] SplitInt32(string p, char p_2)
        {
            List<Int32> integers = new List<int>();
            string[] strings = p.Split(p_2);
            foreach (string str in strings)
            {
                Int32 nValue = 0;
                Int32.TryParse(str, out nValue);
                integers.Add(nValue);
            }
            return integers.ToArray();
        }

        /// <summary>
        /// Handles the Click event of the mnuFileNew control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            // Clear the current simulation and reset it to a fresh one.
            if (backgroundWorker1.IsBusy)
            {
                DialogResult result = MessageBox.Show("Simulation running.  Cancel it?", "Confirm Reset", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
                backgroundWorker1.CancelAsync();
            }

            frmNewWorld dlg = new frmNewWorld();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_world.Reset(dlg.Tiles, dlg.TileSize, dlg.TileShape);
                foreach (Watcher w in m_watcherActive)
                    w.Dispose();
                m_watcherActive.Clear();

                //foreach (Type wt in m_watcherTypes.Values)
                //{
                //    Watcher w = (Watcher)Activator.CreateInstance(wt, m_world.World);
                //}
            }

            m_testSettings.Tiles = dlg.Tiles;
            m_testSettings.TileSize = dlg.TileSize;
            m_testSettings.TileShape = dlg.TileShape;
            DoLogEvent(this, "Created world with " + m_world.TilesWidth + "x" 
                + m_world.TilesHeight + " " + dlg.TileShape + " tiles, sized " + m_world.TileWidth + "x" 
                + m_world.TileHeight + ".");
        }

        /// <summary>
        /// Handles the Click event of the mnuFileExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            //if (m_bIsDirty)
            {
                DialogResult result = MessageBox.Show("Save changes before exiting?", "Confirm Exit", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    // TODO: save changes
                }
            }
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the mnuEditStop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuEditStop_Click(object sender, EventArgs e)
        {
            DoLogEvent(this, "Making stop request.");

            backgroundWorker1.CancelAsync();
            mnuEditGo.Enabled = true;
            mnuEditStep.Enabled = true;
            mnuEditStop.Enabled = false;
            progressBar1.Visible = false;
        }

        /// <summary>
        /// Handles the Click event of the mnuEditGo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuEditGo_Click(object sender, EventArgs e)
        {
            frmStartRunning dlg = new frmStartRunning();
            dlg.m_testSettings = m_testSettings;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                mnuEditGo.Enabled = false;
                mnuEditStep.Enabled = false;
                mnuEditStop.Enabled = true;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                // m_testSettings will already have been updated if they clicked ok in this case
                DoLogEvent(this, "Starting simulation.");

                //SetupTestRun(m_testSettings, m_world);

                m_nTicks = 0;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuEditStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuEditStep_Click(object sender, EventArgs e)
        {
            DoLogEvent(this, "Simulation forward 1 step.");

            Tick();
        }

        private void Tick()
        {
            // just call tick once
            m_world.World.DoPreTickEvent(m_nTicks, m_nRepeat);
            m_world.Tick();
            m_world.World.DoPostTickEvent(m_nTicks, m_nRepeat);

            RefreshProperties();
        }

        /// <summary>
        /// Refreshes the properties of object referenced by the property grid.
        /// </summary>
        private void RefreshProperties()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new MethodInvoker(delegate() { propertyGrid1.Refresh(); }));
            else
                propertyGrid1.Refresh();
        }

        /// <summary>
        /// Logs the results.
        /// </summary>
        private void LogResults()
        {
            //if (lstLeaders.Items.Count == 0)
            //    lstLeaders.Items.Add("   Average   AverageP_s  AverageP_e  AverageP_n    Maximum     Energy      Tile        Incident    Spread      Covered     EventConns  AgentConns");
            //String str = "";
            //if (Monitor.TryEnter(m_world, 3000))
            //{
            //    try
            //    {
            //        str = String.Format("{0,12}{1,12}{2,12}{3,12}{4,12}{5,12}{6,12}{7,12}{8,12}{9,12}{10,12}{11,12}",
            //            m_world.AverageReward.ToString("0.0000"),
            //            m_world.AverageRewardP_s.ToString("0.0000"),
            //            m_world.AverageRewardP_e.ToString("0.0000"),
            //            m_world.AverageRewardP_n.ToString("0.0000"),
            //            m_world.MaxReward.ToString("0.0000"),
            //            m_world.Energy.ToString("0.0000"),
            //            m_world.TileResources.ToString("0.0000"),
            //            m_world.IncidentResources.ToString("0.0000"),
            //            m_world.Spread.ToString("0.0000"),
            //            m_world.IncidentCoverage.ToString("0.0000"),
            //            m_world.IncidentInhabitantEdges.ToString("0.0000"),
            //            m_world.InhabitantInhabitantEdges.ToString("0.0000")
            //            );            
            //    }
            //    finally
            //    {
            //        Monitor.Exit(m_world);
            //    }
            //}
            //else
            //{
            //    // Code to execute if the attempt times out.
            //    // (just wait and try drawing again next time)
            //}
            //lstLeaders.Items.Insert(1, str);
        }

        /// <summary>
        /// Autosaves the save log.
        /// </summary>
        private void AutoSaveLog()
        {
            string strFileName = GetLogfilenameBase();
            StreamWriter sw = File.CreateText(strFileName);
            WriteLogHeaders(sw);
            for (int i = lstLeaders.Items.Count - 1; i > 0; i--)
                sw.WriteLine((lstLeaders.Items.Count - i).ToString() + "," + String.Join(",", lstLeaders.Items[i].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
            sw.Close();
            if (!InvokeRequired)
                lstLeaders.Items.Clear();
            else
                this.BeginInvoke(new MethodInvoker(delegate() { lstLeaders.Items.Clear(); }));
        }

        /// <summary>
        /// Writes the log headers.
        /// </summary>
        /// <param name="sw">The stream to which the headers will be written.</param>
        private void WriteLogHeaders(StreamWriter sw)
        {
            foreach (string str in m_testSettings.Instructions) sw.WriteLine(str);
            //sw.WriteLine(string.Format("Agents: {0}"), String.Join(",", m_testSettings.Agent));
            //sw.Write("Population: "); string strSep = ""; foreach (int i in m_testSettings.Population) { sw.Write(strSep + i.ToString()); strSep = ","; }
            //sw.Write("Range: "); strSep = ""; foreach (int i in m_testSettings.SensorRange) { sw.Write(strSep + i.ToString()); strSep = ","; }
            //sw.WriteLine(string.Format("AgentDeployment: "), m_testSettings.PointDeployment ? "Point" : "Normal");
            sw.WriteLine(string.Format("Environment: {0}({1},{2},{3},{4})"), m_world.World.Tiles.GetType().Name, m_testSettings.Tiles.Width, m_testSettings.Tiles.Height, m_testSettings.TileSize.Width, m_testSettings.TileSize.Height);
            sw.WriteLine(string.Format("Events: {0}({1})"), m_testSettings.IncidentMaxTurnsBeforeMove == 0 ? "Constant" : "Dynamic", m_testSettings.Incident);
            sw.WriteLine(string.Format("EventDeployment: {0}"), m_testSettings.NonUniformIncidentDistribution ? "NonUniform" : "UniformRandom");
            sw.WriteLine("");
            sw.WriteLine("Time," + String.Join(",", lstLeaders.Items[0].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
        }

        /// <summary>
        /// Handles the Click event of the btnSaveLog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            sfdMain.CheckPathExists = true;
            sfdMain.DefaultExt = "csv";
            sfdMain.Filter = "CSV Files|*.csv";
            //sfdMain.FileName = GetLogfilenameBase();
            if (sfdMain.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = File.CreateText(sfdMain.FileName);
                //WriteLogHeaders(sw);
                for (int i=lstLeaders.Items.Count-1; i>0; i--)
                    sw.WriteLine((lstLeaders.Items.Count - i).ToString() + "," + String.Join(",", lstLeaders.Items[i].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
                sw.Close();
            }
        }

        /// <summary>
        /// Gets the base filename of the log file.
        /// </summary>
        /// <returns></returns>
        private string GetLogfilenameBase()
        {
            string strFilenameBase = "simdata-";
            int nFileNumber = 1;

            string strFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            if (m_strLogFolder.Length > 0)
                strFolder = m_strLogFolder;
            while (File.Exists(strFolder + "\\" + strFilenameBase + nFileNumber.ToString("0#") + ".csv"))
                nFileNumber++;

            return strFolder + "\\" + strFilenameBase + nFileNumber.ToString("0#") + ".csv";
        }

        /// <summary>
        /// Creates a bunch of inhabitants.
        /// </summary>
        /// <param name="lst">The list to which the new inhabitants will be added.</param>
        /// <param name="t">The object type of the new inhabitants to create.</param>
        /// <param name="o">The array of initialization parameters for the inhabitant object's constructor.</param>
        /// <param name="nCount">The number of inhabitants to create.</param>
        /// <param name="nSensorRange">The sensor range of the new inhabitants.</param>
        private void GetInhabitants(List<SelectableObject> lst, Type t, object[] o, int nCount, int nSensorRange)
        {
            for (int i = 0; i < nCount; i++)
            {
                SelectableObject inh = (SelectableObject)Activator.CreateInstance(t, o);
                //inh.SensorRange = nSensorRange;
                lst.Add(inh);
            }
        }

        /// <summary>
        /// Sets up the test run.
        /// </summary>
        private void SetupTestRun(TestSettings testSettings, WorldControl world)
        {
            foreach (string str in testSettings.Instructions)
            {
                // the string will contain:
                // - a kind, like "Inhabitant" or "Deployer"
                // - a count
                // - a type name
                // - an assembly name
                // - a list of property-value pairs to be assigned after creation of the object

                Regex regex = new Regex(@"(?<kind>\w+),\s*count\:(?<count>\d+),\s*type\:\""(?<type>.+)\"",\s*assembly\:\""(?<assembly>.+?)\""(,\s*(?<rest>.*))*");
                Match mc = regex.Match(str);

                string kind = mc.Groups["kind"].Value; // args[1];
                int count = Int32.Parse(mc.Groups["count"].Value);
                string type = mc.Groups["type"].Value;
                string assembly = mc.Groups["assembly"].Value;
                string rest = mc.Groups["rest"].Value;
                // args 4+ are properties to set on objects                

                // Inhabitant
                if (kind == "Inhabitant")
                {
                    List<SelectableObject> lstInhabitants = new List<SelectableObject>();
                    Type create = m_agentTypes
                        .Where(s => s.Value.FullName == type)
                        .Select(s => s.Value)
                        .First();

                    Dictionary<string, object> props = new Dictionary<string, object>();
                    Regex regexRest = new Regex(@"(?<name>\w+?)\:\""(?<value>[^\""]+?)\""");
                    Match mcRest = regexRest.Match(rest);
                    while (mcRest.Success)
                    {
                        string propName = mcRest.Groups["name"].Value;
                        string propValue = mcRest.Groups["value"].Value;
                        props[propName] = JsonHelper.DeserializeBase64(propValue);
                        mcRest = mcRest.NextMatch();
                    }

                    for (int i = 0; i < count; i++)
                    {
                        var o = Activator.CreateInstance(create);
                        foreach (string ttps in props.Keys)
                        {
                            // I have a string and need to set the property to its value
                            PropertyInfo pi = o.GetType().GetProperty(ttps);
                            pi.SetValue(o, Convert.ChangeType(props[ttps],pi.PropertyType), null);
                        }
                        SelectableObject so = o as SelectableObject;
                        Debug.Assert(so != null);
                        lstInhabitants.Add(so);
                    }
                    //for (int i = 0; i < testSettings.Agent.Length; i++)
                    //    GetInhabitants(lstInhabitants, m_agentTypes[testSettings.Agent[i]], null, testSettings.Population[i], testSettings.SensorRange[i]);
                    foreach (SelectableObject inhabitant in lstInhabitants)
                    {
                        //inhabitant.World = world.World;
                        //if (testSettings.PointDeployment)
                        //    inhabitant.Position = new Point(world.WorldWidth / 2, world.WorldHeight / 2);
                        //else
                        //    inhabitant.Position = new Point(world.Random.Next(world.WorldWidth), world.Random.Next(world.WorldHeight));
                        ////inhabitant.Velocity = new Point(m_random.Next(20) - 10, m_random.Next(20) - 10);
                        world.Add(inhabitant);
                    }
                    DoLogEvent(this, "Added " + count + " " + type + " inhabitants.");
                }

                // Incidents
                if (kind == "Incident")
                // get type, then attributes; create and add to world
                {
                    // create distribution across tiles, either uniform or not
                    double dTotal = 0.0d;
                    if (testSettings.NonUniformIncidentDistribution)
                    {
                        foreach (Tile t in world.World.Tiles.AllTiles)
                        {
                            t.IncidentDistribution = (float)Math.Max(0.0, world.Random.NextDouble() - 0.5);
                            dTotal += t.IncidentDistribution;
                        }
                    }
                    else
                    {
                        foreach (Tile t in world.World.Tiles.AllTiles)
                        {
                            t.IncidentDistribution = 1.0f;
                            dTotal += t.IncidentDistribution;
                        }
                    }
                    foreach (Tile t in world.World.Tiles.AllTiles)
                        t.IncidentDistribution /= (float)dTotal;

                    for (int i = 0; i < testSettings.Incident; i++)
                    {
                        Incident inc = new Incident();
                        inc.MaxTurnsUntilMove = testSettings.IncidentMaxTurnsBeforeMove;
                        inc.World = world.World;

                        float dRW = (float)world.Random.NextDouble();
                        Tile tMove = world.World.Tiles;
                        foreach (Tile t in world.World.Tiles.AllTiles)
                        {
                            dRW -= t.IncidentDistribution;
                            if (dRW <= 0.0f)
                            {
                                tMove = t;
                                break;
                            }
                        }
                        PointF p = tMove.Position;

                        p.X += world.Random.Next(world.TileWidth);
                        p.Y += world.Random.Next(world.TileHeight);
                        inc.Position = p;
                        Trace.WriteLine(string.Format("Creating incident at ({0},{1}).", p.X, p.Y));

                        inc.Velocity = new PointF(world.Random.Next((int)inc.Size.Width) - inc.Size.Width / 2, world.Random.Next((int)inc.Size.Height) - inc.Size.Height / 2);
                        world.Add(inc);
                    }
                }

                // Deployer
                if (kind == "Deployer")
                {
                    Type create = m_deployerTypes
                        .Where(s => s.Value.FullName == type)
                        .Select(s => s.Value)
                        .First();

                    Dictionary<string, object> props = new Dictionary<string, object>();
                    Regex regexRest = new Regex(@"(?<name>\w+?)\:\""(?<value>[^\""]+?)\""");
                    Match mcRest = regexRest.Match(rest);
                    while (mcRest.Success)
                    {
                        string propName = mcRest.Groups["name"].Value;
                        string propValue = mcRest.Groups["value"].Value;
                        props[propName] = JsonHelper.DeserializeBase64(propValue);
                        mcRest = mcRest.NextMatch();
                    }

                    var o = Activator.CreateInstance(create, m_world.World);
                    foreach (string ttps in props.Keys)
                    {
                        // I have a string and need to set the property to its value
                        PropertyInfo pi = o.GetType().GetProperty(ttps);
                        pi.SetValue(o, Convert.ChangeType(props[ttps], pi.PropertyType), null);
                    }
                    (o as Deployer).Deploy();

                    DoLogEvent(this, "Deployed using " + type + " deployer.");
                }

                // Watcher
                if (kind == "Watcher")
                {
                    Type create = m_watcherTypes
                        .Where(s => s.Value.FullName == type)
                        .Select(s => s.Value)
                        .First();

                    Dictionary<string, object> props = new Dictionary<string, object>();
                    Regex regexRest = new Regex(@"(?<name>\w+?)\:\""(?<value>[^\""]+?)\""");
                    Match mcRest = regexRest.Match(rest);
                    while (mcRest.Success)
                    {
                        string propName = mcRest.Groups["name"].Value;
                        string propValue = mcRest.Groups["value"].Value;
                        props[propName] = JsonHelper.DeserializeBase64(propValue);
                        mcRest = mcRest.NextMatch();
                    }

                    var o = Activator.CreateInstance(create, m_world.World);
                    foreach (string ttps in props.Keys)
                    {
                        // I have a string and need to set the property to its value
                        PropertyInfo pi = o.GetType().GetProperty(ttps);
                        pi.SetValue(o, Convert.ChangeType(props[ttps], pi.PropertyType), null);
                    }
                    DoLogEvent(this, "Added " + type + " watcher.");
                }

            }

        }

        /// <summary>
        /// Handles the Load event of the frmWorldViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void frmWorldViewer_Load(object sender, EventArgs e)
        {
            mnuEditGo.Enabled = true;
            mnuEditStep.Enabled = true;
            mnuEditStop.Enabled = false;
            progressBar1.Visible = false;
            m_world.LogEvent += new WorldControl.LogDelegate(OnLogEvent);

            DoLogEvent(this, "Created world with " + m_world.TilesWidth + "x"
                + m_world.TilesHeight + " Rectangle tiles, sized " + m_world.TileWidth + "x"
                + m_world.TileHeight + ".");

            // add watcher list to menu
            foreach (string strWatcherName in m_watcherTypes.Keys)
            {
                ToolStripMenuItem mnuWatcher = new ToolStripMenuItem(strWatcherName);
                mnuWatcher.Click += new EventHandler(mnuWatcher_Click);
                mnuEditWatcher.DropDown.Items.Add(mnuWatcher);
            }

            if (m_bGoOnStart)
            {
                mnuEditGo.Enabled = false;
                mnuEditStep.Enabled = false;
                mnuEditStop.Enabled = true;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                SetupTestRun(m_testSettings, m_world);
                m_nTicks = 0;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        void mnuWatcher_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuWatcher = (ToolStripMenuItem)sender;
            mnuWatcher.Checked = !mnuWatcher.Checked;
            if (mnuWatcher.Checked)
            {
                DoLogEvent(this, "Added " + mnuWatcher.Text + " watcher.");
                Watcher w = (Watcher)Activator.CreateInstance(m_watcherTypes[mnuWatcher.Text], m_world.World);
                string str = JsonHelper.WorldTypeToJson("Watcher", w, 1);
                m_testSettings.Instructions.Add(str);
            }
        }

        /// <summary>
        /// Called when a LogEvent is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="WorldSim.WorldControl.LogEventArgs"/> instance containing the event data.</param>
        void OnLogEvent(object sender, EventArgs e)
        {
            WorldControl.LogEventArgs ea = (WorldControl.LogEventArgs)e;
            lstLeaders.Items.Add(ea.Message);
        }
        private void DoLogEvent(object sender, string message)
        {
            if (InvokeRequired)
                Invoke(new EventHandler(OnLogEvent),
                    new object[] { sender, new WorldControl.LogEventArgs(message) });
            else
                OnLogEvent(this, new WorldControl.LogEventArgs(message));
        }

        /// <summary>
        /// Handles the DoWork event of the backgroundWorker1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DoLogEvent(sender, "Starting test run.");
            m_world.World.DoPreTestRunEvent();

            for (m_nRepeat = 0; m_nRepeat < Math.Max(m_testSettings.Repeats,1) && !backgroundWorker1.CancellationPending; m_nRepeat++)
            {
                m_nTicks = 0;
                m_world.World.DoPreStepEvent();
                if (m_nRepeat > 0) // don't do the first time
                {
                    //System.Diagnostics.Debug.WriteLine("Resetting world.");
                    DoLogEvent(this, "Resetting world.");

                    this.m_world.Reset(m_testSettings.Tiles, m_testSettings.TileSize, m_testSettings.TileShape);
                    foreach (Watcher w in m_watcherActive)
                        w.Dispose();
                    m_watcherActive.Clear();
                    DoLogEvent(this, "Created world with " + m_world.TilesWidth + "x"
                        + m_world.TilesHeight + " " + m_testSettings.TileShape + " tiles, sized " + m_world.TileWidth + "x"
                        + m_world.TileHeight + ".");

                    // re-setup the environment the same as m_testSettings indicates 
                    // -- any custom settings from initial setup will be lost 
                    // -- in other words, repeats are only really supported via cmdline
                    SetupTestRun(m_testSettings, m_world);
                }

                for (m_nTicks = 0; m_nTicks < m_testSettings.Duration && !backgroundWorker1.CancellationPending; m_nTicks++ )
                {
                    // process a single tick
                    Tick();

                    // log every so many ticks
                    if (m_nTicks % m_testSettings.LogFrequency == 0)
                        backgroundWorker1.ReportProgress((int)(100 * m_nTicks / m_testSettings.Duration));

                    // give the app a chance to catch up
                    Application.DoEvents();
                }
                m_world.World.DoPostStepEvent();
            }

            if (m_bExitOnFinish)
                AutoSaveLog();

            DoLogEvent(this, "Test run completed.");
            m_world.World.DoPostTestRunEvent();
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the backgroundWorker1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mnuEditGo.Enabled = true;
            mnuEditStep.Enabled = true;
            mnuEditStop.Enabled = false;
            progressBar1.Visible = false;
            if (m_bExitOnFinish)
            {
                AutoSaveLog();
                this.Close();
            }
            else
            {
                //MessageBox.Show("Simulation ended successfully.");
                DoLogEvent(this, "Simulation ended successfully.");
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the backgroundWorker1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            LogResults();
        }

        /// <summary>
        /// Handles the Click event of the mnuFileSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            // Need to store the current state of things.  This will hopefully just be
            // a simple matter of serializing out the World property of the WorldControl.

            sfdMain.FileName = "";
            sfdMain.Filter = "XML Files|*.xml";
            sfdMain.CheckFileExists = false;
            sfdMain.OverwritePrompt = true;
            if (sfdMain.ShowDialog() == DialogResult.OK)
            {
                m_world.Serialize(sfdMain.FileName);
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuFileOpen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            ofdMain.FileName = "";
            ofdMain.Filter = "XML Files|*.xml";
            ofdMain.CheckFileExists = true;
            if (ofdMain.ShowDialog() == DialogResult.OK)
            {
                m_world.Deserialize(ofdMain.FileName);
                Invalidate();
            }
        }

        /// <summary>
        /// Handles the ObjectSelectedEvent event of the m_world control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.WorldControl.ObjectSelectedEventArgs"/> instance containing the event data.</param>
        private void m_world_ObjectSelectedEvent(object sender, WorldControl.ObjectSelectedEventArgs e)
        {
            if (e.Selected == null)
                propertyGrid1.SelectedObject = m_world.World;
            else
                propertyGrid1.SelectedObject = e.Selected;
        }

        /// <summary>
        /// Handles the Click event of the zoom10ToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void zoomWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                zoomOut();
            else if (e.Delta > 0)
                zoomIn();
            //else return;
        }
        
        /// <summary>
        /// Modifies the zoom level by 1.
        /// </summary>
        private void zoomIn()
        {
            m_world.Zoom += 0.02f;
        }

        /// <summary>
        /// Modifies the zoom level by -1.
        /// </summary>
        private void zoomOut()
        {
            m_world.Zoom -= 0.02f;
        }

        /// <summary>
        /// Handles the Click event of the mnuAddObjects control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuAddObjects_Click(object sender, EventArgs e)
        {
            frmAddObjects dlg = new frmAddObjects();
            dlg.m_agentTypes = m_agentTypes;
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            // add the specified objects as listed in dlg
            m_testSettings.Instructions.Add(dlg.m_strAgentType);
            for (int i = 0; i < dlg.m_nPopulation; i++)
            {
                SelectableObject o = (SelectableObject)Activator.CreateInstance(dlg.m_agentType, null);

                o.World = m_world.World;
                o.Position = m_world.World.Tiles.Center;
                m_world.World.Tiles.AddObject(o);
#if false
                if (dlg.m_deployMethod == frmAddObjects.DeploymentMethod.Point)
                {
                    Point pt = new Point(m_world.WorldWidth / 2, m_world.WorldHeight / 2);
                    do // agents could land outside the grid.  Repeat until coordinates inside the grid.
                    {
                        double dRow = m_world.Random.NextGaussian() * dlg.m_sigma + 0.5;
                        double dCol = m_world.Random.NextGaussian() * dlg.m_sigma + 0.5;

                        pt.X += (int)(dCol * m_world.WorldWidth);
                        pt.Y += (int)(dRow * m_world.WorldHeight);

                    } while (pt.Y < 0 || pt.Y > m_world.WorldHeight - 1 || pt.X < 0 || pt.X > m_world.WorldWidth - 1);
                    o.Position = pt;
                }                
                else if (dlg.m_deployMethod == frmAddObjects.DeploymentMethod.Spread)
                {
                    int nRow = i / m_world.TilesWidth;
                    int nCol = i % m_world.TilesWidth;
                    // find the center of tile[nRow, nCol]                    
                    o.Position = new Point(m_world.TileWidth / 2 + m_world.TileWidth * nCol,
                        m_world.TileHeight / 2 + m_world.TileHeight * nRow);
                }
                else
                    o.Position = new Point(m_world.Random.Next(m_world.WorldWidth), m_world.Random.Next(m_world.WorldHeight));
                //o.Velocity = new Point(m_random.Next(20) - 10, m_random.Next(20) - 10);
                
                m_world.World.Add(o);
#endif
            }

            DoLogEvent(this, "Added " + dlg.m_nPopulation + " " + dlg.m_agentType + " objects.");
            m_world.Invalidate();
        }

        /// <summary>
        /// Handles the Click event of the mnuEditDeploy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuEditDeploy_Click(object sender, EventArgs e)
        {
            frmDeploy dlg = new frmDeploy();
            dlg.m_deployTypes = m_deployerTypes;
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            Deployer o = (Deployer)Activator.CreateInstance(dlg.m_deployType, m_world.World);
            o.Deploy();
            DoLogEvent(this, "Deployed using " + dlg.m_deployType + " deployer.");
            string str = JsonHelper.WorldTypeToJson("Deployer", o, 1);
            m_testSettings.Instructions.Add(str);
            m_world.Invalidate();
        }

        /// <summary>
        /// Clicking this menu item should toggle the setting that tells the world control
        /// to indicate (by the color of the tile) how long since a tile has been visited or
        /// 'covered' by sensors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showCoverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == showStaleToolStripMenuItem)
                this.m_world.World.TileBackgroundIndication = World.TileBackgroundIndicates.tbiStale;
            else if (sender == showPolledToolStripMenuItem)
                this.m_world.World.TileBackgroundIndication = World.TileBackgroundIndicates.tbiPoll;
            else if (sender == showResourcesToolStripMenuItem)
                this.m_world.World.TileBackgroundIndication = World.TileBackgroundIndicates.tbiResources;
            else // if (sender == showNoneToolStripMenuItem)
                this.m_world.World.TileBackgroundIndication = World.TileBackgroundIndicates.tbiNone;
            this.showResourcesToolStripMenuItem.Checked = (sender == showResourcesToolStripMenuItem);
            this.showStaleToolStripMenuItem.Checked = (sender == showStaleToolStripMenuItem);
            this.showPolledToolStripMenuItem.Checked = (sender == showPolledToolStripMenuItem);
            this.showNoneToolStripMenuItem.Checked = (sender == showNoneToolStripMenuItem);
            m_world.Invalidate();
        }

        private void showZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == zoom10ToolStripMenuItem)
                m_world.Zoom = 0.10f;
            else if (sender == zoom20ToolStripMenuItem)
                m_world.Zoom = 0.25f;
            else if (sender == zoom50ToolStripMenuItem)
                m_world.Zoom = 0.50f;
            else if (sender == zoom75ToolStripMenuItem)
                m_world.Zoom = 0.75f;
            else if (sender == zoom100ToolStripMenuItem)
                m_world.Zoom = 1.0f;
            //this.zoom10ToolStripMenuItem.Checked = (sender == zoom10ToolStripMenuItem);
            //this.zoom20ToolStripMenuItem.Checked = (sender == zoom20ToolStripMenuItem);
            //this.zoom50ToolStripMenuItem.Checked = (sender == zoom50ToolStripMenuItem);
            //this.zoom75ToolStripMenuItem.Checked = (sender == zoom75ToolStripMenuItem);
            //this.zoom100ToolStripMenuItem.Checked = (sender == zoom100ToolStripMenuItem);
            m_world.Invalidate();
        }

    }
}
