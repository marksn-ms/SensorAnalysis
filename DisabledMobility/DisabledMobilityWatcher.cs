using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSim.Interface;

namespace DisabledMobility
{
    public class WatcherCoverage
    {
        public PointF Point { get; set; }
        public int Polled { get; set; }
        public WatcherCoverage(PointF p)
        {
            Point = p;
            Polled = 0;
        }
    }

    public class DisabledMobilityWatcher : Watcher
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSensorWatcher"/> class.
        /// </summary>
        /// <param name="w">The <see cref="World"/>.</param>
        public DisabledMobilityWatcher(World world)
            : base(world)
        {
            Log = new StringWriter();
            if (world != null)
            {
                world.PostTickEvent += new World.PostTickDelegate(OnPostTickEvent);
                world.PostStepEvent += new World.PostStepDelegate(OnPostStepEvent);
            }
        }

        /// <summary>
        /// Called once a test run is over, at which time we'll take the opportunity to write
        /// the contents of our log to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPostStepEvent(object sender, World.PostStepEventArgs e)
        {
            WriteLog();
            Log.Flush();
        }

        /// <summary>
        /// Handles the MessageSentEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.MessageSentEventArgs"/> instance containing the event data.</param>
        private Dictionary<Tile, int> m_tilesPolled;
        private Dictionary<Tile, List<WatcherCoverage>> m_pointsPolled;
        private int m_lastTickLogged = -1;
        private int m_lastTickPolled = -1;
        private int m_timesPolled = 0;
        private int m_lastTickPointsPolled = -1;
        private int m_timesPointsPolled = 0;
        void OnPostTickEvent(object sender, World.PostTickEventArgs e)
        {
            double range = Math.Sqrt(2) * World.Tiles.Size.Width / 2;

            if (m_tilesPolled == null)
            {
                m_tilesPolled = new Dictionary<Tile, int>();
                m_pointsPolled = new Dictionary<Tile, List<WatcherCoverage>>();
                foreach (Tile t in World.Tiles.AllTiles)
                {
                    m_tilesPolled.Add(t, -1);
                    m_pointsPolled.Add(t, new List<WatcherCoverage>());
                    for (int i = 0; i < t.Size.Width; i++)
                    {
                        for (int j = 0; j < t.Size.Height; j++)
                        {
                            m_pointsPolled[t].Add(new WatcherCoverage(new PointF(t.Position.X + i, t.Position.Y + j)));
                        }
                    }
                }
            }

            const int LogTicks = 20;
            if (e.Tick % LogTicks == 0)
            {
                int nSensors = 0;
                int nDisabled = 0;
                foreach (DisabledMobilitySensorRandomWalk d in World.Objects(typeof(DisabledMobilitySensorRandomWalk)))
                {
                    nSensors++;
                    if (d.Expiration == 0)
                        nDisabled++;
                }

                // num_tiles_covered, num_tiles_uncovered, percent_tiles_covered
                // this counts the static number covered after each tick
                int nTilesCovered = 0;
                int nTilesUncovered = 0;
                int nTilesNewlyCovered = 0;
                int nPolled = 0;
                int nNotPolled = 0;
                int nPointsCovered = 0;
                int nPointsNewlyCovered = 0;
                int nPoints = 0;
                int nPointsPolled = 0;
                int nPointsNotPolled = 0;
                foreach (Tile t in World.Tiles.AllTiles)
                {
                    if (t.HasObjects(typeof(DisabledMobilitySensorRandomWalk)))
                    {
                        // this tile is covered
                        nTilesCovered++;

                        // if it was covered last tick, then it isn't newly covered
                        if (m_tilesPolled[t] > m_lastTickLogged)
                            nTilesNewlyCovered++;
                        m_tilesPolled[t] = e.Tick;
                    }
                    else
                    {
                        // this tile is uncovered
                        nTilesUncovered++;
                    }
                    // if all the tiles' last tick visited value is higher than m_lastTickPolled
                    // then it means we have polled again
                    if (m_tilesPolled[t] > m_lastTickPolled)
                        nPolled++;
                    else
                        nNotPolled++;

                    var o = t.Objects(typeof(DisabledMobilitySensorRandomWalk));
                    foreach (WatcherCoverage w in m_pointsPolled[t])
                    {
                        nPoints++;
                        foreach (SelectableObject s in o)
                        {
                            if (World.Distance(w.Point, s.Position) < range)
                            {
                                if (w.Polled > m_lastTickLogged)
                                    nPointsNewlyCovered++;
                                nPointsCovered++;
                                w.Polled = e.Tick;
                                break;
                            }
                        }
                        if (w.Polled > m_lastTickPointsPolled)
                            nPointsPolled++;
                        else
                            nPointsNotPolled++;
                    }
                }

#if false
                // compute a more true coverage (slower)
                int nPointCovered = 0;
                int nPointUncovered = 0;
                foreach (Tile t in World.Tiles.AllTiles)
                {
                    IEnumerable<SelectableObject> inhabitants = t.NearbyObjects(typeof(DisabledMobilitySensorRandomWalk));
                    for (int i = 0; i < t.Size.Width; i+=4)
                    {
                        for (int j = 0; j < t.Size.Height; j+=4)
                        {
                            PointF p = new PointF(t.Position.X+i, t.Position.Y+j);
                            bool bCovered = false;
                            foreach (SelectableObject s in inhabitants)
                            {
                                if (World.Distance(s.Position, p) < range)
                                {
                                    bCovered = true;
                                    break;
                                }
                            }
                            if (bCovered)
                                nPointCovered++;
                            else
                                nPointUncovered++;
                        }
                    }
                }
#endif

                // num_ticks_since_last_poll, num_tiles_polled, num_tiles_unpolled
                // we have to check each tile to see if it has been polled since last time all polled
                if (e.Tick > 0 && nNotPolled == 0) // we polled again
                {
                    m_lastTickPolled = e.Tick - 1;
                    m_timesPolled++;
                }
                if (e.Tick > 0 && nPointsNotPolled == 0) // we polled again
                {
                    m_lastTickPointsPolled = e.Tick - 1;
                    m_timesPointsPolled++;
                }
                m_lastTickLogged = e.Tick;

                Log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}",
                    World.Title,
                    World.Guid.ToString(), 
                    e.Repeat, e.Tick, nDisabled, nSensors,
                    nTilesCovered, nTilesUncovered,
                    nTilesNewlyCovered,
                    nPolled, nNotPolled, m_timesPolled,
                    nPointsCovered, nPoints-nPointsCovered,
                    nPointsNewlyCovered, 
                    nPointsPolled, nPointsNotPolled, m_timesPointsPolled);
            }
            else
            {
                foreach (Tile t in World.Tiles.AllTiles)
                {
                    if (t.HasObjects(typeof(DisabledMobilitySensorRandomWalk)))
                        m_tilesPolled[t] = e.Tick;
                }
            }

            // TODO: periodically, if it is time to write, do that now
        }

        /// <summary>
        /// Removes the event handlers.
        /// </summary>
        public override void RemoveEventHandlers()
        {
            World.PostTickEvent -= this.OnPostTickEvent;
            World.PostStepEvent -= this.OnPostStepEvent;
        }

        private StringWriter Log { get; set; }

        /// <summary>
        /// Autosaves the save log.
        /// </summary>
        private void WriteLog()
        {
            int writeLogTriesLeft = 6000;
            while ((writeLogTriesLeft--) > 0)
            {
                try
                {
                    string strFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    strFolder += "\\" + "DisabledMobility";
                    if (!Directory.Exists(strFolder))
                        Directory.CreateDirectory(strFolder);
                    string strFileName = strFolder + "\\simdata.csv";

                    StreamWriter sw;
                    if (File.Exists(strFileName))
                        sw = File.AppendText(strFileName);
                    else
                    {
                        sw = File.CreateText(strFileName);
                        sw.WriteLine("parms,sim,trial,tick,num_disabled,num_sensors,num_covered,num_uncovered,num_newly_covered,num_polled,num_notpolled,times_polled,points_covered,points_uncovered,points_newlycovered,points_polled,points_notpolled,times_points_polled");
                    }
                    sw.Write(Log.ToString());
                    sw.Close();
                    Log.Flush();

                    // now that we wrote the log, we can leave
                    writeLogTriesLeft = 0;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DisabledMobilityWatcher.WriteLog: unable to write to log...retrying.");
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }    
}
