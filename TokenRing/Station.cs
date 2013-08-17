using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using WorldSim.Interface;
using System.Threading;

namespace TokenRing
{

    /// <summary>
    /// This is the base class for station objects.  The main thing that it does
    /// is provide the ability to make services available to agents.
    /// </summary>
    public class Station : SelectableObject
    {
        private DateTime m_dateLastPing;
        private int m_tickCount = 0;

        public Station Next { get; set; }
        public Pen Arrow { get; set; }
        public List<Message> Pending { get; set; }
        public List<Message> Active { get; set; }
        public Guid Token { get; set; }

        /// <summary>
        /// This is the list of mobile agents about which this station is aware
        /// along with the last successful ping datetime
        /// </summary>
        public List<SelectableObjectListItem> Agents { get; set; }

        /// <summary>
        /// Constructor.  Initialize data members.
        /// </summary>
        public Station()
        {
            Agents = new List<SelectableObjectListItem>();
            Pending = new List<Message>();
            Active = new List<Message>();

            m_dateLastPing = DateTime.Now;

            Next = null;
            Token = Guid.Empty;

            if (Arrow == null)
            {
                Arrow = new Pen(Color.Bisque);
                Arrow.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            }
        }

        public override void Draw(System.Drawing.Graphics bitmapGraphics, System.Drawing.Rectangle rectViewport, float fScale)
        {
            Brush b = m_brushBack;

            if (Token != Guid.Empty)
            {
                b = Brushes.Red;
            }
            else if (Selected || (Parent != null && Parent.Selected))
            {
                b = m_brushSelected;
                foreach (SelectableObjectListItem o in Agents)
                {
                    Point startPoint = new Point((int)((o.agent.Position.X +(o.agent.Size.Width / 2)) * fScale), (int)((o.agent.Position.Y + (o.agent.Size.Height / 2)) * fScale));
                    Point endPoint = new Point((int)(this.Position.X * fScale), (int)(this.Position.Y * fScale));
                    bitmapGraphics.DrawLine(SystemPens.Highlight, startPoint, endPoint);
                }
            }
            if (Next != null)
                bitmapGraphics.DrawLine(Arrow, new PointF(Center.X * fScale, Center.Y * fScale), new PointF(Next.Center.X * fScale, Next.Center.Y * fScale));

            DrawTriangle(bitmapGraphics, rectViewport, Position, new Size((int)Size.Width, (int)Size.Height), fScale, b, m_penFore);
        }

        /// <summary>
        /// Handle the tick of the clock for the Station object.
        /// </summary>
        public override void Tick()
        {
            m_tickCount += 1;
            base.Tick();

            // ping out
            pingOut();

            // process mail
            processInbox();

            // if we haven't heard from the object in a while, 'forget' it.
            pruneAgents();

            if (Token != Guid.Empty)
            {
                // it's our time to process stuff
                Monitor.Enter(Pending);
                Active.AddRange(Pending);
                Pending.Clear();
                Monitor.Exit(Pending);

                // now, process the items in the active queue
                if (Active.Count > 0)
                {
                    Message m = Active[0];

                    switch (m.GetType().ToString())
                    {
                        case "TokenRing.TextMessage":
                            TextMessage tm = (TextMessage)m;
                            Outbox.Add(new TextRoutedMessage(tm.Text, this, Next, tm.RecipientLabel));
                            break;

                        default:
                            break;
                    }
                    
                    Active.Remove(m);
                }
                else
                {
                    // pass the token on to the next station
                    Outbox.Add(new TheTokenIsYoursRoutedMessage(this, Next, Token));
                    Token = Guid.Empty;
                }
            }
        }

        private void pingOut()
        {
            // if it has been over 2 seconds, send a ping to see who is out there
            //DateTime dtPingYet = m_dateLastPing;
            //dtPingYet.AddSeconds(2.0d);
            if ((m_tickCount % 2) == 0)
                Outbox.Add(new PingMessage(this));
        }

