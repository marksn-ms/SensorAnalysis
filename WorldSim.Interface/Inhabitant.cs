#define WORLD_NO_INERTIA

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace WorldSim.Interface
{

    [Serializable]
    public class PerceptionActionReward
    {
        public World.Actions Action;
        public double Reward;
        public string Perception;
        public PerceptionActionReward()
        {
#if WORLD_NO_INERTIA
            Action = World.Actions.actStay;
#else
            Action = World.Actions.actContinue;
#endif
            Reward = 0.0d;
        }
        public PerceptionActionReward(World.Actions a, double d, string s)
        {
            Action = a;
            Reward = d;
            Perception = s;
        }
        public override string ToString()
        {
            return Perception + ", " + Action.ToString() + " -> " + Reward.ToString() + "\r\n";
        }
    }

    [Serializable]
    public class Inhabitant : SelectableObject
    {
        private static int m_nNextID = 0;
        public int ID { get; private set; }
        public int Ticks { get; set; }
        public double RewardTotal { get; set; }
        public virtual double Reward { get; set; }

        /// <summary>
        /// How far the inhabitant can sense other agents/goals.
        /// </summary>
        [CategoryAttribute("Initialization")]
        public double SensorRange { get; set; }

        public Inhabitant()
            : base()
        {
            ID = m_nNextID++;
            Position = new PointF(0,0);
            Size = new Size(5, 5);
            ForeColor = Color.Black;
            BackgroundColor = Color.Red;
            SelectedColor = Color.Green;
            Selected = false;
            Symbol = 'a';
            SensorRange = 100; // default to the same as the tile size
        }

        public override void Tick()
        {
            // increment the tick counter
            Ticks++;

#if PerceptionHistory
            History.Insert(0,new PerceptionActionReward(this.Action, Reward, World.Perceptions(this)));
            if (History.Count > 100)
                History.RemoveRange(100, History.Count - 100);
#endif

            if (Parent != null)
            {
                // Each inhabitant "eats" half a unit of resources from the tile in which it
                // resides, and another half a unit from the adjoining tiles.
                Parent.Resources -= 0.005f;
                foreach (Incident i in Parent.Objects(typeof(Incident)))
                    i.Resources -= 0.005f; // deduct units from incident based on sensor formula
                float fUnits = 0.005f / Parent.Sides; // in case we ever have hex tiles
                foreach (Tile tNeighbor in Parent.Neighbors)
                {
                    tNeighbor.Resources -= fUnits;
                    foreach (Incident i in Parent.Objects(typeof(Incident)))
                        i.Resources -= fUnits; // deduct units from incident based on sensor formula
                }
            }

            // choose action (base class always says status-quo)
#if WORLD_NO_INERTIA
            Action = World.Actions.actStay;
#else
            Action = World.Actions.actContinue;
#endif
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
            Brush b = Selected || (Parent != null && Parent.Selected) ? m_brushSelected : m_brushBack;
            DrawEllipse(g, rectViewport, Position, new Size((int)Size.Width, (int)Size.Height), scale, b, m_penFore);
        }

        /// <summary>
        /// This method is called when an agent needs to drop a message that
        /// can be received by other agents.
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(Marker msg)
        {
            msg.Position = Position;
            msg.Parent = Parent;
            World.Add(msg, Parent);
        }

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "Generic inhabitant capable of drawing itself and just sitting there during the simulation, doing just about nothing."; }
    }
}
