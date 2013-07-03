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
    public class Station2 : SelectableObject
    {
        private DateTime m_dateLastPing;
        private int m_tickCount = 0;

        private Station2 m_nextStation;
        public Station2 Next { get { return m_nextStation; } set { m_nextStation = value; } }

        private SelectableObjectListItem m_tokenAgent;
        public SelectableObjectListItem TokenAgent { get { return m_tokenAgent; } set { m_tokenAgent = value; } }

        private static Pen m_penArrow = null;
        public Pen Arrow { get { return m_penArrow; } set { m_penArrow = value; } }

        private Guid m_token;
        public Guid Token { get { return m_token; } set { m_token = value; } }

        /// <summary>
        /// This is the list of mobile agents about which this station is aware
        /// along with the last successful ping datetime
        /// </summary>
        public List<SelectableObjectListItem> m_agents;

        /// <summary>
        /// Constructor.  Initialize data members.
        /// </summary>
        public Station2()
        {
            m_agents = new List<SelectableObjectListItem>();

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
                foreach (SelectableObjectListItem o in m_agents)
                {
                    Point startPoint = new Point((int)((o.agent.Position.X + (o.agent.Size.Width / 2)) * fScale), (int)((o.agent.Position.Y + (o.agent.Size.Height / 2)) * fScale));
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

            // if we have the token, and we have agents, and we haven't assigned a current
            // agent, then pick the first agent as the current agent.
            if (Token != Guid.Empty)
            {
                if (m_agents.Count > 0)
                {
                    if (TokenAgent == null)
                    {
                        m_tokenAgent = m_agents[0];
                        Outbox.Add(new TheTokenIsYoursMessage(this, Token, m_tokenAgent.agent));
                    }
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
            List<SelectableObjectListItem> lstDelete = new List<SelectableObjectListItem>();
            foreach (SelectableObjectListItem m in m_agents)
            {
                if ((m_tickCount - m.lastTick) > 4)
                {
                    lstDelete.Add(m);

                    if (m_tokenAgent == m)
                    {
                        int nIndex = m_agents.IndexOf(m);
                        if (nIndex >= 0)
                        {
                            if (nIndex < m_agents.Count - 1)
                            {
                                m_tokenAgent = m_agents[++nIndex];
                                Token = Guid.NewGuid();
                                Outbox.Add(new TheTokenIsYoursMessage(this, Token, m_tokenAgent.agent));
                            }
                        }
                    }
                }
            }

            foreach (SelectableObjectListItem m in lstDelete)
            {
                World.LogResults(this.Label + " pruned " + m.agent.Label);
                m_agents.Remove(m);
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
                        if (tmpM.recipient.Label == this.Label)
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
                        break;

                    case "TokenRing.TheTokenIsYoursMessage":
                        World.LogResults(m.ToString() + " received");
                        TheTokenIsYoursMessage ttiym = (TheTokenIsYoursMessage)m;
                        if (ttiym.Token != Token)
                        {
                            Outbox.Add(new InvalidTokenMessage(this, ttiym.Sender));
                        }
                        else
                        {
                            int nIndex = m_agents.IndexOf(m_tokenAgent);
                            if (nIndex >= 0)
                            {
                                if (nIndex < m_agents.Count - 1)
                                {
                                    m_tokenAgent = m_agents[++nIndex];
                                    Outbox.Add(new TheTokenIsYoursMessage(this, Token, m_tokenAgent.agent));
                                }
                                else
                                {
                                    Outbox.Add(new TheTokenIsYoursRoutedMessage(this, Next, Token));
                                    m_tokenAgent = null;
                                    Token = Guid.Empty;
                                }
                            }
                        }
                        break;

                    case "TokenRing.TextMessage":
                        World.LogResults(m.ToString() + " received");
                        TextMessage tm = (TextMessage)m;
                        if (tm.Sender != m_tokenAgent.agent)
                        {
                            Outbox.Add(new InvalidTokenMessage(this, tm.Sender));
                        }
                        else
                        {
                            foreach (SelectableObjectListItem i in m_agents)
                            {
                                if (i.agent.Label == tm.RecipentLabel)
                                {
                                    Outbox.Add(new TextMessage(tm.Text, tm.Sender, tm.RecipentLabel));
                                    tm = null;
                                    break;
                                }
                            }
                        }
                        break;

                    case "TokenRing.TextRoutedMessage":
                        World.LogResults(m.ToString() + " received");
                        TextRoutedMessage trm = (TextRoutedMessage)m;
                        if (trm.Sender == this) // went all the way around and back to ourself
                        {
                            Outbox.Add(new TextMessage(trm.RecipentLabel + " is not available.", this, trm.Sender.Label));
                            break;
                        }
                        foreach (SelectableObjectListItem i in m_agents)
                        {
                            if (i.agent.Label == trm.RecipentLabel)
                            {
                                Outbox.Add(new TextMessage(trm.Text, trm.Sender, trm.RecipentLabel));
                                trm = null;
                                break;
                            }
                        }
                        if (trm != null)
                            Outbox.Add(new TextRoutedMessage(trm.Text, trm.Sender, Next, trm.RecipentLabel));
                        break;

                    default:
                        break;
                }
            }
        }

        private void updateMobileAgent(SelectableObject Sender)
        {
            for (int i = 0; i < m_agents.Count; i++)
            {
                if (m_agents[i].agent.Label == Sender.Label)
                {
                    SelectableObjectListItem itm = m_agents[i];
                    itm.lastTick = m_tickCount;
                    m_agents[i] = itm;
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
            m_agents.Add(ma);
        }

        /// <summary>
        /// Releases the mobile agent.
        /// </summary>
        /// <param name="Sender">The mobile agent to unregister.</param>
        public void releaseMobileAgent(SelectableObject m)
        {
            List<SelectableObjectListItem> lstDelete = new List<SelectableObjectListItem>();
            for (int i = 0; i < m_agents.Count; i++)
            {
                if (m_agents[i].agent.Label == m.Label)
                    lstDelete.Add(m_agents[i]);
            }
            foreach (SelectableObjectListItem itemDelete in lstDelete)
                m_agents.Remove(itemDelete);
        }
    }
}
