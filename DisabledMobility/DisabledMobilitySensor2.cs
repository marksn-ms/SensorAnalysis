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
    public class DisabledMobilitySensor2 : Inhabitant
    {
        private int m_expiration;
        [CategoryAttribute("Behavior"), ReadOnlyAttribute(true)]
        public int Expiration { get { return m_expiration; } set { m_expiration = value; } }

        private PointF m_endActionVector;
        private bool m_actionSet;
        private World.Actions m_action;
        [CategoryAttribute("Initialization"), ReadOnlyAttribute(true)]
        public World.Actions DefaultAction { get { return m_action; } set { m_action = value; } }

        /// <summary>
        /// Constructor.  Set default values.
        /// </summary>
        public DisabledMobilitySensor2()
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

            if (m_expiration > SensorRange)
            {
                if (!m_actionSet)
                {
                    m_actionSet = true;
                    DefaultAction = (World.Actions)Parent.World.Random.Next((int)World.Actions.Max); // returns 0-15 (not actStay)
                }
                Action = DefaultAction;
                m_expiration--;
            }
            else if (m_expiration > 0)
            {
                if (m_endActionVector.IsEmpty)
                {
                    // locate a vacant cell nearby and head for that location
                    int Max = -1;
                    foreach (Tile t in Parent.Neighbors)
                        Max = Math.Max(t.Objects(typeof(DisabledMobilitySensor2)).Count(), Max);
                    RouletteWheel<Tile> r = new RouletteWheel<Tile>(World.Random);
                    foreach (Tile t in Parent.Neighbors)
                        r.Add(0.1 + Max - t.Objects(typeof(DisabledMobilitySensor)).Count(), t);

                    Tile target = r.Choice;

                    // now just head in that direction by setting vector to the direction of that tile
                    // (remember the tile north of us may be on the other (south) end of the grid)
                    m_endActionVector = Parent.VectorTo(target);
                }
                Velocity = m_endActionVector;
                Action = World.Actions.actUseVelocity;
                m_expiration--;
            }
            else
                Action = World.Actions.actStay;
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "A mobile sensor that can stop moving after an assigned expiration date (number of ticks), but as it nears its end, tries to avoid other sensors."; }
    }
}
