#define WORLD_NO_INERTIA

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace WorldSim.Interface
{
    [Serializable]
    public class World
    {
        private Guid m_guid;
        public Guid Guid { get { return m_guid; } internal set { m_guid = value; } }

        /// <summary>
        /// These are all of the actions that an inhabitant of the world can choose from.
        /// </summary>
        public enum Actions // act
        { // these are ordered like this so that when values are the same, the proper one is displayed in the propertygrid
#if WORLD_NO_INERTIA
            Min = 0,
            actGoE = Min,
            actGoESE,
            actGoSE,
            actGoSSE,
            actGoS,
            actGoSSW,
            actGoSW,
            actGoWSW,
            actGoW,
            actGoWNW,
            actGoNW,
            actGoNNW,
            actGoN,
            actGoNNE,
            actGoNE,
            actGoENE,
            Max,
            actStay = Max,
            actUseVelocity
#else
            Min,
            actContinue = Min,
            actTurnRight,
            actTurnLeft,
            actSlowDown,
            Max,
            actSpeedUp = Max
#endif
        }

        /// <summary>
        /// These are all of the pieces of data that an agent has access to in order to decide
        /// what action to take.
        /// </summary>
        public enum Sensors // sen
        {
            senDirection, // our agent's direction (0-7)
            senSpeed, // our agent's speed (0-9)
            senPop_N, // number of individuals in the cell above
            senPop_NE,
            senPop_E,
            senPop_SE,
            senPop_S,
            senPop_SW,
            senPop_W,
            senPop_NW,
            senPop_Here, // population in our own cell
            Min = senDirection,
            Max = senPop_Here,
            Count = Max - Min + 1
        }

        public enum TileBackgroundIndicates // tbi
        {
            tbiNone, // just indicate whether tiles in the world are selected or not
            tbiStale, // indicate how stale tiles are (how long since covered)
            tbiPoll, // indicate whether the tile as been covered since it was last polled
            tbiResources // indicate how much the tile has accumulated in resources
        }
        private TileBackgroundIndicates m_tbi;
        public TileBackgroundIndicates TileBackgroundIndication { get { return m_tbi; } set { m_tbi = value; } }

        /// <summary>
        /// The velocity vector is represented as a vector in x,y coordinates.  This
        /// function adjusts it a certain number of radians while retaining its magnitude.
        /// </summary>
        /// <param name="ptVelocity">A point representing the velocity vector to adjust.</param>
        /// <param name="dUnits">The number of radians to add to the existing vector.</param>
        /// <returns>The new velocity vector.</returns>
        public static PointF AdjustVelocityAngle(PointF ptVelocity, double dUnits)
        {
            // calculate the new angle
            double dThetaNew = Math.Atan2(ptVelocity.Y, ptVelocity.X) + dUnits;
            // calculate the magnitude of the old velocity vector
            double dRadius = Math.Sqrt(ptVelocity.X * ptVelocity.X + ptVelocity.Y * ptVelocity.Y);
            // using the new angle and preserving the old magnitude, compute new point
            return new PointF((float)(dRadius * Math.Cos(dThetaNew)), (float)(dRadius * Math.Sin(dThetaNew)));
        }

        /// <summary>
        /// The velocity vector is represented as a vector in x,y coordinates.  This
        /// function adjusts its magnitude (length) a certain number of units while retaining 
        /// its direction.
        /// </summary>
        /// <param name="velocity">A point representing the velocity vector to adjust.</param>
        /// <param name="fUnits">The number of units to add to the existing vector.</param>
        /// <returns>The new velocity vector.</returns>
        public static PointF AdjustVelocityMagnitude(PointF velocity, double fUnits)
        {
            // update the velocity vector of each to the new direction, and
            double theta = Math.Atan2(velocity.Y, velocity.X);
            double radiusNew = Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y) + fUnits;
            return new PointF((float)(radiusNew * Math.Cos(theta)), (float)(radiusNew * Math.Sin(theta)));
        }

        //private double m_dMaxReward;
        //public double MaxReward { get { return m_dMaxReward; } }
        //private double m_dAverageReward;
        //public double AverageReward { get { return m_dAverageReward; } }
        //private double m_dAverageRewardP_s;
        //public double AverageRewardP_s { get { return m_dAverageRewardP_s; } }
        //private double m_dAverageRewardP_e;
        //public double AverageRewardP_e { get { return m_dAverageRewardP_e; } }
        //private double m_dAverageRewardP_n;
        //public double AverageRewardP_n { get { return m_dAverageRewardP_n; } }
        //private double m_dEnergy;
        //public double Energy { get { return m_dEnergy; } }
        //private double m_dFill;
        //public double Fill { get { return m_dFill; } }
        //private double m_dTileResources;
        //public double TileResources { get { return m_dTileResources; } }
        //private double m_dIncidentResources;
        //public double IncidentResources { get { return m_dIncidentResources; } }
        //private double m_dSpread;
        //public double Spread { get { return m_dSpread; } }
        //private double m_dIncidentsCovered;
        //private int m_nIncidentPasses; // scales m_dIncidentsCovered
        //public double IncidentCoverage 
        //{ 
        //    get 
        //    {
        //        double dCoverage = 0.0d;
        //        if (Monitor.TryEnter(this, 3000))
        //        {
        //            if (m_nIncidentPasses != 0)
        //                dCoverage = m_dIncidentsCovered / m_nIncidentPasses;
        //            Monitor.Exit(this);
        //        }
        //        return dCoverage; 
        //    }
        //}
        //private int m_nIncidentInhabitantEdges;
        //public int IncidentInhabitantEdges
        //{
        //    get { return m_nIncidentInhabitantEdges; }
        //    set { m_nIncidentInhabitantEdges = value; }
        //}
        //private int m_nInhabitantInhabitantEdges;
        //public int InhabitantInhabitantEdges
        //{
        //    get { return m_nInhabitantInhabitantEdges; }
        //    set { m_nInhabitantInhabitantEdges = value; }
        //}


        public int Inhabitants { get { int nCount = 0; foreach (Tile t in Tiles.AllTiles) foreach (Inhabitant i in t.Objects(typeof(Inhabitant))) nCount++; return nCount; } }
        public int Incidents { get { int nCount = 0; foreach (Tile t in Tiles.AllTiles) foreach (Incident i in t.Objects(typeof(Incident))) nCount++; return nCount; } }

        /// <summary>
        /// Returns the inhabitant nearest to the inhabitant i that is within its sensor range.
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public Inhabitant NearestInhabitant(Inhabitant i, List<Inhabitant> iNearby)
        {
            Inhabitant iBest = null;
            double distBest = double.MaxValue;

            // check against every other object in the neighborhood...
            foreach (Inhabitant iN in iNearby)
            {
                if (iN != i)
                {
                    double dist = Distance(iN.Position, i.Position);
                    if (dist < i.Parent.Size.Width && dist < distBest)
                    {
                        iBest = iN;
                        distBest = dist;
                    }
                }
            }

            return iBest;
        }

        /// <summary>
        /// Returns the tile nearest to the individual that it is not currently within.
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public Incident NearestIncident(Inhabitant i)
        {
            Incident iBest = null;
            double distBest = double.MaxValue;
            foreach (Incident iTile in i.Parent.Objects(typeof(Incident)))
            {
                double dist = Distance(iTile.Position, i.Position);
                if (dist < distBest)
                {
                    iBest = iTile;
                    distBest = dist;
                }
            }
            if (iBest != null)
                return iBest;

            // check against every other object in the neighborhood...
            foreach (Tile tN in i.Parent.Neighbors)
            {
                List<Incident> lstIndCheck = new List<Incident>();
                foreach (Incident iN in tN.Objects(typeof(Incident)))
                    lstIndCheck.Add(iN);
                foreach (Incident iN in lstIndCheck)
                {
                    double dist = Distance(iN.Position, i.Position);
                    if (dist < i.Parent.Size.Width && dist < distBest)
                    {
                        iBest = iN;
                        distBest = dist;
                    }
                }
            }
            return iBest;
        }

        private SelectableObject m_selected;
        public SelectableObject Selected
        {
            get { return m_selected; }
            set
            {
                m_selected = value; 
                if (ObjectSelectedEvent != null)
                    ObjectSelectedEvent(this, new ObjectSelectedEventArgs(m_selected));
            }
        }

        private Tile m_tiles; // used to organize individuals in world
        public Tile Tiles
        {
            get 
            { 
                return m_tiles; 
            }
        }        
        private String m_strTitle; // label (if needed)
        public String Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }
        private Size m_size;
        public Size Size
        {
            get { return m_size; }
            set 
            { 
                m_size = value; 
                Type tTile = m_tiles.GetType();
                m_tiles = (Tile)tTile.InvokeMember("Build", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { this, Size, TileSize });
                //m_tiles = RectangleTile.Build(this, m_size, m_tilesize); 
            }
        }
        private Size m_tilesize;
        public Size TileSize
        {
            get { return m_tilesize; }
            set
            {
                m_tilesize = value; Type tTile = m_tiles.GetType();
                m_tiles = (Tile)tTile.InvokeMember("Build", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { this, Size, TileSize });
                //m_tiles = RectangleTile.Build(this, m_size, m_tilesize);
            }
        }
        /// <summary>
        /// For RectangleTile, the height is the same as HexagonTile.
        /// </summary>
        public int Height
        {
            get { return m_size.Height * m_tilesize.Height; }
        }
        /// <summary>
        /// For RectangleTile, the height is the tile height times number of tiles.  For HexagonTile,
        /// however, there is some horizontal overlap.
        /// </summary>
        public int Width
        {
            get { return m_size.Width * m_tilesize.Width; }
        }
        public enum ermRewardMethods
        {
            ermNumberOfNearbyInhabitants = 1, // reward varies as distance to nearest agent
            ermP_e = 2, // reward varies as distance to incidents in sensor range
            ermInhabitantCrowding = 4 // reward (penalty, actually) varies as agents are more crowded
        }
        private int m_nRewardMethods;
        public int RewardMethods
        {
            get { return m_nRewardMethods; }
        }
        public void Draw(Graphics bitmapGraphics, Rectangle rectViewport, float fScale)
        {
            //if (Monitor.TryEnter(this, 5000))
            {
                try
                {
                    // Place code protected by the Monitor here.
                    List<SelectableObject> objects = new List<SelectableObject>();
                    //List<Incident> incidents = new List<Incident>();
                    //List<Inhabitant> inhabitants = new List<Inhabitant>();
                    //List<Marker> messages = new List<Marker>();
                    //List<SelectableObject> agents = new List<SelectableObject>();
                    foreach (Tile t in Tiles.AllTiles)
                    {
                        if (t.IsVisible(rectViewport, fScale))
                        {
                            t.Draw(bitmapGraphics, rectViewport, fScale, m_tbi);
                            foreach (SelectableObject o in t.Objects(typeof(SelectableObject)))
                                objects.Add(o);
                            //foreach (Marker message in t.Objects(typeof(Marker)))
                            //    messages.Add(message);
                            //foreach (Incident incident in t.Objects(typeof(Incident)))
                            //    incidents.Add(incident);
                            //foreach (Inhabitant inhabitant in t.Objects(typeof(Inhabitant)))
                            //    inhabitants.Add(inhabitant);
                            //foreach (SelectableObject agent in t.Objects(typeof(SelectableObject)))
                            //    agents.Add(agent);
                        }
                    }
                    foreach (SelectableObject dr in objects)
                        dr.Draw(bitmapGraphics, rectViewport, fScale);
                    //foreach (Marker m in messages)
                    //    m.Draw(bitmapGraphics, rectViewport, fScale);
                    //foreach (Incident dr in incidents)
                    //    dr.Draw(bitmapGraphics, rectViewport, fScale);
                    //foreach (Inhabitant dr in inhabitants)
                    //    dr.Draw(bitmapGraphics, rectViewport, fScale);
                    //foreach (SelectableObject dr in agents)
                    //    dr.Draw(bitmapGraphics, rectViewport, fScale);
                }
                catch (InvalidOperationException)
                {
                    // just swallow it and paint them next time
                    Debug.WriteLine("InvalidOperationException while painting.");
                }
                catch (Exception ex)
                {
                    // just swallow it and paint them next time
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    //Monitor.Exit(this);
                }
            }
            //else
            //{
                // Code to execute if the attempt times out.
                // (just wait and try drawing again next time)
                //System.Diagnostics.Debug.WriteLine("Drawing timed out.");
            //}
        }

        private ECRandom m_random;
        public ECRandom Random
        {
            get { return m_random; }
            //set { m_random = value; }
        }
        
        /// <summary>
        /// Constructor for the World object.
        /// </summary>
        /// <param name="sizeTiles">The number of tiles horizontally and vertically that make 
        /// up this world.</param>
        /// <param name="sizeTile">The size of a tile in units horizontally and vertically.</param>
        public World(Size sizeTiles, Size sizeTile, String strTileShape, ECRandom r)
        {
            Initialize(sizeTiles, sizeTile, strTileShape, "New World", r);
        }

        /// <summary>
        /// Constructor for the World object.
        /// </summary>
        /// <param name="sizeTiles">The number of tiles horizontally and vertically that make 
        /// up this world.</param>
        /// <param name="sizeTile">The size of a tile in units horizontally and vertically.</param>
        /// <param name="strTitle">The title of this world.</param>
        public World(Size sizeTiles, Size sizeTile, String strTileShape, String strTitle, ECRandom r)
        {
            Initialize(sizeTiles, sizeTile, strTileShape, strTitle, r);
        }

        private void Initialize(Size sizeTiles, Size sizeTile, String strTileShape, String strTitle, ECRandom r)
        {
            m_strTitle = strTitle;
            m_size = sizeTiles;
            m_tilesize = sizeTile;
            if (strTileShape.StartsWith("Hex"))
                m_tiles = HexagonTile.Build(this, m_size, m_tilesize);
            else
                m_tiles = RectangleTile.Build(this, m_size, m_tilesize);
            m_random = r;
            m_nRewardMethods = (int)ermRewardMethods.ermP_e 
                | (int)ermRewardMethods.ermNumberOfNearbyInhabitants
                | (int)ermRewardMethods.ermInhabitantCrowding;
            RewardScaleP_s = 1.0d;
            RewardScaleP_e = 1.0d;
            RewardScaleP_n = 1.0d;
            MinAgentNeighbors = 1;
            MaxAgentNeighbors = 4;
            Guid = Guid.NewGuid();
        }

