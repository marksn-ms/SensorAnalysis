using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace WorldSim.Interface
{
    public class GaussianDeployer : Deployer
    {
        /// <summary>
        /// Constructor for the deployer object.
        /// </summary>
        /// <param name="w"></param>
        public GaussianDeployer(World w)
            : base(w)
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

            DeployAroundPoint(World.Random, 0.10d, tiles, inhabitants);

            // now spread the inhabitants evenly throughout the tiles
            //foreach (Inhabitant i in inhabitants)
            //{
            //    int whichTile = World.Random.Next(tiles.Count - 1);
            //    Tile receiveInhabitant = tiles[whichTile];
            //    i.Parent.RemoveObject(i);
            //    i.Position = receiveInhabitant.RandomPoint(World.Random);
            //    receiveInhabitant.AddObject(i);
            //}
        }

        private void DeployAroundPoint(ECRandom r, double Sigma, List<Tile> tiles, List<Inhabitant> inhabitants)
        {
            // find center point
            if (tiles == null || tiles.Count == 0 || inhabitants == null || inhabitants.Count == 0)
                return;
            PointF center = new PointF();
            PointF bounds = new PointF();
            foreach (Tile t in tiles)
            {
                center.X += t.Center.X;
                center.Y += t.Center.Y;
                bounds.X = Math.Max(t.Center.X, bounds.X);
                bounds.Y = Math.Max(t.Center.Y, bounds.Y);
            }
            center.X /= tiles.Count;
            center.Y /= tiles.Count;
            center.X = center.Y = Math.Min(center.X, center.Y);
            
            bool bRepeat = false;
            int nRepeats = 0;
            foreach (Inhabitant i in inhabitants)
            {
                bRepeat = false;
                bool bPlaced = false;
                do // agents could land outside the grid.  Repeat until coordinates inside the grid.
                {
                    if (bRepeat) nRepeats++;
                    bRepeat = true;
                    PointF place = new PointF((float)(r.NextGaussian() * Sigma + 0.5) * bounds.X,
                        (float)(r.NextGaussian() * Sigma + 0.5) * bounds.Y);

                    // see if we can place new point in a tile
                    bPlaced = false;
                    foreach(Tile t in tiles)
                    {
                        if (t.PointInRegion(place))
                        {
                            i.Parent.RemoveObject(i);
                            i.Position = place;
                            t.AddObject(i);
                            bPlaced = true;
                        }
                    }

                } while (!bPlaced);
            }
            Debug.WriteLine(String.Format("Picking random cell was {0}% efficient ({1}/{2}) with Sigma={3}.", 
                ((int)(100.0d - 100.0d * nRepeats / inhabitants.Count)).ToString(), nRepeats, inhabitants.Count, Sigma));
        }

        public override string Info()
        {
            return "This deployer spreads all inhabitants around a point in the center of the world using a Gaussian distribution.";
        }
    }
}
