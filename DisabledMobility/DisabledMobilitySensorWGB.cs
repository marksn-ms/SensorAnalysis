﻿using System;
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
    public class TileColoring
    {
        public enum Coloring { White, Gray, Black }
        public Coloring Value
        {
            get 
            {
                if (Black.Count() > 0)
                    return Coloring.Black;
                else if (Gray.Count() > 0)
                    return Coloring.Gray;
                else
                    return Coloring.White;
            }
        }
        public List<DisabledMobilitySensorRandomWalk> Gray { get; set; }
        public List<DisabledMobilitySensorRandomWalk> Black { get; set; }
        public TileColoring()
        {
            Gray = new List<DisabledMobilitySensorRandomWalk>();
            Black = new List<DisabledMobilitySensorRandomWalk>();
        }
    }

    [Serializable]
    public class DisabledMobilitySensorWGB : DisabledMobilitySensorRandomWalk
    {
        private List<Tile> m_previousTargets;
        public Tile ActionTarget 
        { 
            get 
            {
                if (m_previousTargets == null || m_previousTargets.Count() == 0)
                    return null;
                return m_previousTargets.Last();
            } 
            set
            {
                if (m_previousTargets == null)
                    m_previousTargets = new List<Tile>();
                if (m_previousTargets.Count() == 0 || m_previousTargets.Last() != value)
                    m_previousTargets.Add(value);
                while (m_previousTargets.Count() > 20)
                    m_previousTargets.RemoveAt(0);
            } 
        }

        /// <summary>
        /// Constructor.  Set default values.
        /// </summary>
        public DisabledMobilitySensorWGB()
            : base()
        {
            Expiration = -1;
        }

        const bool bMitigate = false;

        /// <summary>
        /// As time (ticks) progress, the odds that the mobility of this
        /// agent will fail increase.  Once the sensor expires, it stops
        /// moving.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            Debug.Assert(Parent.PointInRegion(Position));

            if (ActionTarget == null)
                ActionTarget = Parent;

            // indicate where we intend to go next
            ActionTarget = GetActionTarget(Parent, ActionTarget, Position, World);

            // if we got here, we have a target to move towards, so compute the vector to it
            if ((Expiration == 0) || World.Distance(Position, ActionTarget.Center) < 5.0)
            {
                Velocity = PointF.Empty;
                Action = World.Actions.actStay;
            }
            else
            {
                Velocity = Tile.VectorTo(Position, ActionTarget.Center, Parent.World.Width, Parent.World.Height);
                Action = World.Actions.actUseVelocity;
            }
        }

        private int idToTrace = -1;
        private Tile GetActionTarget(Tile myParent, Tile currentActionTarget, PointF currentPosition, World world)
        {
            Tile newActionTarget = currentActionTarget;
            double distanceToTarget = World.Distance(PointF.Empty, Tile.VectorTo(currentPosition, currentActionTarget.Center, world.Width, world.Height, false));
            if (distanceToTarget < 5.0)
            {
                if (this.ID == idToTrace)
                    Debug.WriteLine("37 - arrived; position({0},{1}), target({2},{3}), distance({4}).", 
                        Position.X, Position.Y, currentActionTarget.Center.X, currentActionTarget.Center.Y, distanceToTarget);

                Dictionary<Tile, TileColoring> tileTargets = new Dictionary<Tile, TileColoring>();
                List<Tile> otherTiles = new List<Tile>();
                tileTargets.Add(myParent, new TileColoring());
                foreach (Tile t in myParent.Neighbors)
                {
                    tileTargets.Add(t, new TileColoring());
                }

                List<DisabledMobilitySensorWGB> agents = new List<DisabledMobilitySensorWGB>();
                foreach (Tile t in tileTargets.Keys)
                {
                    foreach (DisabledMobilitySensorWGB s in t.Objects(typeof(DisabledMobilitySensorWGB)))
                        agents.Add(s);
                    foreach (Tile tt in t.Neighbors)
                    {
                        if (!tileTargets.ContainsKey(tt) && !otherTiles.Contains(tt))
                        {
                            otherTiles.Add(tt);
                            foreach (DisabledMobilitySensorWGB s in tt.Objects(typeof(DisabledMobilitySensorWGB)))
                                agents.Add(s);
                        }
                    }
                }

                // count how many occupants of nearby tiles have that tile as a target
                foreach (DisabledMobilitySensorWGB s in agents)
                {
                    Tile target = (s.ActionTarget == null) ? s.Parent : s.ActionTarget;
                    if (tileTargets.ContainsKey(target))
                        tileTargets[target].Black.Add(s);

                    foreach (Tile t in target.Neighbors)
                    {
                        if (tileTargets.ContainsKey(t))
                            tileTargets[t].Gray.Add(s);
                    }
                }

                Debug.Assert(tileTargets.Count >= 9);

                int parentGrayCount = tileTargets[myParent].Gray.Count();
                int parentBlackCount = tileTargets[myParent].Black.Count();
                var tiles = tileTargets
                    .OrderBy(tt => tt.Value.Value)
                    .ThenBy(tt => tt.Value.Gray.Count())
                    .ThenBy(tt => tt.Value.Black.Count())
                    .ThenBy(tt => tt.Key.Center.X)
                    .ThenBy(tt => tt.Key.Center.Y);

                int nThisTileBlack = tileTargets[myParent].Black.Count();
                int nThisTileGray = tileTargets[myParent].Gray.Count();
                int nFirstTargetBlack = tiles.First().Value.Black.Count();
                int nFirstTargetGray = tiles.First().Value.Gray.Count();

                // now we either stay put or go to the first tile in the list
                if (nThisTileBlack > nFirstTargetBlack)
                    newActionTarget = tiles.First().Key;
                else if ((nThisTileBlack == nFirstTargetBlack)
                    && (nThisTileGray > nFirstTargetGray))
                    newActionTarget = tiles.First().Key;
                else
                    newActionTarget = myParent;

                if (this.ID == idToTrace)
                    Debug.WriteLine("37 - picked new target; position({0},{1}), target({2},{3}).",
                        Position.X, Position.Y, currentActionTarget.Center.X, currentActionTarget.Center.Y);
            }

            // by default, try to stay where we currently are
            return newActionTarget;
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "A mobile sensor that can stop moving after an assigned expiration date (number of ticks), but uses a tile coloring based algorithm for selecting a place to move to fill coverage holes and avoid other sensors."; }
    }
}