#if PerceptionHistory
        /// <summary>
        /// From the specified inhabitant's point of view, return what that
        /// object can see.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public String Perceptions(Inhabitant i)
        {
            // divide the view from that point into slivers.  Each sliver represents
            // 360 / #slivers degrees.  If there is a goal or other agent in range
            // of the object in that direction, then the position of the perception
            // string will contain a symbol, otherwise, it will contain a space.
            // The string's middle symbol (or the one to the left of center if the
            // number of slivers is even) is the direction the object is facing.

            int nSlivers = 16;

            // get a list of the tiles that could contain objects we need to consider
            List<Tile> neighborhood = new List<Tile>();
            if (i.Parent != null)
            {
                neighborhood.Add(i.Parent);
                neighborhood.AddRange(i.Parent.Neighbors);
            }

            // get a list of the goals and objects in range of this object
            SortedList<int, SelectableObject> nearby = new SortedList<int, SelectableObject>();
            foreach (Tile t in neighborhood)
            {
                foreach (Inhabitant it in t.Objects(typeof(Inhabitant)))
                {
                    if (i == it) continue;
                    int nDirection = RelativeDirection(i, it.Position, nSlivers);
                    if (nearby.ContainsKey(nDirection))
                    {
                        if (Distance(i.Position, it.Position) < Distance(i.Position, nearby[nDirection].Position))
                            nearby[nDirection] = it;
                    }
                    else
                        nearby.Add(nDirection, it);
                }
                foreach (Incident it in t.Objects(typeof(Incident)))
                {
                    int nDirection = RelativeDirection(i, it.Position, nSlivers);
                    if (nearby.ContainsKey(nDirection))
                    {
                        if (Distance(i.Position, it.Position) < Distance(i.Position, nearby[nDirection].Position))
                            nearby[nDirection] = it;
                    }
                    else
                        nearby.Add(nDirection, it);
                }
            }

            StringBuilder sb = new StringBuilder(nSlivers);
            sb.Append('.', nSlivers);

            for (int nSliver = nSlivers / 2; nSliver < 3 * nSlivers / 2; nSliver++) 
            {
                int nSliverCheck = nSliver % nSlivers;
                if (nearby.ContainsKey(nSliverCheck))
                {
                    if (Distance(i.Position, nearby[nSliverCheck].Position) < i.SensorRange / 3)
                        sb[nSliverCheck] = Char.ToUpper(nearby[nSliverCheck].Symbol);
                    else
                        sb[nSliverCheck] = Char.ToLower(nearby[nSliverCheck].Symbol);
                }
            }

            return sb.ToString();
        }
