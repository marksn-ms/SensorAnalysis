using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using WorldSim.Interface;

namespace Sensors
{

    /// <summary>
    /// This is the base class for station objects.  The main thing that it does
    /// is provide the ability to make services available to agents.
    /// </summary>
    public class Station : SelectableObject
    {

        DateTime m_dateLastPing;
        int m_tickCount = 0;

        /// <summary>
        /// This is the list of mobile agents about which this station is aware
        /// along with the last successful ping datetime
        /// </summary>
        public List<SelectableObjectListItem> m_agents;

        /// <summary>
        /// This is the parent
        /// </summary>
        public SelectableObject parentStation { get; set; }

        /// <summary>
        /// Constructor.  Initialize data members.
        /// </summary>
        public Station()
        {
            m_agents = new List<SelectableObjectListItem>();
            m_dateLastPing = DateTime.Now;
        }

        public override void Draw(System.Drawing.Graphics bitmapGraphics, System.Drawing.Rectangle rectViewport, float fScale)
        {
            Brush b = m_brushBack;
            if (Selected || (Parent != null && Parent.Selected))
            {
                b = m_brushSelected;
                foreach (SelectableObjectListItem o in m_agents)
                {
                    Point startPoint = new Point((int)((o.agent.Position.X +(o.agent.Size.Width / 2)) * fScale), (int)((o.agent.Position.Y + (o.agent.Size.Height / 2)) * fScale));
                    Point endPoint = new Point((int)(this.Position.X * fScale), (int)(this.Position.Y * fScale));
                    bitmapGraphics.DrawLine(SystemPens.Highlight, startPoint, endPoint);
                }
            }

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

            foreach (SelectableObjectListItem m in new List<SelectableObjectListItem>(m_agents))
            {
                if ((m_tickCount-m.lastTick) > 4)
                {
                    //World.LogResults(DateTime.Now + " - Station: " + this.Label + " PRUNED MobileAgent: " + m.agent.Label);
                    m_agents.Remove(m);
                }
            }
        }

        private void processInbox()
        {
            foreach (Message m in Inbox)
            {
                
                switch (m.GetType().ToString())
                {
                    case "Sensors.PingAckMessage": 
                        if (m.Sender.GetType().ToString() == "Sensors.MobileAgent")
                            updateMobileAgent(m.Sender);
                        break;

                    case "Sensors.RegisterMobileAgentMessage":
                        //MobileAgent ma = (MobileAgent)(m.Sender);
                        RegisterMobileAgentMessage tmpM = (RegisterMobileAgentMessage)m;

                        // if this is the tower intended then register.
                        if (tmpM.Recipient.Label == this.Label)
                        {
                            registerMobileAgent(tmpM);
                        }
                        break;

                    case "Sensors.ReleaseMobileAgentMessage":
                        // Audit.printOut(m.ToString());
                        releaseMobileAgent(m.Sender);

                        // find LCA (least common ancestor)

                        // release mobile agent registration
                        // up to, but not including,
                        // LCA

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

        private void registerMobileAgent(RegisterMobileAgentMessage m)
        {

            if (this.parentStation != null)
            {
                Outbox.Add(new UpdateMobileAgentRecordRoutedMessage(m, this, this.parentStation, m.Sender, this));

                SelectableObjectListItem ma = new SelectableObjectListItem();
                ma.agent = m.Sender;
                ma.lastTick = m_tickCount;
                m_agents.Add(ma);
            }
        }

        private String showParentPaths(BaseStation thisBS, int cnt)
        {
            cnt++;
            if (thisBS.parentStation.Label == thisBS.Label || cnt > 3)
            {
                return thisBS.Label;
            }
            else
            {
                return thisBS.Label + "->" + showParentPaths((BaseStation)thisBS.parentStation, cnt);
            }
        }

        public void releaseMobileAgent(SelectableObject Sender)
        {

            //World.LogResults(DateTime.Now + " - Station: " + this.Label + " RELEASED MobileAgent: " + Sender.Label);
            
            for (int i = 0; i < m_agents.Count; i++)
            {
                if (m_agents[i].agent.Label == Sender.Label)
                {
                    m_agents.RemoveAt(i);
                }
            }

        }


        //public void showLine(SelectableObject sender)
        //{

        //    //e.Graphics.DrawLine(SystemPens.Highlight, startPoint, endPoint)

        //    Point startPoint = new Point(sender.Position.X,sender.Position.Y)
        //    Point endPoint = new Point(this.Position.X, this.Position.Y)
        //    bitmap

        //}

    }
}
