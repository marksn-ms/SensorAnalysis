using System;
using System.Collections.Generic;
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
            if (world != null)
            {
                world.PostTickEvent += new World.PostTickDelegate(OnPostTickEvent);
                world.PostStepEvent += new World.PostStepDelegate(OnPostStepEvent);
            }
        }

        private void OnPostStepEvent(object sender, World.PostStepEventArgs e)
        {
            m_sw.Close();
            m_sw = null;
        }

        /// <summary>
        /// Handles the MessageSentEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.MessageSentEventArgs"/> instance containing the event data.</param>
        void OnPostTickEvent(object sender, World.PostTickEventArgs e)
        {
            if (m_sw == null)
                CreateLog();

            int nSensors = 0;
            int nDisabled = 0;
            foreach (DisabledMobilitySensor d in World.Objects(typeof(DisabledMobilitySensor)))
            {
                nSensors++;
                if (d.Expiration < e.Tick)
                    nDisabled++;
            }
            m_sw.WriteLine(e.Tick.ToString("0000#") + "," + nDisabled.ToString() + "," + nSensors.ToString());
        }

        /// <summary>
        /// Removes the event handlers.
        /// </summary>
        public override void RemoveEventHandlers()
        {
            World.PostTickEvent -= this.OnPostTickEvent;
            World.PostStepEvent -= this.OnPostStepEvent;
        }

        private StreamWriter m_sw;

        /// <summary>
        /// Autosaves the save log.
        /// </summary>
        private void CreateLog()
        {
            string strFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            strFolder += "\\" + "DisabledMobility";
            if (!Directory.Exists(strFolder))
                Directory.CreateDirectory(strFolder);
            int nFileNumber = 0;
            string strFileName = "";
            do
            {
                nFileNumber++;
                strFileName = strFolder + "\\simdata-" + nFileNumber.ToString("0#") + ".csv";
            } while (File.Exists(strFileName));
            m_sw = File.CreateText(strFileName);
        }
    }    
}