        private void pruneAgents()
        {
            // if it has been over two seconds since hearing from an agent
            // then prune agent from agent list

            foreach (SelectableObjectListItem m in new List<SelectableObjectListItem>(Agents))
            {
                if ((m_tickCount-m.lastTick) > 4)
                {
                    //World.LogResults(DateTime.Now + " - Station: " + this.Label + " PRUNED MobileAgent: " + m.agent.Label);
                    World.LogResults(this.Label + " pruned " + m.agent.Label);
                    Agents.Remove(m);
                }
            }
        }

        private void processInbox()
        {
            foreach (Message m in Inbox)
            {
                switch (m.GetType().ToString())
                {
                    case "TokenRing.PingAckMessage":
                        if (m.Sender.GetType().ToString() == "TokenRing.MobileAgent")
                            updateMobileAgent(m.Sender);
                        break;

                    case "TokenRing.RegisterMobileAgentMessage":
                        World.LogResults(m.ToString() + " received");
                        RegisterMobileAgentMessage tmpM = (RegisterMobileAgentMessage)m;

                        // if this is the tower intended then register.
                        if (tmpM.Recipient.Label == this.Label)
                        {
                            registerMobileAgent(tmpM);
                        }
                        break;

                    case "TokenRing.ReleaseMobileAgentMessage":
                        World.LogResults(m.ToString() + " received");
                        releaseMobileAgent(m.Sender);
                        break;

                    case "TokenRing.TheTokenIsYoursRoutedMessage":
                        World.LogResults(m.ToString() + " received");
                        TheTokenIsYoursRoutedMessage mt = (TheTokenIsYoursRoutedMessage)m;
                        Token = mt.Token;
                        Active.Add(new PingMessage(this));
                        break;

                    case "TokenRing.TextMessage":
                        World.LogResults(m.ToString() + " received");
                        TextMessage tm = (TextMessage)m;
                        foreach (SelectableObjectListItem i in Agents)
                        {
                            if (i.agent.Label == tm.RecipientLabel)
                            {
                                Outbox.Add(new TextMessage(tm.Text, tm.Sender, tm.RecipientLabel));
                                tm = null;
                                break;
                            }
                        }
                        if (tm != null)
                            Pending.Add(tm);
                        break;

                    case "TokenRing.TextRoutedMessage":
                        World.LogResults(m.ToString() + " received");
                        TextRoutedMessage trm = (TextRoutedMessage)m;
                        if (trm.Sender == this) // went all the way around and back to ourself
                        {
                            Outbox.Add(new TextMessage(trm.RecipientLabel + " is not available.", this, trm.Sender.Label));
                            break;
                        }
                        foreach (SelectableObjectListItem i in Agents)
                        {
                            if (i.agent.Label == trm.RecipientLabel)
                            {
                                Outbox.Add(new TextMessage(trm.Text, trm.Sender, trm.RecipientLabel));
                                trm = null;
                                break;
                            }
                        }
                        if (trm != null)
                            Outbox.Add(new TextRoutedMessage(trm.Text, trm.Sender, Next, trm.RecipientLabel));
                        break;

                    default:
                        break;
                }
            }
        }

        private void updateMobileAgent(SelectableObject Sender)
        {
            for (int i = 0; i < Agents.Count; i++)
            {
                if (Agents[i].agent.Label == Sender.Label)
                {
                    SelectableObjectListItem itm = Agents[i];
                    itm.lastTick = m_tickCount;
                    Agents[i] = itm;
                }
            }
        }

        /// <summary>
        /// Registers the mobile agent.
        /// </summary>
        /// <param name="m">The mobile agent to register.</param>
        private void registerMobileAgent(RegisterMobileAgentMessage m)
        {
            SelectableObjectListItem ma = new SelectableObjectListItem();
            ma.agent = m.Sender;
            ma.lastTick = m_tickCount;
            Agents.Add(ma);
        }

        /// <summary>
        /// Releases the mobile agent.
        /// </summary>
        /// <param name="Sender">The mobile agent to unregister.</param>
        public void releaseMobileAgent(SelectableObject m)
        {
            List<SelectableObjectListItem> lstDelete = new List<SelectableObjectListItem>();
            for (int i = 0; i < Agents.Count; i++)
            {
                if (Agents[i].agent.Label == m.Label)
                    lstDelete.Add(Agents[i]);
            }
            foreach (SelectableObjectListItem itemDelete in lstDelete)
                Agents.Remove(itemDelete);
        }
    }
}
