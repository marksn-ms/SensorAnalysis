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
    public class HexagonTile : Tile
    {
        public HexagonTile North { get; set; }
        public HexagonTile South { get; set; }
        public HexagonTile Northeast { get; set; }
        public HexagonTile Northwest { get; set; }
        public HexagonTile Southeast { get; set; }
        public HexagonTile Southwest { get; set; }

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

        /// <summary>Returns true if the specified point lies within the bounding rectangle of this object.</summary>
        /// <param name="pt">The point to test (in world coordinates)</param>
        /// <returns>True = the point lies within the bounding rectangle.</returns>
        public override bool PointInRegion(PointF pt)
        {
            if (m_bRegionDirty)
            {
                m_outline = GetGraphicsPath();
                m_region = new Region(m_outline);
                m_bRegionDirty = false;
            }
            return m_region.IsVisible(pt);
            //PointF q2 = new PointF(Math.Abs(pt.X - Center.X), Math.Abs(pt.Y - Center.Y));
            //if (q2.X >= Size.Width || q2.Y > Size.Width) 
            //    return false;
            //float dot = 2 * Size.Width * Size.Height - Size.Width * q2.X - 2 * Size.Height * q2.Y;
            //return dot >= 0;
        }
        private Region m_region;
        private GraphicsPath m_outline;
        public override IEnumerable<Tile> Neighbors
        {
            get
            {
                yield return North;
                yield return Northeast;
                yield return Southeast;
                yield return South;
                yield return Southwest;
                yield return Northwest;
            }
        }
        public override IEnumerable<Tile> AllTiles
        {
            get
            {
                bool firstRow = true;
                HexagonTile current = (HexagonTile)this;
                for (; firstRow || current.Position.Y != ((HexagonTile)this).Position.Y; current = current.South)
                {
                    firstRow = false;
                    bool firstCol = true;
                    for (; firstCol || current.Position.X != ((HexagonTile)this).Position.X; current = current.Southeast)
                    {
                        firstCol = false;
                        yield return current;
                    }
                }

/*
                // process this row
                HexagonTile rowStart = (HexagonTile)this;
                do
                {
                    HexagonTile nextEast = rowStart;
                    do
                    {
                        yield return nextEast;
                        nextEast = nextEast.Southeast;
                    } while (nextEast.Position.X > rowStart.Position.X);
                    rowStart = rowStart.South;
                } while (rowStart.Position.Y > this.Position.Y);
 */
            }
        }
        public override int Sides { get { return 6; } }
        public static Tile Build(World w, Size tiles, Size size)
        {
            // make sure tile proportion specified is correct -- compute Height from Width
            SizeF tileSize = new SizeF(size.Width, (int)(Math.Sqrt(3) * size.Width / 2));
            Size numTiles = new Size((int)(Math.Ceiling(2.0 * tiles.Width) / 2), (int)(Math.Ceiling((2.0 * tiles.Height) / 2)));

            double deltaX = tileSize.Width * 0.75;  // second row starts staggered to the right 3/4 the width of a tile
            double deltaY = tileSize.Height / 2; // second row starts staggered down half the height of a tile

            List<List<Tile>> sheet = new List<List<Tile>>();
            List<Tile> list = new List<Tile>();
            
            // sheet[row][col]
            double y = 0.0;
            for (int i = 0; i < numTiles.Height; i++)
            {
                sheet.Add(new List<Tile>()); // add row
                double x = 0.0 + i % 2 * deltaX;
                for (int j = 0; j < numTiles.Width; j++)
                {
                    HexagonTile newTile = new HexagonTile(new PointF((float)x, (float)y), tileSize);
                    newTile.World = w;
                    list.Add((Tile)newTile); // add column to this row
                    sheet[i].Add(newTile);
                    x = x + 2 * deltaX; // this position toggles back and forth
                }
                y = y + tileSize.Height / 2;
            }
            // now hook up the references to neighbors
            for (int i = 0; i < numTiles.Height; i++) // rows
            {
                for (int j = 0; j < numTiles.Width; j++) // cols
                {
                    HexagonTile ht = (HexagonTile)sheet[i][j];

                    Point ptNorth = new Point((i + numTiles.Height - 2) % numTiles.Height, j);
                    Point ptSouth = new Point((i + 2) % numTiles.Height, j);
                    Point ptNorthwest = new Point((i + numTiles.Height - 1) % numTiles.Height, (j + numTiles.Width - (i + 1) % 2) % numTiles.Width);
                    Point ptSoutheast = new Point((i + 1) % numTiles.Height, (i % 2 + j) % numTiles.Width);
                    Point ptSouthwest = new Point((i + 1) % numTiles.Height, (j + numTiles.Width - (i + 1) % 2) % numTiles.Width);
                    Point ptNortheast = new Point((i + numTiles.Height - 1) % numTiles.Height, (i % 2 + j) % numTiles.Width);

                    // two rows prior; same column
                    ht.North = (HexagonTile)sheet[ptNorth.X][ptNorth.Y];
                    // two rows further; same column
                    ht.South = (HexagonTile)sheet[ptSouth.X][ptSouth.Y];
                    // previous row; for even rows, prior column, for odd rows, same column
                    ht.Northwest = (HexagonTile)sheet[ptNorthwest.X][ptNorthwest.Y];
                    // next row; for even rows, same column, for odd rows, next column
                    ht.Southeast = (HexagonTile)sheet[ptSoutheast.X][ptSoutheast.Y];
                    // next row; for even rows, prior column, for odd rows, same column
                    ht.Southwest = (HexagonTile)sheet[ptSouthwest.X][ptSouthwest.Y];
                    // prior row; for even rows, same column, for odd rows, next column
                    ht.Northeast = (HexagonTile)sheet[ptNortheast.X][ptNortheast.Y];
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

            // since we're drawing a hexagon, this rectangle is the bounding rectangle
            Matrix m = new Matrix();
            m.Scale((float)scale, (float)scale);
            m.Translate(-rectViewport.X, -rectViewport.Y);

            GraphicsPath pathDraw = new GraphicsPath();
            pathDraw.AddPath(m_outline, false);
            pathDraw.Transform(m);

            Region rgnDraw = new Region(pathDraw);
            //rgnDraw.Intersect(rectViewport);

            Brush brush;
            if (Selected)
                brush = m_brushSelected;
            else
                brush = m_brushBack;

            g.FillRegion(brush, rgnDraw);
            g.DrawPath(new Pen(ForeColor), pathDraw);

            if (Selected)
            {
                Pen pDotted = new Pen(ForeColor, 2.0f);
                pDotted.DashStyle = DashStyle.Dot;
                g.DrawPath(pDotted, pathDraw);
            }
        }

        private bool m_bRegionDirty;

        public HexagonTile(PointF ptOrigin, SizeF size)
            : base()
        {
            Position = ptOrigin;
            Size = size;
            North = South = Northeast = Northwest = Southeast = Southwest = this;
            m_bRegionDirty = true;
            ForeColor = Color.FromArgb(255, 255, 200);
        }

        private GraphicsPath GetGraphicsPath()
        {
            int x0 = (int)(Size.Width * 0.0), 
                x1 = (int)(Size.Width * 0.25), 
                x2 = (int)(Size.Width * 0.5),
                x3 = (int)(Size.Width * 0.75), 
                x4 = (int)(Size.Width * 1.0);
            int y0 = (int)(Size.Height * 0.0), 
                y1 = (int)(Size.Height / 2),
                y2 = (int)(Size.Height); 

            GraphicsPath g = new GraphicsPath();
            Point[] p = new Point[] 
            { 
                new Point((int)Position.X + x1, (int)Position.Y + y0), 
                new Point((int)Position.X + x3, (int)Position.Y + y0),
                new Point((int)Position.X + x4, (int)Position.Y + y1),
                new Point((int)Position.X + x3, (int)Position.Y + y2),
                new Point((int)Position.X + x1, (int)Position.Y + y2),
                new Point((int)Position.X + x0, (int)Position.Y + y1)
            };
            g.AddPolygon(p);
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
            PointF p1 = Position;
            HexagonTile t = this;
            while (p.X < p1.X) // in case it's north of us
            {
                t = t.North;
                p1.X -= World.TileSize.Height;
            }
            while (p.X > p1.X + World.TileSize.Height) // in case it's south of us
            {
                t = t.South;
                p1.X += World.TileSize.Height;
            }
            // we have the right row now
            while (p.Y < p1.Y) // in case it's west of us
            {
                if (p.X < p1.X + World.TileSize.Height / 2)
                {
                    t = t.Northwest;
                    p1.X -= World.TileSize.Height / 2;
                }
                else
                {
                    t = t.Southwest;
                    p1.X += World.TileSize.Height / 2;
                }
                p1.Y -= World.TileSize.Width;
            }
            while (p.Y > p1.Y + World.TileSize.Width) // in case it's east of us
            {
                if (p.X < p1.X + World.TileSize.Height / 2)
                {
                    t = t.Northeast;
                    p1.X -= World.TileSize.Height / 2;
                }
                else
                {
                    t = t.Southeast;
                    p1.X += World.TileSize.Height / 2;
                }
                p1.Y += World.TileSize.Width;
            }
            // the point is within the pseudo bounding rectangle of this tile
            if (t.PointInRegion(p))
                return t;
            // if we're still here, it's either in the NE, NW, SE, or SW tile from t
            if (p.X < t.Center.X)
            {
                if (p.Y < t.Center.Y)
                    return t.Northwest;
                else
                    return t.Southwest;
            }
            else
            {
                if (p.Y < t.Center.Y)
                    return t.Northeast;
                else
                    return t.Southeast;
            }
#if false
            Point pp = new Point(p.X % (World.TileSize.Width * World.Size.Width),
                p.Y % (int)(World.TileSize.Height * Math.Sqrt(3.0d) * World.Size.Height));
            if (PointInRegion(pp))
                return this;
            Tile closest = null;
            double distanceClosest = double.PositiveInfinity;
            foreach (var t in AllTiles)
            {
                double distance = World.Distance(p, t.Center);
                if (distance < distanceClosest)
                {
                    closest = t;
                    distanceClosest = distance;
                }
                if (t.PointInRegion(pp))
                    return t;
            }
            return closest;
#endif
            throw new ApplicationException(String.Format("Not able to locate a tile containing point ({0},{1}).", p.X, p.Y));
        }

        public override SizeF Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                // the (approximate) height of a hexagon is a function of the width, so change that here
                base.Size = new SizeF(value.Width, (int)(Math.Sqrt(3) * value.Width / 2));
            }
        }

        public override PointF RandomPoint(ECRandom r)
        {
            PointF p = new PointF();
            float x = (float)(r.NextDouble() * Math.Floor(0.75 * Size.Width - 1));
            float y = (float)(r.NextDouble() * Math.Floor(Size.Height - 1));
            if (x > 0.75 * Size.Width || y > Size.Height)
                throw new ApplicationException("WTF is the deal with random?!?!");
            p.X = Position.X + x;
            p.Y = Position.Y + y;
            if (!PointInRegion(p))
            {
                p.X = Position.X + x + (float)(0.75 * Size.Width);
                p.Y = Position.Y + ((y < Size.Height / 2) ? y + (float)Size.Height / 2 : y - (float)Size.Height / 2);
                if (!PointInRegion(p))
                {
                    throw new ApplicationException("Foobar!  How come my calculations don't produce a point within this hexagon?!?!");
                }
            }

            return p;
        }
    }
}
