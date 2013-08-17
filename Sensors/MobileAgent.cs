using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;

namespace Sensors
{

    /// <summary>
    /// This is a structure to hold a selectable object as well as the last 
    /// ping date/time.
    /// </summary>
    public struct SelectableObjectListItem
    {
        public SelectableObject agent;
        public int lastTick;
    }

    /// <summary>
    /// This is the base class for the mobile agent, an agent that moves
    /// around the world, going into and out of range of sensors, and
    /// using the services provided by those sensors. 
    /// </summary>
    [Serializable]
    public class MobileAgent : SelectableObject
    {
        private int tickCnt = 1;
        private SelectableObject m_parentTower = null;
        public SelectableObject ParentTower
        {
            get
            {
                return m_parentTower;
            }
            set
            {
                m_parentTower = value;
            }
        }

        /// <summary>
        /// This is the address book containing
        /// other mobile agent ID's for messaging.
        /// </summary>
        public List<String> addressBook;

        public MobileAgent()
        {
            //m_objectsInRange = new Dictionary<SelectableObject, DateTime>();
            addressBook = new List<String>();
            BackgroundColor = System.Drawing.Color.Red;
            Size = new System.Drawing.Size(5, 5);
            //Velocity = 0.1f;
        }

        public override void Draw(System.Drawing.Graphics bitmapGraphics, System.Drawing.Rectangle rectViewport, float fScale)
        {
            base.Draw(bitmapGraphics, rectViewport, fScale);
        }

        public override void Tick()
        {
            base.Tick();
            tickCnt -= 1;
            if (tickCnt < 1)
            {
                Action = (World.Actions)Parent.World.Random.Next((int)World.Actions.Max + 1); // returns 0-4
                tickCnt = Parent.World.Random.Next(10) + 10;
            }

            // process mail
            processInbox();

        }

        private void processInbox()
        {
            foreach (Message m in Inbox)
            {
                
                switch (m.GetType().ToString())
                {
                    case "Sensors.PingMessage":
                        if (m.Sender.GetType().ToString() == "Sensors.Station")
                        {
                            chooseTower(m.Sender);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void chooseTower(SelectableObject Sender)
        {
            // is the sending tower the same as my parent tower?
            // if so, send ping acknoledgement to let it know
            // I am still there
            if (m_parentTower != null && Sender.Label == m_parentTower.Label)
            {
                Outbox.Add(new PingAckMessage(this));
            }
            else  // parent tower and pinging tower are different
            {
                 // Decide if the pinging tower is closer than exiting parent tower.
                // If it is closer, or unlikely equal distance, then make the ping
                // sending tower the new parent tower, send new tower registration message
                if (m_parentTower == null || (calcDistance(Sender.Position) <= calcDistance(m_parentTower.Position)))
                {
                    Outbox.Add(new RegisterMobileAgentMessage(this, Sender));
                    m_parentTower = Sender;
                }
            }
        }

        private int calcDistance(System.Drawing.PointF towerPos)
        {
            double xDiff = towerPos.X - this.Position.X;
            double yDiff = towerPos.Y - this.Position.Y;

            return (int)(Math.Sqrt(Math.Pow(xDiff,2.0) + Math.Pow(yDiff, 2.0)));
        }
    }
}
