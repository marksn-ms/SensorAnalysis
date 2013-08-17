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
    public class DisabledMobilitySensorProxy : DisabledMobilitySensor
    {
        public PointF ActionTarget { get; set; }

        /// <summary>
        /// Constructor.  Set default values.
        /// </summary>
        public DisabledMobilitySensorProxy()
            : base()
        {
            ActionTarget = PointF.Empty;
        }

        /// <summary>
        /// As time (ticks) progress, the odds that the mobility of this
        /// agent will fail increase.  Once the sensor expires, it stops
        /// moving.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (Expiration > 0)
            {
                if (ActionTarget.IsEmpty || World.Distance(PointF.Empty, Tile.VectorTo(Position, ActionTarget, World.Width, World.Height, false)) < 3.0)
                {
                    // count how many occupants of nearby tiles have sensors with that tile as a target
                    Dictionary<PointF, float> tileTargets = new Dictionary<PointF, float>();
                    AddTileTargets(tileTargets, Parent);
                    foreach (Tile t in Parent.Neighbors)
                        AddTileTargets(tileTargets, t);
                    Debug.Assert(tileTargets.Count >= 9);

                    // pick the closest tile with the lowest occupancy and go there
                    RouletteWheel<PointF> rw = new RouletteWheel<PointF>(World.Random);
                    rw.SmallValuesBetter = true;
                    float minTarget = float.NegativeInfinity;
                    foreach (PointF target in tileTargets.Keys)
                    {
                        double distanceToTarget = World.Distance(PointF.Empty, Tile.VectorTo(Position, target, World.Width, World.Height, false));
                        if (minTarget == float.NegativeInfinity || tileTargets[target] < minTarget)
                        {
                            rw.Clear().Add(distanceToTarget, target);
                            minTarget = tileTargets[target];
                        }
                        else if (tileTargets[target] == minTarget)
                        {
                            rw.Add(distanceToTarget, target);
                        }
                    }
                    PointF closestTarget = rw.Choice;

                    // now just head in that direction by setting vector to the direction of that tile
                    // (remember the tile north of us may be on the other (south) end of the grid)
                    ActionTarget = closestTarget;
                }
                Velocity = Tile.VectorTo(Position, ActionTarget, Parent.World.Width, Parent.World.Height);
                //Debug.WriteLine("DisabledMobilitySensor2.Tick: Position({0},{1}), Target({2},{3}), Velocity({4},{5}), Sensor({6},{7}).",
                //    Parent.Center.X, Parent.Center.Y, ActionTarget.X, ActionTarget.Y,
                //    Velocity.X, Velocity.Y, Position.X, Position.Y);
                Action = World.Actions.actUseVelocity;
            }
        }

        private void AddTileTargets(Dictionary<PointF, float> tileTargets, Tile t)
        {
            if (!tileTargets.ContainsKey(t.Center))
                tileTargets[t.Center] = 0;
            foreach (DisabledMobilitySensorProxy s in t.Objects(typeof(DisabledMobilitySensorProxy)))
            {
                PointF target = (s.ActionTarget.IsEmpty) ? s.Parent.Center : s.ActionTarget;
                if (!tileTargets.ContainsKey(target))
                    tileTargets[target] = 0;
                if (target == s.Parent.Center)
                    tileTargets[target] = tileTargets[target] + 1;
                else
                    tileTargets[target] = tileTargets[target] + 0.5f;
            }
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "A mobile sensor that can stop moving after an assigned expiration date (number of ticks), but uses a proxy based algorithm for selecting a place to move to fill coverage holes and avoid other sensors."; }
    }
}
