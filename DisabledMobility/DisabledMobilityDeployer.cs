using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSim.Interface;

namespace DisabledMobility
{
    public class DisabledMobilityDeployer : Deployer
    {        
        /// <summary>
        /// Constructor for the deployer object.
        /// </summary>
        /// <param name="w"></param>
        public DisabledMobilityDeployer(World w) : base(w)
        {
        }

        /// <summary>
        /// This deployer should loop through the world's DisabledMobilitySensor inhabitants
        /// and set the expiration date of each of them so that over time they become
        /// disabled.
        /// </summary>
        public override void Deploy()
        {
            List<Tile> tiles = new List<Tile>();
            List<DisabledMobilitySensor> inhabitants = new List<DisabledMobilitySensor>();
            foreach (Tile t in World.Tiles.AllTiles)
            {
                tiles.Add(t);
                foreach (Inhabitant i in t.Objects(typeof(DisabledMobilitySensor)))
                    inhabitants.Add((DisabledMobilitySensor)i);
            }
            // now set the expiration on all of the (disabled-type) inhabitants
            double Sigma = 0.10d;
            double dMin = double.PositiveInfinity;
            double dMax = double.NegativeInfinity;
            foreach (DisabledMobilitySensor i in inhabitants)
            {
                double d = (World.Random.NextGaussian() * Sigma + 0.5) * 1000;

                dMin = Math.Min(dMin, d);
                dMax = Math.Max(dMax, d);

                i.Expiration = (int)d;
            }
            Debug.WriteLine("After deploying, expiration range is {0} to {1}.", dMin.ToString(), dMax.ToString());
        }

        public override string Info()
        {
            return "This deployer spreads all inhabitants evenly across the world surface.";
        }
    }
}
