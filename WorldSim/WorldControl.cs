using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WorldSim.Interface;

namespace WorldSim
{
    public partial class WorldControl : UserControl
    {
        private float m_fScale; // zoom factor
        [Category("Appearance"), Description("The zoom level at which the world is viewed.")]
        public float Zoom
        {
            get { return m_fScale; }
            set { m_fScale = value; Invalidate(); }
        }
        private Point m_offset; // world coord's of upper left corner of view
        [Category("Appearance"), Description("The offset from the first upper left tile in the world from the viewport origin.")]
        public Point Origin
        {
            get { return m_offset; }
            set { m_offset = value; Invalidate(); }
        }
        private Bitmap m_localBitmap;

        private World m_world;
        [ReadOnly(true), Description("The world object in use by this control."), Category("Behavior")]
        public World World { get { return m_world; } }

        [ReadOnly(true), Category("Layout"), Description("The absolute width of the world.")]
        public int WorldWidth { get { return m_world.Width; } }

        [ReadOnly(true), Category("Layout"), Description("The absolute height of the world.")]
        public int WorldHeight { get { return m_world.Height; } }
        [Category("Layout"), Description("The number of tiles horizontally in the world.")]
        public int TilesWidth { get { return m_world.Size.Width; } set { m_world.Size = new Size(value, m_world.Size.Width); } }
        [Category("Layout"), Description("The number of tiles vertically in the world.")]
        public int TilesHeight { get { return m_world.Size.Height; } set { m_world.Size = new Size(m_world.Size.Height, value); } }
        [Category("Layout"), Description("The width of a single tile in the world.")]
        public int TileWidth { get { return m_world.TileSize.Width; } set { m_world.TileSize = new Size(value, m_world.TileSize.Width); } }
        [Category("Layout"), Description("The height of a single tile in the world.")]
        public int TileHeight { get { return m_world.TileSize.Height; } set { m_world.TileSize = new Size(m_world.TileSize.Height, value); } }
        public void Add(SelectableObject obj) { m_world.Add(obj); }
        //public void Add(Inhabitant ind) { m_world.Add(ind); Invalidate(); }
        //public void Add(Incident inc) { m_world.Add(inc); Invalidate(); }
        //public void Add(WorldSim.Interface.Marker msg) { m_world.Add(msg, null); Invalidate(); }

        public double RewardScaleP_s
        {
            get { return m_world.RewardScaleP_s; }
            set { m_world.RewardScaleP_s = value; }
        }

        public double RewardScaleP_e
        {
            get { return m_world.RewardScaleP_e; }
            set { m_world.RewardScaleP_e = value; }
        }

        public double RewardScaleInhabitantProximity
        {
            get { return m_world.RewardScaleInhabitantProximity; }
            set { m_world.RewardScaleInhabitantProximity = value; }
        }

        private ECRandom m_random;
        public ECRandom Random
        {
            get { return m_random; }
            //set { m_random = value; }
        }

        private int m_nTrial;
        public int Trial
        {
            get { return m_nTrial; }
            set { m_nTrial = value; }
        }

        public WorldControl()
        {
            InitializeComponent();
            m_random = new ECRandom((int)DateTime.Now.Ticks);
            m_world = new World(new Size(10, 10), new Size(100, 100), "Rectangle", m_random);
            m_world.CollisionDetectedEvent += OnCollisionDetected;
            m_world.ObjectSelectedEvent += OnObjectSelected;
            m_world.LogEvent += OnLogEvent;
            
            m_world.RewardScaleP_s = 1;
            m_world.RewardScaleP_e = 1;
            m_world.RewardScaleP_n = 1;

            // keep control from trying to paint the background
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);

            // initialize stuff
            m_fScale = 1.0f;
            m_localBitmap = null;            
        }

        void OnLogEvent(object sender, World.LogEventArgs e)
        {
            if (LogEvent != null)
                LogEvent(this, new LogEventArgs(e.Message));
        }

        public void Reset(Size tiles, Size tileSize, String strTileShape)
        {
            m_world.CollisionDetectedEvent -= OnCollisionDetected;
            m_world.ObjectSelectedEvent -= OnObjectSelected;
            m_world.LogEvent -= OnLogEvent;
            m_world = new World(tiles, tileSize, strTileShape, m_random);
            m_world.LogEvent += OnLogEvent;
            m_world.CollisionDetectedEvent += OnCollisionDetected;
            m_world.ObjectSelectedEvent += OnObjectSelected;

            // re-add all the little guys



            Invalidate();
        }

