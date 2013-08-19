﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSim.Interface;

namespace DisabledMobility
{
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
        private int m_lastTickPolled = -1;
        private int m_lastTickLogged = -1;
        private int m_timesPolled = 0;
        void OnPostTickEvent(object sender, World.PostTickEventArgs e)
        {
            if (m_tilesPolled == null)
            {
                m_tilesPolled = new Dictionary<Tile, int>();
                foreach (Tile t in World.Tiles.AllTiles)
                    m_tilesPolled.Add(t, -1);
            }

            const int LogTicks = 10;
            if (e.Tick % LogTicks == 0)
            {
                int nSensors = 0;
                int nDisabled = 0;
                foreach (DisabledMobilitySensor d in World.Objects(typeof(DisabledMobilitySensor)))
                {
                    nSensors++;
                    if (d.Expiration < e.Tick)
                        nDisabled++;
                }

                // num_tiles_covered, num_tiles_uncovered, percent_tiles_covered
                // this counts the static number covered after each tick
                int nTilesCovered = 0;
                int nTilesUncovered = 0;
                int nTilesNewlyCovered = 0;
                int nTilesNotNewlyCovered = 0;
                int nPolled = 0;
                int nNotPolled = 0;
                foreach (Tile t in World.Tiles.AllTiles)
                {
                    if (t.HasObjects(typeof(DisabledMobilitySensor)))
                    {
                        // this tile is covered
                        nTilesCovered++;

                        // if it was covered last tick, then it isn't newly covered
                        if (m_tilesPolled[t] > m_lastTickLogged)
                            nTilesNewlyCovered++;
                        else
                            nTilesNotNewlyCovered++;
                        m_tilesPolled[t] = e.Tick;
                    }
                    else
                    {
                        // this tile is uncovered
                        nTilesUncovered++;
                        nTilesNotNewlyCovered++;
                    }
                }

                // num_ticks_since_last_poll, num_tiles_polled, num_tiles_unpolled
                // we have to check each tile to see if it has been polled since last time all polled
                foreach (int i in m_tilesPolled.Values)
                {
                    // if all the tiles' last tick visited value is higher than m_lastTickPolled
                    // then it means we have polled again
                    if (i <= m_lastTickPolled)
                        nNotPolled++;
                    else
                        nPolled++;
                }
                if (e.Tick > 0 && nNotPolled == 0) // we polled again
                {
                    m_lastTickPolled = e.Tick - 1;
                    m_timesPolled++;
                }
                m_lastTickLogged = e.Tick;

                Log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    World.Guid.ToString(), 
                    e.Repeat, e.Tick, nDisabled, nSensors,
                    nTilesCovered, nTilesUncovered,
                    nTilesNewlyCovered, nTilesNotNewlyCovered,
                    nPolled, nNotPolled, m_timesPolled);
            }
            else
            {
                foreach (Tile t in World.Tiles.AllTiles)
                {
                    if (t.HasObjects(typeof(DisabledMobilitySensor)))
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
                        sw.WriteLine("sim,trial,tick,num_disabled,num_sensors,num_covered,num_uncovered,num_newly_covered,num_notnewly_covered,num_polled,num_notpolled,times_polled");
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
