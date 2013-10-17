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
    public class DisabledMobilitySensorVF : DisabledMobilitySensorRandomWalk
    {
        internal class DoubleComparer : IComparer<double>
        {
            const double eps = 1E-10;
            public int Compare(double x, double y)
            { return y > x + eps ? -1 : y < x - eps ? 1 : 0; }
        } 

        public PointF ActionTarget { get; set; }

        /// <summary>
        /// Constructor.  Set default values.
        /// </summary>
        public DisabledMobilitySensorVF()
            : base()
        {
            Expiration = -1;
        }

        /// <summary>
        /// As time (ticks) progress, the odds that the mobility of this
        /// agent will fail increase.  Once the sensor expires, it stops
        /// moving.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            Debug.Assert(Parent.PointInRegion(Position));

            Action = World.Actions.actStay;
            Velocity = PointF.Empty;
            if (ActionTarget == null)
                ActionTarget = Parent.Center;

            if (Expiration == 0)
                return;
            //if ((Ticks + ID) % 5 == 0) // add a bit of randomness to their behavior
            //    return;

            if (((Ticks + ID) % 25 == 0) || World.Distance(PointF.Empty, Parent.VectorTo(Position, ActionTarget, false)) < 5.0)
            {
                // get list of tiles nearby, then agents nearby, limit to closest three agents
                List<Tile> tileTargets = new List<Tile>();
                List<Tile> otherTiles = new List<Tile>();
                tileTargets.Add(Parent);
                foreach (Tile t in Parent.Neighbors)
                {
                    tileTargets.Add(t);
                }

                SortedList<double,DisabledMobilitySensorVF> agents = new SortedList<double,DisabledMobilitySensorVF>(new DoubleComparer());
                foreach (Tile t in tileTargets)
                {
                    foreach (DisabledMobilitySensorVF s in t.Objects(typeof(DisabledMobilitySensorVF)))
                    {
                        agents.Add(World.Distance(s.Position, this.Position), s);
                    }
                    foreach (Tile tt in t.Neighbors)
                    {
                        if (!tileTargets.Contains(tt) && !otherTiles.Contains(tt))
                        {
                            otherTiles.Add(tt);
                            foreach (DisabledMobilitySensorVF s in tt.Objects(typeof(DisabledMobilitySensorVF)))
                                agents.Add(World.Distance(s.Position, this.Position), s);
                        }
                    }
                    while (agents.Keys.Count > 6) // only keep closest 6
                        agents.Remove(agents.Keys.Last());
                }

                // for each nearby sensor, we want to sum up the attractive/repulsive force
                // the force is the vector from sensor to this agent, minus length of ideal distance (distance * sqrt(3) / 2)
                // the sum of all these vectors is the ideal spot we'd like to head towards
                PointF newTarget = new PointF();
                foreach (var p in agents.Values)
                {
                    PointF v = Parent.VectorTo(p.Position, this.Position, false);
                    PointF f = GetForce(v);
                    newTarget.X += f.X;
                    newTarget.Y += f.Y;
                }

                // by default, try to stay where we currently are
                ActionTarget = newTarget;
            }

            // if we got here, we have a target to move towards, so compute the vector to it
            if (World.Distance(Position, ActionTarget) < 5.0)
                Velocity = PointF.Empty;
            else
                Velocity = Tile.VectorTo(Position, ActionTarget, Parent.World.Width, Parent.World.Height);
            Action = World.Actions.actUseVelocity;
        }

        private PointF GetForce(PointF vector)
        {
            const double sqrt3over2 = 0.8660254037844386; // Math.Sqrt(3) / 2;
            PointF force = new PointF();
            double dist = World.Distance(PointF.Empty, vector);
            double scale = sqrt3over2 / dist;
            force.X = (float)(vector.X * scale);
            force.Y = (float)(vector.Y * scale);
            return force;
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "A mobile sensor that can stop moving after an assigned expiration date (number of ticks), but uses a virtual force based algorithm for selecting a place to move to fill coverage holes and avoid other sensors."; }
    }
}
