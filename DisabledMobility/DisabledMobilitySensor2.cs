using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSim.Interface;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace DisabledMobility
{
    [Serializable]
    public class DisabledMobilitySensor2 : DisabledMobilitySensor
    {
        private PointF m_endActionTarget;

        /// <summary>
        /// Constructor.  Set default values.
        /// </summary>
        public DisabledMobilitySensor2()
            : base()
        {
        }

        /// <summary>
        /// As time (ticks) progress, the odds that the mobility of this
        /// agent will fail increase.  Once the sensor expires, it stops
        /// moving.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (Expiration > 0 && Expiration <= SensorRange)
            {
                if (m_endActionTarget.IsEmpty)
                {
                    // locate a vacant cell nearby and head for that location
                    int Max = -1;
                    foreach (Tile t in Parent.Neighbors)
                        Max = Math.Max(t.Objects(typeof(DisabledMobilitySensor)).Count(), Max);
                    RouletteWheel<Tile> r = new RouletteWheel<Tile>(Parent.World.Random);
                    foreach (Tile t in Parent.Neighbors)
                        r.Add(0.1 + Max - t.Objects(typeof(DisabledMobilitySensor)).Count(), t);

                    Tile target = r.Choice;

                    // now just head in that direction by setting vector to the direction of that tile
                    // (remember the tile north of us may be on the other (south) end of the grid)
                    m_endActionTarget = target.Center;
                }
                PointF v = Tile.VectorTo(Position, m_endActionTarget, Parent.World.Width, Parent.World.Height);
                Velocity = new PointF(v.X * 2, v.Y * 2);
                //Debug.WriteLine("DisabledMobilitySensor2.Tick: Position({0},{1}), Target({2},{3}), Velocity({4},{5}), Sensor({6},{7}).",
                //    Parent.Center.X, Parent.Center.Y, m_endActionTarget.X, m_endActionTarget.Y,
                //    Velocity.X, Velocity.Y, Position.X, Position.Y);
                Action = World.Actions.actUseVelocity;
            }
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "A mobile sensor that can stop moving after an assigned expiration date (number of ticks), but as it nears its end, tries to avoid other sensors."; }
    }
}