        public void Deserialize(String strFileName)
        {
            //Opens file "data.xml" and deserializes the object from it.
            Stream stream = File.Open(strFileName, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            m_world = (World)formatter.Deserialize(stream);
            m_world.CollisionDetectedEvent += OnCollisionDetected;
            m_world.ObjectSelectedEvent += OnObjectSelected;
            m_world.LogEvent += OnLogEvent;
            stream.Close();
        }

        public void Serialize(String strFileName)
        {
            Stream stream = File.Open(strFileName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            m_world.CollisionDetectedEvent -= OnCollisionDetected;
            m_world.ObjectSelectedEvent -= OnObjectSelected;
            m_world.LogEvent -= OnLogEvent;
            formatter.Serialize(stream, m_world);
            m_world.LogEvent += OnLogEvent;
            m_world.CollisionDetectedEvent += OnCollisionDetected;
            m_world.ObjectSelectedEvent += OnObjectSelected;
            stream.Close();
        }

        protected void OnCollisionDetected(object sender, World.CollisionDetectedEventArgs e)
        {
            if (e.m_ind1 == Selected 
                || e.m_ind2 == Selected
                || e.m_ind1.Parent == Selected
                || e.m_ind2.Parent == Selected)
                System.Media.SystemSounds.Beep.Play();
        }

        public class ObjectSelectedEventArgs : EventArgs
        {
            private SelectableObject m_selected;
            public SelectableObject Selected
            {
                get { return m_selected; }
                set { m_selected = value; }
            }
            public ObjectSelectedEventArgs(SelectableObject obj)
            {
                Selected = obj;
            }
        }
        public delegate void ObjectSelectedEventDelegate(object sender, ObjectSelectedEventArgs e);
        public event ObjectSelectedEventDelegate ObjectSelectedEvent;

        protected void OnObjectSelected(object sender, World.ObjectSelectedEventArgs e)
        {
            if (ObjectSelectedEvent != null)
                ObjectSelectedEvent(this, new ObjectSelectedEventArgs(e.Selected));
        }

        public void Tick()
        {
            // tell all the objects to increment their time position one step
            m_world.Tick();
            Invalidate();
        }

        //[ReadOnly(true), Description("The maximum reward value attained for agents in this world."), Category("Statistics")]
        //public double MaxReward { get { return m_world.MaxReward; } }
        //[ReadOnly(true), Description("The average reward value attained by agents in this world."), Category("Statistics")]
        //public double AverageReward { get { return m_world.AverageReward; } }
        //[ReadOnly(true), Description("The average NAP reward value attained by agents in this world."), Category("Statistics")]
        //public double AverageRewardP_s { get { return m_world.AverageRewardP_s; } }
        //[ReadOnly(true), Description("The average IP reward value attained by agents in this world."), Category("Statistics")]
        //public double AverageRewardP_e { get { return m_world.AverageRewardP_e; } }
        //[ReadOnly(true), Description("The average NMP reward value attained by agents in this world."), Category("Statistics")]
        //public double AverageRewardP_n { get { return m_world.AverageRewardP_n; } }
        //[ReadOnly(true), Description("The energy value of agents currently (sum of velocity vectors)."), Category("Statistics")]
        //public double Energy { get { return m_world.Energy; } }
        //[ReadOnly(true), Description("The amount of surface area occupied by agents in this world."), Category("Statistics")]
        //public double Spread { get { return m_world.Spread; } }
        //[ReadOnly(true), Description("The amount of surface area occupied by agents in this world."), Category("Statistics")]
        //public double Fill { get { return m_world.Fill; } }
        //[ReadOnly(true), Description("The sum of resources stored by tiles in this world."), Category("Statistics")]
        //public double TileResources { get { return m_world.TileResources; } }
        //[ReadOnly(true), Description("The sum of resources stored by incidents in this world."), Category("Statistics")]
        //public double IncidentResources { get { return m_world.IncidentResources; } }
        //[ReadOnly(true), Description("The number of incidents covered by agents."), Category("Statistics")]
        //public double IncidentCoverage { get { return m_world.IncidentCoverage; } }
        //[ReadOnly(true), Description("The number of incident-inhabitant connections."), Category("Statistics")]
        //public double IncidentInhabitantEdges { get { return m_world.IncidentInhabitantEdges; } }
        //[ReadOnly(true), Description("The number of inhabitant-inhabitant connections."), Category("Statistics")]
        //public double InhabitantInhabitantEdges { get { return m_world.InhabitantInhabitantEdges; } }
        //[ReadOnly(true), Description("The selected tile or object."), Category("Appearance")]
        public SelectableObject Selected
        {
            get { return m_world.Selected; }
        }

        private void WorldControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // create back-buffer bitmap
            if (m_localBitmap == null || m_localBitmap.Width != ClientRectangle.Width || m_localBitmap.Height != ClientRectangle.Height)
                m_localBitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics bitmapGraphics = Graphics.FromImage(m_localBitmap);

            bitmapGraphics.Clear(this.BackColor);

            Rectangle rectViewport = new Rectangle();
            rectViewport.X = m_offset.X;
            rectViewport.Y = m_offset.Y;
            rectViewport.Width = Convert.ToInt32(ClientRectangle.Width / m_fScale);
            rectViewport.Height = Convert.ToInt32(ClientRectangle.Height / m_fScale);

            // draw to bitmapGraphics
            m_world.Draw(bitmapGraphics, rectViewport, m_fScale);

            // push back-buffer bitmap to foreground
            g.DrawImage(m_localBitmap, 0, 0);
            bitmapGraphics.Dispose();
        }

        private void WorldControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_world.SelectAt(new Point(e.X, e.Y), m_offset, m_fScale);
            //else if (e.Button == MouseButtons.Right)
            //    m_world.SelectIndividualAt(new Point(e.X, e.Y), m_offset, m_fScale);
            Invalidate();
        }

        public class LogEventArgs : EventArgs
        {
            private string m_strMessage;
            public string Message { get { return m_strMessage; } set { m_strMessage = value; } }
            public LogEventArgs(string m) { m_strMessage = m; }
        }
        public delegate void LogDelegate(object sender, LogEventArgs e);
        public event LogDelegate LogEvent;

    }
}
