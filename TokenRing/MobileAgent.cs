using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;

namespace TokenRing
{
    /// <summary>
    /// This is a structure to hold a selectable object as well as the last 
    /// ping date/time.
    /// </summary>
    public class SelectableObjectListItem
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
        public SelectableObject ParentTower { get; set; }

        /// <summary>
        /// This is the address book containing
        /// other mobile agent ID's for messaging.
        /// </summary>
        public string AddressBook { get; set; }

        public MobileAgent()
        {
            AddressBook = "";
            BackgroundColor = System.Drawing.Color.Red;
            Size = new System.Drawing.Size(5, 5);
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

            if (AddressBook.Length > 0)
            {
                Outbox.Add(new TextMessage("Hello from " + Label, this, AddressBook));
                AddressBook = String.Empty;
            }
        }

        private void processInbox()
        {
            foreach (Message m in Inbox)
            {
                
                switch (m.GetType().ToString())
                {
                    case "TokenRing.PingMessage":
                        if (m.Sender.GetType().ToString() == "TokenRing.Station")
                        {
                            chooseTower(m.Sender);
                        }
                        break;

                    case "TokenRing.TextMessage":
                        TextMessage tm = (TextMessage)m;
                        World.LogResults(Label + ": received '" + tm.Text + "'.");
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
            if (ParentTower != null && Sender.Label == ParentTower.Label)
            {
                Outbox.Add(new PingAckMessage(this));
            }
            else  // parent tower and pinging tower are different
            {
                 // Decide if the pinging tower is closer than exiting parent tower.
                // If it is closer, or unlikely equal distance, then make the ping
                // sending tower the new parent tower, send new tower registration message
                if (ParentTower == null || (calcDistance(Sender.Position) <= calcDistance(ParentTower.Position)))
                {
                    Outbox.Add(new RegisterMobileAgentMessage(this, Sender));
                    ParentTower = Sender;
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
