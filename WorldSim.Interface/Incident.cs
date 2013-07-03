using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace WorldSim.Interface
{
    /// <summary>
    /// This class represents a "happening" within the world.  Its purpose is to
    /// be something like a goal for the agents.  Depending on the encoded behavior
    /// of the agents, they may seek it out, hover near it, avoid it, etc.  Based
    /// on the properties of the event, it may be static or moving, constant or
    /// intermittent.
    /// </summary>
    [Serializable]
    public class Incident : SelectableObject
    {
        /// <summary>
        /// The maximum number of turns for which this incident will
        /// be in one spot before popping up somewhere else.
        /// </summary>
        private int m_nMaxTurnsUntilMove;
        public int MaxTurnsUntilMove
        {
            get { return m_nMaxTurnsUntilMove; }
            set { m_nMaxTurnsUntilMove = value; }
        }

        private int m_nTurnsUntilMove;

        private Color m_clrWave;
        protected Pen m_penWave;
        public Color WaveColor
        {
            get { return m_clrWave; }
            set
            {
                m_clrWave = value;
                m_penWave = new Pen(m_clrWave);
            }
        }
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
                m_brushResources = new SolidBrush(BlendColors(ResourceColor, WaveColor, Resources));
                m_penWave = new Pen(BlendColors(ResourceColor, BackgroundColor, Resources));
            }
        }
        private Color m_clrResources;
        protected SolidBrush m_brushResources;
        private Color BlendColors(Color clrA, Color clrB, float fPercentOfA)
        {
            int R, G, B;
            R = (int)(ResourceColor.R * fPercentOfA + BackgroundColor.R * (1.0f - fPercentOfA));
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
                m_penWave = new Pen(BlendColors(ResourceColor, BackgroundColor, Resources));
            }
        }

        public override void Tick() 
        {
            if (m_nMaxTurnsUntilMove < 32767)
            {
                if (m_nTurnsUntilMove > 32767)
                    m_nTurnsUntilMove = World.Random.Next(m_nMaxTurnsUntilMove);

                // check to see if we need to move
                if (m_nTurnsUntilMove <= 0)
                {
                    m_nTurnsUntilMove = World.Random.Next(m_nMaxTurnsUntilMove);
                    //int dx = World.Random.Next(World.Width);
                    //int dy = World.Random.Next(World.Height);

                    float dRW = (float)World.Random.NextDouble();
                    Tile tMove = World.Tiles;
                    foreach (Tile t in World.Tiles.AllTiles)
                    {
                        dRW -= t.IncidentDistribution;
                        if (dRW <= 0.0f)
                        {
                            tMove = t;
                            break;
                        }
                    }

                    PointF p = tMove.Position;
                    p.X += World.Random.Next(World.TileSize.Width);
                    p.Y += World.Random.Next(World.TileSize.Height);
                    //Trace.WriteLine(string.Format("Moving incident from ({0},{1}) to ({2},{3}).", Position.X, Position.Y, p.X, p.Y));

                    OnRelocationRequested((int)p.X, (int)p.Y);
                }

                m_nTurnsUntilMove--;
            }
        }

        public Incident()
            : base()
        {
            Initialize();
        }
        public Incident(PointF p, PointF v)
            : base()
        {
            Initialize();
        }
        public Incident(PointF p, PointF v, String strLabel)
            : base()
        {
            Initialize();
        }
        private void Initialize()
        {
            Size = new Size(10, 10);
            ForeColor = Color.Blue;
            BackgroundColor = Color.Gray;
            Symbol = 'x';
            WaveColor = Color.LightGray;
            ResourceColor = Color.Red;
            m_nMaxTurnsUntilMove = Int32.MaxValue;
            m_nTurnsUntilMove = Int32.MaxValue;
        }
        public virtual bool IsVisible(Rectangle rectViewport, float scale)
        {
            Rectangle rectDraw = new Rectangle((int)(Position.X * scale), (int)(Position.Y * scale), (int)(Size.Width * scale), (int)(Size.Height * scale));
            return rectDraw.IntersectsWith(rectViewport);
        }

        public bool PointInRegion(Point pt)
        {
            return pt.X >= Position.X && pt.X < Position.X + Size.Width
                && pt.Y >= Position.Y && pt.Y < Position.Y + Size.Height;
        }

        public override void Draw(Graphics g, Rectangle rectViewport, float scale)
        {
            Rectangle rectDraw = new Rectangle((int)(Position.X * scale), (int)(Position.Y * scale), (int)(this.Size.Width * scale), (int)(this.Size.Height * scale));
            rectDraw.Intersect(rectViewport);
            rectDraw.Offset(-rectViewport.X, -rectViewport.Y);
            if (Selected || (Parent != null && Parent.Selected))
                g.FillEllipse(m_brushSelected, rectDraw);
            else
                g.FillEllipse(m_brushResources, rectDraw);
            g.DrawEllipse(m_penFore, rectDraw);
            rectDraw.Inflate((int)(Size.Width * scale), (int)(Size.Height * scale));
            g.DrawEllipse(m_penWave, rectDraw);
            rectDraw.Inflate((int)(Size.Width * scale), (int)(Size.Height * scale));
            g.DrawEllipse(m_penWave, rectDraw);
        }

        public class RelocationRequestedEventArgs : EventArgs
        {
            public Incident Incident;
            public System.Drawing.Point NewPosition;
            public RelocationRequestedEventArgs(int X, int Y, Incident i)
            {
                NewPosition = new System.Drawing.Point(X, Y);
                Incident = i;
            }
        }
        public delegate void RelocationRequestedEventDelegate(object sender, RelocationRequestedEventArgs e);
        public event RelocationRequestedEventDelegate RelocationRequestedEvent;

        public void OnRelocationRequested(int dx, int dy)
        {
            if (RelocationRequestedEvent != null)
                RelocationRequestedEvent(this, new RelocationRequestedEventArgs(dx, dy, this));
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "Something that happens in the world."; }
    }
}