#endif

        static int RelativeDirection(Inhabitant i, PointF p, int nSlivers)
        {
            double dRadiansP = Math.Atan2(i.Position.Y - p.Y, i.Position.X - p.X);
            double dRadians = Math.Atan2(-i.Velocity.Y, -i.Velocity.X);

            // direction to p, 0-7 from this.Position to p that number'th bit is on, the rest off
            int nDirectionP = (int)Math.Floor(((dRadiansP + Math.PI) * 4 / Math.PI) % nSlivers);

            // direction, to keep it simple, use navigation values 0 thru 7
            int nDirection = (int)Math.Floor(((dRadians + Math.PI) * 4 / Math.PI) % nSlivers);

            return Math.Abs(nSlivers + nDirectionP - nDirection) % nSlivers;
        }

        public class Tallies
        {
            public double dRewardP_n;
            public double dRewardP_e;
            public double dRewardP_s;
            public double dTotalReward;
            public double dMaxReward;
            public double dEnergy;
            public double dFill;
            //public double dTileResources;
            //public double dIncidentResources;
            public double dSpread;
            public int nCount;
            public int nIncidentsCovered;
            public int nIncidents;
            public int nIncidentInhabitantEdges;
            public int nInhabitantInhabitantEdges;
            public Tallies()
            {

            }
        }

        public IEnumerable<Incident> _Incidents
        {
            get
            {
                List<Incident> list = new List<Incident>();
                foreach (Tile tEnv in Tiles.AllTiles)
                    foreach (Incident i in tEnv.Objects(typeof(Incident)))
                        list.Add(i);
                foreach (Incident i in list)
                    yield return i;
            }
        }

        public IEnumerable<Inhabitant> _Inhabitants
        {
            get
            {
                List<Inhabitant> list = new List<Inhabitant>();
                foreach (Tile tEnv in Tiles.AllTiles)
                    foreach (Inhabitant i in tEnv.Objects(typeof(Inhabitant)))
                        list.Add(i);
                foreach (Inhabitant i in list)
                    yield return i;
            }
        }

        public IEnumerable<Marker> _Markers
        {
            get
            {
                List<Marker> list = new List<Marker>();
                foreach (Tile tEnv in Tiles.AllTiles)
                    foreach (Marker m in tEnv.Objects(typeof(Marker)))
                        list.Add(m);
                foreach(Marker m in list)
                    yield return m;
            }
        }

        public IEnumerable<SelectableObject> AllObjects
        {
            get
            {
                List<SelectableObject> list = new List<SelectableObject>();
                foreach (Tile t in Tiles.AllTiles)
                    foreach (SelectableObject o in t.Objects(typeof(SelectableObject)))
                        list.Add(o);
                foreach(SelectableObject o in list)
                    yield return o;
            }
        }

        public IEnumerable<SelectableObject> Objects(Type T)
        {
            List<SelectableObject> list = new List<SelectableObject>();
            foreach (Tile t in Tiles.AllTiles)
                foreach (SelectableObject o in t.Objects(T))
                    list.Add(o);
            foreach (SelectableObject o in list)
                yield return o;
        }

        /// <summary>
        /// This is the function that gets called every so often to update the state
        /// of the environment.  
        /// </summary>
        public void Tick()
        {
            if (Monitor.TryEnter(this, 3000))
            {
                try
                {
                    // get a list of all of the objects
                    List<SelectableObject> mAll = new List<SelectableObject>();
                    foreach (SelectableObject o in AllObjects)
                        mAll.Add(o);

                    // Remove expired objects
                    List<SelectableObject> mDelete = new List<SelectableObject>();
                    foreach (SelectableObject o in AllObjects)
                        if (o.Expires < 0)
                            mDelete.Add(o);
                    foreach (SelectableObject m in mDelete)
                        m.Parent.RemoveObject(m);

                    foreach (Tile t in Tiles.AllTiles)
                        t.Tick();

                    // have each object evaluate its situation and decide on an action
                    foreach (SelectableObject o in mAll)
                        o.Tick();

                    // process the mail
                    foreach (SelectableObject o in mAll)
                        o.Inbox.Clear();
                    foreach (SelectableObject o in mAll)
                        DeliverMail(o);

                    // carry out the action selected by each object
                    foreach (SelectableObject o in mAll)
                        PerformRequestedAction(o);
                    foreach (SelectableObject o in mAll)
                        MovePoint(o);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
            else
            {
                // Code to execute if the attempt times out.
                throw new ApplicationException("Why did this take so long?");
            }
        }

        /// <summary>
        /// The name of this method could be somewhat misleading.  The purpose of 
        /// this function is to broadcast the messages in the object's outbox, 
        /// which places them in the inbox of any other object within range.  It
        /// is up to the receiving object to decide what it will do with the 
        /// message.
        /// </summary>
        /// <param name="o"></param>
        private bool DeliverMail(SelectableObject o)
        {
            bool bMessagesSent = false;
            if (o.Outbox.Count > 0)
            {
                // get the routed messages from the outbox and handle them first
                List<RoutedMessage> listRouted = new List<RoutedMessage>();
                foreach (Message m in o.Outbox)
                {
                    Type t1 = m.GetType();
                    Type t2 = typeof(RoutedMessage);
                    if (t2.IsAssignableFrom(t1))
                        listRouted.Add(m as RoutedMessage);
                }
                foreach (RoutedMessage m in listRouted)
                {
                    o.Outbox.Remove(m);
                    if (m.Recipient == o)
                    {
                        Trace.WriteLine("*** Objects can't send routed messages to themselves! ***");
                    }
                    else
                    {
                        m.Recipient.Inbox.Add(m);
                        bMessagesSent = true;
                        if (MessageSentEvent != null)
                            MessageSentEvent(this, new MessageSentEventArgs(m, o, m.Recipient));
                    }
                }

                // get a list of objects within range, and place the messages in o's outbox 
                // in each nearby object's inbox, then clear o's outbox
                IEnumerable<SelectableObject> listNearby = o.Parent.NearbyObjects(typeof(SelectableObject));
                foreach (SelectableObject n in listNearby)
                {
                    if (n == o || (null == o as Inhabitant))
                        continue;
                    if (Distance(o.Center, n.Center) <= (o as Inhabitant).SensorRange)
                    {
                        n.Inbox.AddRange(o.Outbox);
                        bMessagesSent = true;
                        if (MessageSentEvent != null)
                            foreach (Message m in o.Outbox)
                                MessageSentEvent(this, new MessageSentEventArgs(m, o, n));
                    }
                }
                o.Outbox.Clear();
            }
            return bMessagesSent;
        }

        private void DistributeReward(Inhabitant i, List<Inhabitant> iNearby, object o)
        {
            Tallies t = (Tallies)o;

            // Give the dot his cookie.
            i.Reward = Reward(i, iNearby, t);

            // record how well this agent is doing
            i.RewardTotal += i.Reward;

#if WORLD_NO_INERTIA
            t.dEnergy += i.Action == Actions.actStay ? 0.0 : 1.0;
#else
            t.dEnergy += Math.Sqrt(i.Velocity.X * i.Velocity.X + i.Velocity.Y * i.Velocity.Y);
#endif
            t.dFill += i.Area;
            t.dSpread += iNearby.Count; // don't count the fact that we see ourself nearby
            t.nCount++;
        }

        private static void PerformRequestedAction(SelectableObject i)
        {
            // based on i.Action, apply the agent's requested action, if possible
            // apply actions to individual
            switch (i.Action)
            {
#if WORLD_NO_INERTIA
                case Actions.actStay: i.Velocity = new PointF(0, 0); break;
                case Actions.actUseVelocity:
                    // normalize vector
                    float length = (float)Math.Sqrt(i.Velocity.X * i.Velocity.X + i.Velocity.Y * i.Velocity.Y);
                    if (length != 0)
                        i.Velocity = new PointF(i.Velocity.X / length, i.Velocity.Y / length);
                    break;
                default: i.Velocity = new PointF((float)(3.0f * Math.Sin(((int)i.Action) * 22.5f * Math.PI / 180)), 
                    (float)(3.0f * Math.Cos(((int)i.Action) * 22.5f * Math.PI / 180))); 
                    break;
#else
                case Actions.actSpeedUp: // increase magnitude of velocity vector a little
                    if (i.Velocity.X * i.Velocity.X + i.Velocity.Y * i.Velocity.Y < (i.Size.Width / 2) * (i.Size.Width / 2))
                        i.Velocity = AdjustVelocityMagnitude(i.Velocity, 4);
                    break;
                case Actions.actSlowDown: // decrease magnitude of velocity vector a little
                    if (i.Velocity.X * i.Velocity.X + i.Velocity.Y * i.Velocity.Y > 4 * 4)
                        i.Velocity = AdjustVelocityMagnitude(i.Velocity, -4);
                    break;
                case Actions.actTurnLeft: // rotate velocity vector up a little
                    i.Velocity = AdjustVelocityAngle(i.Velocity, Math.PI / 16);
                    break;
                case Actions.actTurnRight: // rotate velocity vector down a little
                    i.Velocity = AdjustVelocityAngle(i.Velocity, -Math.PI / 16);
                    break;
                case Actions.actContinue: // just keep on the current course and speed
                    break;
#endif
            }
        }

        private void MovePoint(SelectableObject ind)
        {
            Debug.Assert(!Double.IsNaN(ind.Velocity.X) && !Double.IsNaN(ind.Velocity.Y));
            ind.Position = new PointF((Width + ind.Position.X + (int)ind.Velocity.X) % Width,
                (Height + ind.Position.Y + (int)ind.Velocity.Y) % Height);
            SetParent(ind);
#if WORLD_NO_INERTIA
            ind.Velocity = new PointF(0, 0);
#endif        
        }

        private void MovePoint(Incident inc)
        {
            inc.Position = new PointF((Width + inc.Position.X + (int)inc.Velocity.X) % Width,
                (Height + inc.Position.Y + (int)inc.Velocity.Y) % Height);
            SetParent(inc);
#if WORLD_NO_INERTIA
            inc.Velocity = new PointF(0, 0);
#endif
        }

        public double Reward(Inhabitant i, List<Inhabitant> iNearby, Tallies t)
        {
            // get list of incidents in the vicinity
            List<Incident> lst = new List<Incident>();
            foreach (Tile tt in i.Parent.Neighbors)
                foreach (Incident inc in tt.Objects(typeof(Incident)))
                    lst.Add(inc);
            foreach (Incident inc in i.Parent.Objects(typeof(Incident)))
                lst.Add(inc);

            double dRewardP_n = ScoreForP_n(i, iNearby);
            double dRewardP_e = ScoreForP_e(i, lst);
            double dRewardP_s = ScoreForP_s(i, iNearby);

            //double dReward = (dRewardP_n + dRewardP_e + dRewardP_s) / (RewardScaleP_n + RewardScaleP_e + RewardScaleP_s);
            double dReward = (dRewardP_n * dRewardP_e * dRewardP_s) / (RewardScaleP_n * RewardScaleP_e * RewardScaleP_s);            
            if (t != null)
            {
                t.dRewardP_n += dRewardP_n;
                t.dRewardP_e += dRewardP_e;
                t.dRewardP_s += dRewardP_s;
                t.dTotalReward += dReward;
                t.dMaxReward = Math.Max(t.dMaxReward, i.RewardTotal);
            }

            return dReward;
        }

        /// <summary>
        /// This function computes the payoff for the specified agent when considering
        /// the relative location of the events in the system.  The max for this is
        /// RewardScaleP_e (when the two agents occupy the same point), 
        /// and the min is zero (when no other agents are visible).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="pts"></param>
        /// <returns></returns>
        public double ScoreForP_e(Inhabitant a, List<Incident> pts)
        {
            double dReward = 0, dRewardScale = 0;
            foreach (Incident pE in pts)
            {
                double dDistance = Distance(a.Position, pE.Position);
                if (dDistance <= a.SensorRange / 2)
                {
                    dReward++;
                    dRewardScale++;
                }
                else if (dDistance <= a.SensorRange)
                {
                    dReward += 1.0d / (1.0d + 100.0d * ( Math.Pow(dDistance / a.SensorRange - 0.5, 2) ) );
                    dRewardScale++;
                }
            }
            return dRewardScale == 0 ? 0 : RewardScaleP_e * dReward / dRewardScale;
        }

        /// <summary>
        /// This function computes the payoff for the specified agent when considering
        /// the relative location of the other agents in the system.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double ScoreForP_s(Inhabitant a, List<Inhabitant> b)
        {
            double dScore = 0;
            double dDistanceClosestAgent = double.MaxValue;
            foreach (Inhabitant i in b)
            {
                if (i == a)
                    continue;
                double dDistance = Distance(a.Position, i.Position);
                if (dDistance <= a.SensorRange)
                    if (dDistance < dDistanceClosestAgent)
                        dDistanceClosestAgent = dDistance;
            }

            if (dDistanceClosestAgent <= a.SensorRange)
            {
                // Part 1 is a nice smooth curve peaking at Sin(Pi/4)
                dScore += 1.0d / (1.0d + Math.Pow((dDistanceClosestAgent - a.SensorRange * Math.Sin(Math.PI / 4)) / a.SensorRange, 2));
                // Part 2 is flat from 0 to Sin(Pi/4) and then goes down with the square of the distance above that
                dScore += 1.0d / (1.0d + 100 * Math.Pow((dDistanceClosestAgent - a.SensorRange * Math.Sin(Math.PI / 4)) / a.SensorRange, 2));
            }

            return RewardScaleP_s * dScore / 2;
        }

        /// <summary>
        /// This function returns a score based on how many close neighbors an agent has.
        /// If the agent has between a certain upper and lower bound, then a flat reward
        /// is given.  If it is less than or greater than the good range, then the reward
        /// decays quickly.  Estimated good range is between loglogn and logn.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double ScoreForP_n(Inhabitant a, List<Inhabitant> b)
        {
            int nNeighbors = -1; // self counts as one
            foreach (Inhabitant i in b)
                if (Distance(a.Position, i.Position) <= a.SensorRange)
                    nNeighbors++;

            double dScore = 0.0d;
            if (nNeighbors < m_nMinAgentNeighbors ) 
                dScore = RewardScaleP_n / (1.0d + Math.Pow(MinAgentNeighbors - nNeighbors, 2));
            else if (nNeighbors > m_nMaxAgentNeighbors)
                dScore = RewardScaleP_n / (1.0d + Math.Pow(nNeighbors - MaxAgentNeighbors, 2));
            else
                dScore = RewardScaleP_n;
            return dScore;
        }

        private double m_dRewardScaleP_s;
        public double RewardScaleP_s
        {
            get { return m_dRewardScaleP_s; }
            set { m_dRewardScaleP_s = value; }
        }

        private double m_dRewardScaleP_e;
        public double RewardScaleP_e
        {
            get { return m_dRewardScaleP_e; }
            set { m_dRewardScaleP_e = value; }
        }

        private double m_dRewardScaleInhabitantProximity;
        public double RewardScaleInhabitantProximity
        {
            get { return m_dRewardScaleInhabitantProximity; }
            set { m_dRewardScaleInhabitantProximity = value; }
        }

        private int m_nMinAgentNeighbors;
        public int MinAgentNeighbors
        {
            get { return m_nMinAgentNeighbors; }
            set { m_nMinAgentNeighbors = value; }
        }

        private int m_nMaxAgentNeighbors;
        public int MaxAgentNeighbors
        {
            get { return m_nMaxAgentNeighbors; }
            set { m_nMaxAgentNeighbors = value; }
        }

        private double m_dP_n;
        public double RewardScaleP_n
        {
            get { return m_dP_n; }
            set { m_dP_n = value; }
        }

        /// <summary>
        /// Detects collisions between a inhabitant and a list of inhabitants in its vicinity.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="iNearby">The i nearby.</param>
        /// <param name="o">The o.</param>
        private void CollisionDetection(Inhabitant i, List<Inhabitant> iNearby)
        {
            // if the id of the first is smaller than the latter...
            foreach (Inhabitant iN in iNearby)
                if (i.ID < iN.ID)
                    CollisionDetection(i, iN);
        }

        /// <summary>
        /// Compare the two specified inhabitants to see if they bumped into one another.
        /// Handle the collision physics and update the position and velocity vectors.
        /// </summary>
        /// <param name="i">An inhabitant.</param>
        /// <param name="iN">Another inhabitant in the vicinity of i.</param>
        private void CollisionDetection(Inhabitant i, Inhabitant iN)
        {
            PointF ptN = iN.Position;
            if (i.Parent.Position.X + i.Parent.Size.Width < iN.Parent.Position.X) // wraps around on left side of i.Parent?
                ptN.X = i.Parent.Position.X - (iN.Parent.Position.X + iN.Parent.Size.Width - iN.Position.X);
            else if (i.Parent.Position.X - i.Parent.Size.Width > iN.Parent.Position.X) // wraps around on right side of i.Parent?
                ptN.X = i.Parent.Position.X + i.Parent.Size.Width + iN.Position.X - iN.Parent.Position.X;
            if (i.Parent.Position.Y + i.Parent.Size.Width < iN.Parent.Position.Y) // wraps around on left side of i.Parent?
                ptN.Y = i.Parent.Position.Y - (iN.Parent.Position.Y + iN.Parent.Size.Width - iN.Position.Y);
            else if (i.Parent.Position.Y - i.Parent.Size.Width > iN.Parent.Position.Y) // wraps around on right side of i.Parent?
                ptN.Y = i.Parent.Position.Y + i.Parent.Size.Width + iN.Position.Y - iN.Parent.Position.Y;

            // see if they collided, and if they did...
            double fDistance = Distance(i.Position, ptN);
            if (fDistance < (i.Size.Width / 2 + iN.Size.Width / 2))
            {
                // Calculate the angle of the imaginary axis through the 
                // centres of the balls with the horizontal axis. That is: 
                // tan a = dy/dx.
                double dTheta = Math.Atan2(i.Position.Y - iN.Position.Y, i.Position.X - iN.Position.X);

                // Rotate the motion vectors of both balls by the inverse 
                // angle (-a); this has the effect of aligning the collision 
                // to the horizontal axis.
                // x' = x × cosa - y × sina
                // y' = x × sina + y × cosa 
                PointF V1 = new PointF((float)(i.Velocity.X * Math.Cos(-dTheta) - i.Velocity.Y * Math.Sin(-dTheta)),
                    (float)(i.Velocity.X * Math.Sin(-dTheta) + i.Velocity.Y * Math.Cos(-dTheta)));
                PointF V2 = new PointF((float)(iN.Velocity.X * Math.Cos(-dTheta) - iN.Velocity.Y * Math.Sin(-dTheta)),
                    (float)(iN.Velocity.X * Math.Sin(-dTheta) + iN.Velocity.Y * Math.Cos(-dTheta)));

                // Swap the horizontal components of the vectors, as in the 
                // one-dimensional case; the vertical components of the vectors 
                // remain unchanged.
                float fTemp = V1.X; V1.X = V2.X; V2.X = fTemp;

                // Rotate the vectors back (+a). 
                i.Velocity = new PointF((float)(V1.X * Math.Cos(dTheta) - V1.Y * Math.Sin(dTheta)),
                    (float)(V1.X * Math.Sin(dTheta) + V1.Y * Math.Cos(dTheta)));
                iN.Velocity = new PointF((float)(V2.X * Math.Cos(dTheta) - V2.Y * Math.Sin(dTheta)),
                    (float)(V2.X * Math.Sin(dTheta) + V2.Y * Math.Cos(dTheta)));

                // send an event to the client so it can log it, count it, make a sound, etc.
                if (CollisionDetectedEvent != null)
                    CollisionDetectedEvent(this, new CollisionDetectedEventArgs(i, iN));
            }
        }

        /// <summary>
        /// Sets the parent of the specified object to the appropriate tile.
        /// </summary>
        /// <param name="ind">The ind.</param>
        private void SetParent(SelectableObject o)
        {
            Tile parent = o.Parent;
            if (parent != null)
            {
                // if I'm in my parent's region, no need to search at all
                if (parent.PointInRegion(o.Position))
                    return;

                // remove it from its current parent's list
                parent.RemoveObject(o);
                o.Parent = null;

                // Then search my current parent's neighbors and see if I 
                // moved into one of them.
                IEnumerable<Tile> neighbors = parent.Neighbors;
                foreach (Tile t in neighbors)
                {
                    if (t.PointInRegion(o.Position))
                    {
                        o.Parent = t;
                        t.AddObject(o);
                        return;
                    }
                }
            }

            // if we still haven't found it, search every tile
            Debug.WriteLine("World.SetParent: object not in parent nor parent's neighbors.");
            double dShortestDistance = double.PositiveInfinity;
            Tile tShortestDistance = null;
            foreach (Tile t in Tiles.AllTiles)
            {
                if (t.PointInRegion(o.Position))
                {
                    t.AddObject(o);
                    o.Parent = t;
                    return;
                }
                double dDistance = Distance(t.Center, o.Position);
                if (dDistance < dShortestDistance)
                {
                    dShortestDistance = dDistance;
                    tShortestDistance = t;
                }
            }
            // if we made it here, then just put it in the closest tile
            Debug.WriteLine("World.SetParent: parent found for lost object.");
            tShortestDistance.AddObject(o);
            o.Parent = tShortestDistance;

            // just a double-check
            if (o.Parent == null)
                throw new ApplicationException("World.SetParent: no suitable tile found for lost object.");
        }

        /// <summary>
        /// Moves the specified object a specified distance in a specified direction
        /// from its current location.
        /// </summary>
        /// <param name="ind">The object to move.</param>
        /// <param name="fDistance">The distance to move the object.</param>
        /// <param name="theta">The direction to move the object (I believe 0 degrees is east).</param>
        private void MovePoint(SelectableObject o, double fDistance, double theta)
        {
            // update position
            o.Position = new PointF((Width + o.Position.X + (int)(fDistance * Math.Cos(theta))) % Width, 
                (Height + o.Position.Y + (int)(Math.Round(fDistance * Math.Sin(theta)))) % Height);

            SetParent(o);
        }

        /// <summary>
        /// Shortens the specified velocity vector by 75% in magnitude but preserves
        /// the direction.
        /// </summary>
        /// <param name="velocity">The velocity.</param>
        /// <returns>The new velocity.</returns>
        private static PointF ShortenVelocity(PointF velocity)
        {
            // shorten the velocity vector by 75%
            double theta = Math.Atan2(velocity.Y, velocity.X);
            double radiusNew = Distance(new PointF(0, 0), velocity) * 0.75f;
            return new PointF((float)(radiusNew * Math.Cos(theta)), (float)(radiusNew * Math.Sin(theta)));
        }

        /// <summary>
        /// Calculates the distances between the two points.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <returns>The distance between the points.</returns>
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        /// <summary>
        /// Calculates the distances between the two points.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <returns>The distance between the points.</returns>
        public static double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        /// <summary>
        /// Calculates the distances between the centers of the two objects.
        /// </summary>
        /// <param name="s1">The first object.</param>
        /// <param name="s2">The second object.</param>
        /// <returns>The distance between the objects.</returns>
        public static double Distance(SelectableObject s1, SelectableObject s2)
        {
            if (s1.Parent != s2.Parent)
            {
                // compute the distance if we crossed a world edge to get from one
                // to the other.  The trick is, there are up to eight different
                // transformations to look at.
            }
            return Distance(s1.Center, s2.Center);
        }

        /// <summary>
        /// If they click on an Inhabitant, select (or deselect) it, else, 
        /// select (or deselect) the tile in which they clicked.
        /// </summary>
        /// <param name="pt">Point in screen coordinates where mouse was clicked.</param>
        /// <param name="offset">Offset from origin of world to upper left of screen.</param>
        /// <param name="scale">Zoom factor.</param>
        public void SelectAt(Point pt, Point offset, double scale)
        {
            Point ptClick = new Point((int)((pt.X + offset.X) / scale), (int)((pt.Y + offset.Y) / scale));
            foreach (Tile t in m_tiles.AllTiles)
            {
                if (t.PointInRegion(ptClick))
                {
                    // find SelectableObject in this tile
                    foreach (SelectableObject ind in t.Objects(typeof(SelectableObject)))
                    {
                        if (ind.PointInRegion(ptClick))
                        {
                            if (ind != m_selected)
                            {
                                if (m_selected != null)
                                    m_selected.Selected = false;
                                ind.Selected = true;
                                Selected = ind;
                            }
                            else
                            {
                                if (m_selected != null)
                                    m_selected.Selected = false;
                                Selected = null;
                            }
                            return;
                        }
                    }
                    foreach (Tile tt in t.Neighbors)
                    {
                        // find SelectableObject in this tile
                        foreach (SelectableObject ind in tt.Objects(typeof(SelectableObject)))
                        {
                            if (ind.PointInRegion(ptClick))
                            {
                                if (ind != m_selected)
                                {
                                    if (m_selected != null)
                                        m_selected.Selected = false;
                                    ind.Selected = true;
                                    Selected = ind;
                                }
                                else
                                {
                                    if (m_selected != null)
                                        m_selected.Selected = false;
                                    Selected = null;
                                }
                                return;
                            }
                        }
                    }
                    if (t != m_selected)
                    {
                        if (m_selected != null)
                            m_selected.Selected = false;
                        t.Selected = true;
                        Selected = t;
                    }
                    else
                    {
                        if (m_selected != null)
                            m_selected.Selected = false;
                        Selected = null;
                    }
                    return;
                }
            }

        }

        /// <summary>
        /// Add the specified Inhabitant to the world.
        /// </summary>
        /// <param name="ind"></param>
        public void Add(SelectableObject o)
        {
            Tiles.AddObject(o); // if it isn't in this tile, the tile should find the right tile
        }

        /// <summary>
        /// Add the specified object to the world.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="tHint"></param>
        public void Add(SelectableObject o, Tile tHint)
        {
            if (tHint != null && tHint.PointInRegion(o.Position))
                tHint.AddObject(o);
            else
                Add(o);
        }

        private List<Incident.RelocationRequestedEventArgs> m_lstMoveEvents;
        public void OnRelocationRequested(object sender, Incident.RelocationRequestedEventArgs e)
        {
            if (m_lstMoveEvents == null)
                m_lstMoveEvents = new List<Incident.RelocationRequestedEventArgs>();
            m_lstMoveEvents.Add(e);
        }

        public void LogResults(string strLine)
        {
            if (LogEvent != null)
                LogEvent(this, new LogEventArgs(strLine));
        }

        #region MessageSentEvent
        /// <summary>
        /// This class represents the data associated with the MessageSentEvent.
        /// </summary>
        public class MessageSentEventArgs : EventArgs
        {
            private Message m_message;
            public Message Message { get { return m_message; } set { m_message = value; } }
            private SelectableObject m_from;
            public SelectableObject From { get { return m_from; } set { m_from = value; } }
            private SelectableObject m_to;
            public SelectableObject To { get { return m_to; } set { m_to = value; } }
            public MessageSentEventArgs(Message m, SelectableObject from, SelectableObject to) { m_message = m; m_from = from; m_to = to; }
        }
        /// <summary>
        /// Delegate for the MessageSentEvent.
        /// </summary>
        public delegate void MessageSentDelegate(object sender, MessageSentEventArgs e);
        /// <summary>
        /// Occurs when a message is sent by an inhabitant of the <see cref="World"/>.
        /// </summary>
        public event MessageSentDelegate MessageSentEvent;
        #endregion

        #region LogEvent
        /// <summary>
        /// The data associated with the LogEvent.
        /// </summary>
        public class LogEventArgs
        {
            private string m_strMessage;
            public string Message { get { return m_strMessage; } set { m_strMessage = value; } }
            public LogEventArgs(string m) { m_strMessage = m; }
        }
        /// <summary>
        /// Delegate for the LogEvent.
        /// </summary>
        public delegate void LogDelegate(object sender, LogEventArgs e);
        /// <summary>
        /// Occurs when someone in the world requests that a string be added to whatever
        /// log might be active.  These events should be consumed by anyone monitoring
        /// activity during a simulation run.
        /// </summary>
        public event LogDelegate LogEvent;        
        #endregion

        #region ObjectSelectedEvent
        /// <summary>
        /// The data associated with the ObjectSelectedEvent.
        /// </summary>
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
        /// <summary>
        /// Delegate for the ObjectSelectedEvent.
        /// </summary>
        public delegate void ObjectSelectedEventDelegate(object sender, ObjectSelectedEventArgs e);
        /// <summary>
        /// Occurs when an object in the <see cref="World"/> is selected.
        /// </summary>
        public event ObjectSelectedEventDelegate ObjectSelectedEvent; 
        #endregion

        #region CollisionDetectedEvent
        /// <summary>
        /// The data associated with the CollisionDetectedEvent.
        /// </summary>
        public class CollisionDetectedEventArgs : EventArgs
        {
            public Inhabitant m_ind1, m_ind2;
            public CollisionDetectedEventArgs(Inhabitant i1, Inhabitant i2)
            {
                m_ind1 = i1;
                m_ind2 = i2;
            }
        }
        /// <summary>
        /// Delegate for the CollisionDetectedEvent.
        /// </summary>
        public delegate void CollisionDetectedEventDelegate(object sender, CollisionDetectedEventArgs e);
        /// <summary>
        /// Occurs when a collision is detected between two object in the <see cref="World"/>.
        /// </summary>
        public event CollisionDetectedEventDelegate CollisionDetectedEvent; 
        #endregion

        #region PreTestRunEvent
        /// <summary>
        /// The data associated with the PreTestRunEvent.
        /// </summary>
        public class PreTestRunEventArgs
        {
            public PreTestRunEventArgs() { }
        }
        /// <summary>
        /// Delegate for the PreTestRunEvent.
        /// </summary>
        public delegate void PreTestRunDelegate(object sender, PreTestRunEventArgs e);
        /// <summary>
        /// Occurs when .
        /// </summary>
        public event PreTestRunDelegate PreTestRunEvent;
        #endregion

        #region PostTestRunEvent
        /// <summary>
        /// The data associated with the PostTestRunEvent.
        /// </summary>
        public class PostTestRunEventArgs
        {
            public PostTestRunEventArgs() { }
        }
        /// <summary>
        /// Delegate for the PostTestRunEvent.
        /// </summary>
        public delegate void PostTestRunDelegate(object sender, PostTestRunEventArgs e);
        /// <summary>
        /// Occurs when .
        /// </summary>
        public event PostTestRunDelegate PostTestRunEvent;
        #endregion

        #region PreStepEvent
        /// <summary>
        /// The data associated with the PreStepEvent.
        /// </summary>
        public class PreStepEventArgs
        {
            public PreStepEventArgs() { }
        }
        /// <summary>
        /// Delegate for the PreStepEvent.
        /// </summary>
        public delegate void PreStepDelegate(object sender, PreStepEventArgs e);
        /// <summary>
        /// Occurs when .
        /// </summary>
        public event PreStepDelegate PreStepEvent;
        #endregion

        #region PostStepEvent
        /// <summary>
        /// The data associated with the PostStepEvent.
        /// </summary>
        public class PostStepEventArgs
        {
            public PostStepEventArgs() { }
        }
        /// <summary>
        /// Delegate for the PostStepEvent.
        /// </summary>
        public delegate void PostStepDelegate(object sender, PostStepEventArgs e);
        /// <summary>
        /// Occurs when .
        /// </summary>
        public event PostStepDelegate PostStepEvent;
        #endregion

        #region PreTickEvent
        /// <summary>
        /// The data associated with the PreTickEvent.
        /// </summary>
        public class PreTickEventArgs : EventArgs
        {
            private int m_repeat;
            public int Repeat { get { return m_repeat; } }
            private int m_tick;
            public int Tick { get {return m_tick;} }
            public PreTickEventArgs(int tick, int repeat) { m_tick = tick; m_repeat = repeat; }
        }
        /// <summary>
        /// Delegate for the PreTickEvent.
        /// </summary>
        public delegate void PreTickDelegate(object sender, PreTickEventArgs e);
        /// <summary>
        /// Occurs when the Tick method of an object in the world is about to be called.
        /// </summary>
        public event PreTickDelegate PreTickEvent;
        #endregion

        #region PostTickEvent
        /// <summary>
        /// The data associated with the PostTickEvent.
        /// </summary>
        public class PostTickEventArgs
        {
            private int m_repeat;
            public int Repeat { get { return m_repeat; } }
            private int m_tick;
            public int Tick { get { return m_tick; } }
            public PostTickEventArgs(int tick, int repeat) { m_tick = tick; m_repeat = repeat; }
        }
        /// <summary>
        /// Delegate for the PostTickEvent.
        /// </summary>
        public delegate void PostTickDelegate(object sender, PostTickEventArgs e);
        /// <summary>
        /// Occurs after the Tick method of an object in the world has been called.
        /// </summary>
        public event PostTickDelegate PostTickEvent;
        #endregion

        public void DoPreStepEvent()
        {
            if (PreStepEvent != null)
                PreStepEvent(this, new World.PreStepEventArgs());
        }

        public void DoPostStepEvent()
        {
            if (PostStepEvent != null)
                PostStepEvent(this, new World.PostStepEventArgs());
        }

        public void DoPreTestRunEvent()
        {
            if (PreTestRunEvent != null)
                PreTestRunEvent(this, new World.PreTestRunEventArgs());
        }

        public void DoPostTestRunEvent()
        {
            if (PostTestRunEvent != null)
                PostTestRunEvent(this, new World.PostTestRunEventArgs());
        }

        public void DoPreTickEvent(int tick, int repeat)
        {
            if (PreTickEvent != null)
                PreTickEvent(this, new PreTickEventArgs(tick, repeat));
        }

        public void DoPostTickEvent(int tick, int repeat)
        {
            
            if (PostTickEvent != null)
                PostTickEvent(this, new PostTickEventArgs(tick, repeat));
        }
    }
}
