using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSim.Interface;
using System.Runtime.Serialization;

namespace DisabledMobility
{
    [Serializable]
    public class DisabledMobilitySensor : Inhabitant
    {
        [CategoryAttribute("Behavior"), ReadOnlyAttribute(true)]
        public int Expiration { get; set; }

        private bool m_actionSet;
        [CategoryAttribute("Initialization"), ReadOnlyAttribute(true)]
        public World.Actions DefaultAction { get; set; }

        /// <summary>
        /// Constructor.  Set default values.
        /// </summary>
        public DisabledMobilitySensor()
        {
            m_actionSet = false;
        }

        /// <summary>
        /// As time (ticks) progress, the odds that the mobility of this
        /// agent will fail increase.  Once the sensor expires, it stops
        /// moving.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (!m_actionSet)
            {
                m_actionSet = true;
                DefaultAction = (World.Actions)Parent.World.Random.Next((int)World.Actions.Max); // returns 0-15 (not actStay)
            }

            if (Expiration > 0)
            {
                Action = DefaultAction;
                Expiration--;
            }
            else
                Action = World.Actions.actStay;
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "A mobile sensor that can stop moving after an assigned expiration date (number of ticks)."; }
    }
}
