using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Interface
{
    public class RandomDeployer : Deployer
    {
        /// <summary>
        /// Constructor for the deployer object.
        /// </summary>
        /// <param name="w"></param>
        public RandomDeployer(World w) : base(w)
        {
        }

        public override void Deploy()
        {
            List<Tile> tiles = new List<Tile>();
            List<Inhabitant> inhabitants = new List<Inhabitant>();
            foreach (Tile t in World.Tiles.AllTiles)
            {
                tiles.Add(t);
                foreach (Inhabitant i in t.Objects(typeof(Inhabitant)))
                    inhabitants.Add(i);
            }
            // now spread the inhabitants evenly throughout the tiles
            foreach (Inhabitant i in inhabitants)
            {
                int whichTile = World.Random.Next(tiles.Count - 1);
                Tile receiveInhabitant = tiles[whichTile];
                i.Parent.RemoveObject(i);
                i.Position = receiveInhabitant.RandomPoint(World.Random);
                receiveInhabitant.AddObject(i);
            }
        }

        public override string Info()
        {
            return "This deployer spreads all inhabitants evenly across the world surface.";
        }
    }
}
