using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Threading;

namespace WorldSim.Interface
{
    /// <summary>
    /// Needs to support:
    ///   - individuals asking if they are still inside region
    ///   - individuals asking for list of neighbor regions
    ///   - world asking if tile is visible (should be drawn)
    ///   - world asking tile to draw itself
    ///   - variety of tile shapes
    ///   - (recursive) generation of world one tile at a time
    /// </summary>
    [Serializable]
    public abstract class Tile : SelectableObject
    {
        private float m_fIncidentDistribution;
        public float IncidentDistribution { get { return m_fIncidentDistribution; } set { m_fIncidentDistribution = value; } }

        private float m_coverage;
        public float Coverage { get { return m_coverage; } set { m_coverage = value; } }
        public void IncrementCoverageSlightly(bool bIncrement = true) 
        { 
            if (m_coverage >= 0.0f && m_coverage < 1.0f) 
                m_coverage += (bIncrement ? 0.001f : -0.001f) * (1.0f - m_coverage); 
        }
        private bool m_polled;
        public bool Polled { get { return m_polled; } set { m_polled = value; } }

        /// <summary>
        /// This property represents the amount of some resource this tile has stockpiled.
        /// The value is represented as a percentage, or a value between 0.0 and 1.0.
        /// </summary>
        private float m_fResources;
        public float Resources
        {
            get { return m_fResources; }
            set
            {
                m_fResources = Math.Max(0.0f, Math.Min(1.0f, value)); 
                m_brushResources = new SolidBrush(BlendColors(ResourceColor, BackgroundColor, Resources));
            }
        }
        private Color m_clrResources;
        [NonSerialized]
        protected SolidBrush m_brushResources;

        public override void Tick()
        {
            base.Tick();
            // adjust coverage and resources

            if (HasObjects(typeof(Inhabitant)))
            {
                Polled = true;
                Coverage = 0.0f;
            }
            else
            {
                IncrementCoverageSlightly();
            }
        }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            m_brushResources = new SolidBrush(BlendColors(ResourceColor, BackgroundColor, Resources));
        }
        protected Color BlendColors(Color clrA, Color clrB, float fPercentOfA)
        {
            int R, G, B;
            R = (int)(ResourceColor.R * fPercentOfA + BackgroundColor.R * (1.0f-fPercentOfA));
            G = (int)(ResourceColor.G * fPercentOfA + BackgroundColor.G * (1.0f - fPercentOfA));
            B = (int)(ResourceColor.B * fPercentOfA + BackgroundColor.B * (1.0f - fPercentOfA));

            return Color.FromArgb(R, G, B);
        }
        public Color ResourceColor
        {
            get { return m_clrResources; }
            set
            {
                m_clrResources = value;
                m_brushResources = new SolidBrush(BlendColors(ResourceColor, BackgroundColor, Resources));
            }
        }
        public abstract IEnumerable<Tile> Neighbors
        {
            get;
        }

        /// <summary>
        /// Used to iterate all tiles from this starting point tile.
        /// </summary>
        public abstract IEnumerable<Tile> AllTiles
        {
            get;
        }
        
        public abstract int Sides { get; }
        private List<SelectableObject> m_objects;
        public Boolean HasObjects(Type T)
        {
            foreach (SelectableObject s in m_objects)
                if (T.IsAssignableFrom(s.GetType()))
                    return true;
            return false;
        }
        [CategoryAttribute("Behavior")]
        public IEnumerable<SelectableObject> Objects(Type T)
        {
            List<SelectableObject> list = new List<SelectableObject>();
            foreach (SelectableObject s in m_objects)
                if (T.IsAssignableFrom(s.GetType()))
                    list.Add(s);
            foreach (SelectableObject s in list)
                yield return s;
        }
        public IEnumerable<SelectableObject> NearbyObjects(Type T)
        {
            List<SelectableObject> list = new List<SelectableObject>();
            foreach (Tile t in Neighbors)
                foreach (SelectableObject m in t.Objects(T))
                    list.Add(m);
            foreach (SelectableObject m in this.Objects(T))
                list.Add(m);
            foreach (SelectableObject m in list)
                yield return m;
        }
        public void RemoveObject(SelectableObject o)
        {
            m_objects.Remove(o);
        }
        public void AddObject(SelectableObject o, bool force = false)
        {
            if (PointInRegion(o.Position) || force)
            {
                m_objects.Add(o);
                o.Parent = this;
            }
            else
                TileFromPoint(new Point((int)o.Position.X, (int)o.Position.Y)).AddObject(o, true);
        }

        public abstract Tile TileFromPoint(Point p);
        public abstract PointF RandomPoint(ECRandom r);

        public abstract bool IsVisible(Rectangle rectViewport, float scale);
        public abstract void Draw(Graphics g, Rectangle rectViewport, double scale, World.TileBackgroundIndicates coverage);
        public Tile()
            : base()
        {
            ForeColor = Color.Black;
            BackgroundColor = Color.Bisque;
            SelectedColor = Color.AliceBlue;
            ResourceColor = Color.LightGreen;
            Selected = false;
            m_objects = new List<SelectableObject>();
            m_fIncidentDistribution = 1.0f;
        }
    }
}
