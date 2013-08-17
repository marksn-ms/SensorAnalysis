using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace WorldSim.Interface
{
    /// <summary>
    /// Base class for all selectable objects.  This object encapsulates all of the
    /// properties and methods required to draw an object that can be selected.
    /// </summary>
    [Serializable]
    public class SelectableObject
    {
        // class members
        private Color m_clrFore;
        private Color m_clrBack;
        private Color m_clrSelected;
        private PropertyBag m_bag;
        public PropertyBag Properties
        {
            get { return m_bag; }
        }

        [NonSerialized]
        protected Pen m_penFore;
        [NonSerialized]
        protected Brush m_brushBack;
        [NonSerialized]
        protected Brush m_brushSelected;

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            m_penFore = new Pen(ForeColor);
            m_brushBack = new SolidBrush(BackgroundColor);
            m_brushSelected = new SolidBrush(SelectedColor);
        }

        /// <summary>Foreground color, used for drawing lines and borders around object.</summary>
        [CategoryAttribute("Appearance")]
        public Color ForeColor { get { return m_clrFore; } set { m_clrFore = value; m_penFore = new Pen(value); } }
        /// <summary>Background color, used for filling the region occupied by the object.</summary>
        [CategoryAttribute("Appearance")]
        public Color BackgroundColor { get { return m_clrBack; } set { m_clrBack = value; m_brushBack = new SolidBrush(value); } }
        /// <summary>When the object is selected, this color is used for filling the region occupied by the object.</summary>
        [CategoryAttribute("Appearance")]
        public Color SelectedColor { get { return m_clrSelected; } set { m_clrSelected = value; m_brushSelected = new SolidBrush(value); } }
        /// <summary>Returns true if this object is currently selected.</summary>
        [CategoryAttribute("Appearance"), ReadOnlyAttribute(true)]
        public bool Selected { get; set; }
        /// <summary>Returns true if this object currently has the focus.</summary>
        [CategoryAttribute("Appearance"), ReadOnlyAttribute(true)]
        public bool Focus { get; set; }
        /// <summary>The position (in world coordinates) of this object.</summary>
        [CategoryAttribute("Behavior")]
        public virtual PointF Position { get; set; }
        /// <summary>Returns the Size (width and height) of this object (in world coordinates).</summary>
        [CategoryAttribute("Appearance")]
        public virtual SizeF Size { get; set; }
        /// <summary>Returns the point that is the center of this object's bounding sphere.</summary>
        [CategoryAttribute("Behavior"), ReadOnlyAttribute(true)]
        public PointF Center
        {
            get { return new PointF(Position.X + Size.Width / 2, Position.Y + Size.Height / 2); }
        }
        /// <summary>Returns true if the specified point lies within the bounding rectangle of this object.</summary>
        /// <param name="pt">The point to test (in world coordinates)</param>
        /// <returns>True = the point lies within the bounding rectangle.</returns>
        public virtual bool PointInRegion(PointF pt)
        {
            return pt.X >= Position.X && pt.X < Position.X + Size.Width
                && pt.Y >= Position.Y && pt.Y < Position.Y + Size.Height;
        }
        /// <summary>Returns the area of the world that the bounding sphere of this object covers.</summary>
        [CategoryAttribute("Behavior"), ReadOnlyAttribute(true)]
        public virtual double Area
        {
            get { return Math.PI * Size.Width * Size.Width / 4; }
        }
        public char Symbol { get; set; }
        public Tile Parent { get; set; }

        /// <summary>Returns a reference to the World object inhabited by this object.</summary>
        public World World { get; set; }

        /// <summary>
        /// The action chosen by this Inhabitant during its last Tick.
        /// </summary>
        public World.Actions Action { get; set; }

#if PerceptionHistory
        /// <summary>
        /// The list of past actions by this object.
        /// </summary>
        private List<PerceptionActionReward> m_listHistory;
        public List<PerceptionActionReward> History
        {
            get { return m_listHistory; }
        }
#endif

        /// <summary>
        /// A unit vector that indicates the amount of residual inertia
        /// of this object.  In a "No intertia" configuration, this will
        /// always be zero in magnitude.
        /// </summary>
        public PointF Velocity { get; set; }

        /// <summary>
        /// The label of this object (if needed).
        /// </summary>
        public String Label { get; set; }
        static int m_nNextLabel = 100;

        /// <summary>
        /// Constructor for the SelectableObject.
        /// </summary>
        /// <param name="w">The world this object will inhabit.</param>
        public SelectableObject()
        {
            Label = (m_nNextLabel++).ToString();
            Size = new Size(5, 5);
            Position = new PointF(0, 0);
            Focus = false;
            Selected = false;
            m_clrBack = Color.Bisque;
            m_brushBack = Brushes.Bisque;
            m_clrFore = Color.Black;
            m_penFore = Pens.Black;
            m_clrSelected = Color.LightGreen;
            m_brushSelected = Brushes.LightGreen;
            Symbol = 'z';
            Velocity = new PointF(0, 0);
#if PerceptionHistory
            m_listHistory = new List<PerceptionActionReward>();
#endif
            m_bag = new PropertyBag(this);
            Inbox = new List<Message>();
            Outbox = new List<Message>();
            Action = World.Actions.actStay;
        }

        public virtual void Draw(Graphics bitmapGraphics, Rectangle rectViewport, float fScale)
        {
            Rectangle rectDraw = new Rectangle((int)(Position.X * fScale), (int)(Position.Y * fScale), (int)(this.Size.Width * fScale), (int)(this.Size.Height * fScale));
            rectDraw.Intersect(rectViewport);
            rectDraw.Offset(-rectViewport.X, -rectViewport.Y);
            if (Selected || (Parent != null && Parent.Selected))
                bitmapGraphics.FillEllipse(m_brushSelected, rectDraw);
            else
                bitmapGraphics.FillEllipse(m_brushBack, rectDraw);
            bitmapGraphics.DrawEllipse(m_penFore, rectDraw);
        }

        public virtual void Tick()
        {
            if (World == null)
                throw new ApplicationException("This object has no world.");
        }

        public int Expires { get; internal set; }

        public List<Message> Inbox { get; private set; }
        public List<Message> Outbox { get; private set; }

        /// <summary>
        /// This method provides derived classes the ability to emit messages
        /// intended to be logged by the system to track the results of the 
        /// behavior of the simulation.
        /// </summary>
        /// <param name="strLine"></param>
        protected void LogResults(string strLine)
        {
            World.LogResults(strLine);
        }

        protected static void DrawEllipse(Graphics g, Rectangle rectViewport, PointF position, Size size, float scale, Brush brushBack, Pen penFore)
        {
            Rectangle rectDraw = new Rectangle((int)(position.X * scale), (int)(position.Y * scale), (int)(size.Width * scale), (int)(size.Height * scale));
            rectDraw.Intersect(rectViewport);
            rectDraw.Offset(-rectViewport.X, -rectViewport.Y);
            g.FillEllipse(brushBack, rectDraw);
            g.DrawEllipse(penFore, rectDraw);
        }

        protected static void DrawRectangle(Graphics g, Rectangle rectViewport, PointF position, Size size, float scale, Brush brushBack, Pen penFore)
        {
            Rectangle rectDraw = new Rectangle((int)((position.X - (size.Width / 2)) * scale), (int)((position.Y - (size.Width / 2)) * scale), (int)(size.Width * scale), (int)(size.Height * scale));
            rectDraw.Intersect(rectViewport);
            rectDraw.Offset(-rectViewport.X, -rectViewport.Y);
            g.FillRectangle(brushBack, rectDraw);
            g.DrawRectangle(penFore, rectDraw);
        }

        protected static void DrawTriangle(Graphics g, Rectangle rectViewport, PointF position, Size size, float scale, Brush brushBack, Pen penFore)
        {
            Rectangle rectDraw = new Rectangle((int)(position.X * scale), (int)(position.Y * scale), (int)(size.Width * scale), (int)(size.Height * scale));
            rectDraw.Intersect(rectViewport);
            rectDraw.Offset(-rectViewport.X, -rectViewport.Y);

            Point[] triangle = new Point[]
                {
                    new Point((int)(rectDraw.Left + rectDraw.Width/2), rectDraw.Top),
                    new Point(rectDraw.Left, rectDraw.Bottom),
                    new Point(rectDraw.Right, rectDraw.Bottom)
                };

            g.FillPolygon(brushBack, triangle);
            g.DrawPolygon(penFore, triangle);
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public virtual string Info() { return "A selectable object that can be added to the world."; }
    }
}
