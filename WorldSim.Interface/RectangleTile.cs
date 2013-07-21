using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace WorldSim.Interface
{
    [Serializable]
    public class RectangleTile : Tile
    {
        private RectangleTile m_north;
        public RectangleTile North
        {
            get { return m_north; }
            set { m_north = value; }
        }
        private RectangleTile m_south;
        public RectangleTile South
        {
            get { return m_south; }
            set { m_south = value; }
        }
        private RectangleTile m_east;
        public RectangleTile East
        {
            get { return m_east; }
            set { m_east = value; }
        }
        private RectangleTile m_west;
        public RectangleTile West
        {
            get { return m_west; }
            set { m_west = value; }
        }
        public override PointF Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                m_bRegionDirty = true;
            }
        }
        ///// <summary>Returns true if the specified point lies within the bounding rectangle of this object.</summary>
        ///// <param name="pt">The point to test (in world coordinates)</param>
        ///// <returns>True = the point lies within the bounding rectangle.</returns>
        //public override bool PointInRegion(PointF pt)
        //{
        //    if (m_bRegionDirty)
        //    {
        //        m_outline = GetGraphicsPath();
        //        m_region = new Region(m_outline);
        //        m_bRegionDirty = false;
        //    }
        //    return m_region.IsVisible(pt);
        //}
        [NonSerialized]
        private Region m_region;
        [NonSerialized]
        private GraphicsPath m_outline;
        public override IEnumerable<Tile> Neighbors
        {
            get
            {
                List<Tile> neighbors = new List<Tile>();
                neighbors.Add(North);
                neighbors.Add(North.East);
                neighbors.Add(East);
                neighbors.Add(East.South);
                neighbors.Add(South);
                neighbors.Add(South.West);
                neighbors.Add(West);
                neighbors.Add(West.North);
                foreach (Tile t in neighbors)
                    yield return t;
            }
        }
        public override IEnumerable<Tile> AllTiles
        {
            get 
            {
                // process this row
                RectangleTile rowStart = (RectangleTile)this;
                do
                {
                    RectangleTile nextEast = rowStart;
                    do
                    {
                        yield return nextEast;
                        nextEast = nextEast.East;
                    } while (!object.ReferenceEquals(nextEast,rowStart));
                    rowStart = rowStart.South;
                } while (!object.ReferenceEquals(rowStart,this));
            }
        }
        public override int Sides { get { return 4; } }
        public static Tile Build(World w, Size size, Size sizeTile)
        {
            List<List<Tile>> sheet = new List<List<Tile>>();
            List<Tile> list = new List<Tile>();
            // sheet[row][col]
            for (int i = 0, y = 0; i < size.Height; i++, y+=sizeTile.Height)
            {
                sheet.Add(new List<Tile>()); // add row
                for (int j = 0, x = 0; j < size.Width; j++, x+=sizeTile.Width)
                {
                    RectangleTile rtNew = new RectangleTile(new Point(x, y), sizeTile);
                    rtNew.World = w;
                    list.Add((Tile)rtNew);
                    sheet[i].Add(rtNew);
                    //Debug.Write(string.Format("({0},{1})-", rtNew.Position.X, rtNew.Position.Y));
                }
                //Debug.WriteLine("");
            }
            // now hook up the references to neighbors
            for (int i = 0; i < size.Height; i++)
            {
                for (int j = 0; j < size.Width; j++)
                {
                    RectangleTile rt = (RectangleTile)sheet[j][i];
                    rt.North = (RectangleTile)sheet[j][(i + size.Height - 1) % size.Height];
                    rt.South = (RectangleTile)sheet[j][(i + 1) % size.Height];
                    rt.West = (RectangleTile)sheet[(j + size.Width - 1) % size.Width][i];
                    rt.East = (RectangleTile)sheet[(j + 1) % size.Width][i];
                }
            }
            return list[0];
        }
        public override bool IsVisible(Rectangle rectViewport, float scale)
        {
            Rectangle rectDraw = new Rectangle((int)(Position.X * scale), (int)(Position.Y * scale), (int)(Size.Width * scale), (int)(Size.Height * scale));
            return rectDraw.IntersectsWith(rectViewport);
        }

        public override void Draw(Graphics g, Rectangle rectViewport, double scale, World.TileBackgroundIndicates coverage)
        {
            if (m_bRegionDirty)
            {
                m_outline = GetGraphicsPath();
                m_region = new Region(m_outline);
                m_bRegionDirty = false;
            }

            // since we're drawing a rectangle, this rectangle is the bounding rectangle
            Matrix m = new Matrix();
            m.Scale((float)scale, (float)scale);
            m.Translate(-rectViewport.X, -rectViewport.Y);

            GraphicsPath pathDraw = new GraphicsPath();
            pathDraw.AddPath(m_outline, false);
            pathDraw.Transform(m);

            Region rgnDraw = new Region(pathDraw);
            //rgnDraw.Intersect(rectViewport);

            Brush brush = m_brushBack;
            switch(this.World.TileBackgroundIndication)
            {
                case Interface.World.TileBackgroundIndicates.tbiNone:
                    brush = Selected ? m_brushSelected : m_brushBack;
                    break;
                case Interface.World.TileBackgroundIndicates.tbiResources:
                    brush = new SolidBrush(BlendColors(ResourceColor, BackgroundColor, Resources));
                    break;
                case Interface.World.TileBackgroundIndicates.tbiStale:
                    brush = new SolidBrush(BlendColors(ResourceColor, BackgroundColor, Coverage));
                    break;
                case Interface.World.TileBackgroundIndicates.tbiPoll:
                    brush = Polled ? m_brushSelected : m_brushBack;
                    break;
            }

            g.FillRegion(brush, rgnDraw);
            g.DrawPath(new Pen(ForeColor), pathDraw);

            if (Selected)
            {
                Pen pDotted = new Pen(ForeColor, 2.0f);
                pDotted.DashStyle = DashStyle.Dot;
                g.DrawPath(pDotted, pathDraw);
            }
        }

        [NonSerialized]
        private bool m_bRegionDirty;
        public RectangleTile(Point ptOrigin, Size size)
            : base()
        {
            Position = ptOrigin;
            Size = size;
            m_north = m_south = m_east = m_west = this;
            m_bRegionDirty = true;
            ForeColor = Color.FromArgb(255,255,200);
        }

        private GraphicsPath GetGraphicsPath()
        {
            GraphicsPath g = new GraphicsPath();
            g.AddRectangle(new Rectangle((int)Position.X, (int)Position.Y, (int)Size.Width, (int)Size.Height));
            return g;
        }

        /// <summary>
        /// This method returns the tile containing the specified point, even if it overlaps
        /// out of this tile and into another.  If that point is outside the bounds of the
        /// collection of tiles, then it will assume wrapping and resolve anyway.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override Tile TileFromPoint(Point p)
        {
            Point pp = new Point(p.X % (World.TileSize.Width * World.Size.Width),
                p.Y % (int)(World.TileSize.Height * Math.Sqrt(3.0d) * World.Size.Height));
            if (PointInRegion(pp))
                return this;
            foreach (var t in AllTiles)
            {
                if (t.PointInRegion(pp))
                    return t;
            }
            throw new ApplicationException(String.Format("Not able to locate a tile containing point ({0},{1}).", p.X, p.Y));
        }

        public override PointF RandomPoint(ECRandom r)
        {
            return new PointF(Position.X + (float)r.NextDouble() * (this.Size.Width - 1), Position.Y + (float)r.NextDouble() * (this.Size.Height - 1));
        }
    }
}
