using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;

namespace TokenRing
{
    /// <summary>
    /// This is the base class for the mobile agent, an agent that moves
    /// around the world, going into and out of range of sensors, and
    /// using the services provided by those sensors. 
    /// </summary>
    [Serializable]
    public class MobileAgent2 : SelectableObject
    {
        /// <summary>
        /// The mobile agent is passed the token when its current tower receives it
        /// and then when it is this mobile agent's turn.  When it has the token,
        /// it can send messages to perform operations, such as sending text messages.
        /// If the agent moves out of range and its tower drops it or to another tower 
        /// and tries to send a message, the tower will have generated a new token 
        /// and the messages will be ignored.
        /// </summary>
        private Guid m_token;
        public Guid Token { get { return m_token; } set { m_token = value; } }

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
        private string m_addressBook;
        public string AddressBook { get { return m_addressBook; } set { m_addressBook = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAgent2"/> class.
        /// </summary>
        public MobileAgent2()
        {
            m_addressBook = "";
            BackgroundColor = System.Drawing.Color.Red;
            Size = new System.Drawing.Size(5, 5);
            m_token = Guid.Empty;
        }

        /// <summary>
        /// Draws the specified bitmap graphics.
        /// </summary>
        /// <param name="bitmapGraphics">The bitmap graphics.</param>
        /// <param name="rectViewport">The viewport.</param>
        /// <param name="fScale">The scale.</param>
        public override void Draw(System.Drawing.Graphics bitmapGraphics, System.Drawing.Rectangle rectViewport, float fScale)
        {
            base.Draw(bitmapGraphics, rectViewport, fScale);
        }

        /// <summary>
        /// Ticks this instance.
        /// </summary>
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

            if (Token != Guid.Empty)
            {
                // it's our turn, we have the token.  If we have something to do, do it.
                if (AddressBook.Length > 0)
                {
                    Outbox.Add(new TextMessage("Hello from " + Label, this, AddressBook));
                    AddressBook = String.Empty;
                }
                else
                {
                    Outbox.Add(new TheTokenIsYoursMessage(this, Token, ParentTower));
                    Token = Guid.Empty;
                }
            }
        }

        private void processInbox()
        {
            foreach (Message m in Inbox)
            {
                
                switch (m.GetType().ToString())
                {
                    case "TokenRing.PingMessage":
                        if (m.Sender.GetType().ToString() == "TokenRing.Station2")
                            chooseTower(m.Sender);
                        break;

                    case "TokenRing.TextMessage":
                        TextMessage tm = (TextMessage)m;
                        World.LogResults(Label + ": received '" + tm.Text + "'.");
                        break;

                    case "TokenRing.TheTokenIsYoursMessage":
                        TheTokenIsYoursMessage ttiym = (TheTokenIsYoursMessage)m;
                        if (ttiym.Recipient == this)
                            Token = ttiym.Token;
                        break;

                    case "TokenRing.InvalidTokenMessage":
                        InvalidTokenMessage itm = (InvalidTokenMessage)m;
                        if (itm.Recipient == this)
                            Token = Guid.Empty;
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Chooses the tower.
        /// </summary>
        /// <param name="Sender">The sender.</param>
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

        /// <summary>
        /// Calcs the distance.
        /// </summary>
        /// <param name="towerPos">The tower pos.</param>
        /// <returns></returns>
        private int calcDistance(System.Drawing.PointF towerPos)
        {
            double xDiff = towerPos.X - this.Position.X;
            double yDiff = towerPos.Y - this.Position.Y;

            return (int)(Math.Sqrt(Math.Pow(xDiff,2.0) + Math.Pow(yDiff, 2.0)));
        }
    }
}
