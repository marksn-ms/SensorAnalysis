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
        public List<DisabledMobilitySensorWGB> Gray { get; set; }
        public List<DisabledMobilitySensorWGB> Black { get; set; }
        public TileColoring()
        {
            Gray = new List<DisabledMobilitySensorWGB>();
            Black = new List<DisabledMobilitySensorWGB>();
        }
    }

    [Serializable]
    public class DisabledMobilitySensorWGB : DisabledMobilitySensor
    {
        public Tile ActionTarget { get; set; }

        /// <summary>
        /// Constructor.  Set default values.
        /// </summary>
        public DisabledMobilitySensorWGB()
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

            Debug.Assert(Parent.PointInRegion(Position));

            Action = World.Actions.actStay;
            Velocity = PointF.Empty;
            if (ActionTarget == null)
                ActionTarget = Parent;

            if (Expiration == 0)
                return;

            if (/*(Ticks + ID) % 10 == 0 ||*/ World.Distance(PointF.Empty, Tile.VectorTo(Position, ActionTarget.Center, World.Width, World.Height, false)) < 5.0)
            {
                Dictionary<Tile, TileColoring> tileTargets = new Dictionary<Tile, TileColoring>();
                List<Tile> otherTiles = new List<Tile>();
                tileTargets.Add(Parent, new TileColoring());
                foreach (Tile t in Parent.Neighbors)
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

                // count how many occupants of nearby tiles have sensors with that tile as a target
                foreach (DisabledMobilitySensorWGB s in agents)
                {
                    Tile target = (s.ActionTarget == null) ? s.Parent : s.ActionTarget;
                    if (tileTargets.ContainsKey(target))
                    {
                        tileTargets[target].Black.Add(s);
                    }
                    foreach (Tile t in target.Neighbors)
                    {
                        if (tileTargets.ContainsKey(t))
                        {
                            tileTargets[t].Gray.Add(s);
                        }
                    }
                }

                Debug.Assert(tileTargets.Count >= 9);

                // by default, try to stay where we currently are
                ActionTarget = Parent;
                    
                int parentGrayCount = tileTargets[Parent].Gray.Count();
                int parentBlackCount = tileTargets[Parent].Black.Count();
                var whiteTiles = tileTargets
                    .Where(tt => tt.Value.Value == TileColoring.Coloring.White)
                    .Select(tt => tt.Key).ToArray();
                if (whiteTiles.Count() > 0)
                {
                    // if there's an unoccupied tile nearby and nobody headed that direction, we should head that way
                    ActionTarget = whiteTiles.First(); // [World.Random.Next(whiteTiles.Count())].Center;
                }
                else
                {
                    // if there are unoccupied tiles nearby that someone is headed towards, then we should consider
                    // heading that way only if our tile is overcrowded
                    var grayTiles = tileTargets
                        .Where(tt => tt.Value.Value == TileColoring.Coloring.Gray)
                        .OrderBy(tt => tt.Value.Gray.Count())
                        .Select(tt => tt.Key)
                        .ToArray();
                    if (grayTiles.Count() > 0)
                        ActionTarget = grayTiles.First();
                    else
                    {
                        // if there are occupied tiles nearby, then we should consider heading that way only if our tile is overcrowded
                        var blackTiles = tileTargets
                            .Where(tt => tt.Value.Value == TileColoring.Coloring.Black)
                            .OrderBy(tt => tt.Value.Black.Count())
                            .Select(tt => tt.Key).ToArray();
                        if (blackTiles.Count() > 0)
                            ActionTarget = blackTiles.First();
                    }
                }
            }

            // if we got here, we have a target to move towards, so compute the vector to it
            Velocity = Tile.VectorTo(Position, ActionTarget.Center, Parent.World.Width, Parent.World.Height);
            Action = World.Actions.actUseVelocity;
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "A mobile sensor that can stop moving after an assigned expiration date (number of ticks), but uses a tile coloring based algorithm for selecting a place to move to fill coverage holes and avoid other sensors."; }
    }
}
