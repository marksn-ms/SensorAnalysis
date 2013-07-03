using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using WorldSim.Interface;

namespace Sensors
{
    class BaseStation : SelectableObject
    {

        /// <summary>
        /// This is the list of child stations of which this station is
        /// the parent.
        /// </summary>
        /// 
        public List<SelectableObject> childStations;

        public struct mobileAgentDBListItem
        {
            public SelectableObject mobileAgent;
            public SelectableObject childPath;
            public SelectableObject location;
        }
        public List<mobileAgentDBListItem> mobileAgentDB;

        /// <summary>
        /// This is the parent station of this station.
        /// </summary>
        private SelectableObject m_parentStation;
        public string parentStationLabel;
        public SelectableObject parentStation
        {
            get 
            { 
                return m_parentStation; 
            }
            set 
            { 
                m_parentStation = value;
                parentStationLabel = m_parentStation.Label;
            }
        }
        

        /// <summary>
        /// The below enumeration and properties are to help 
        /// determine what location scheme we are running
        /// </summary>
        public enum SchemeType { Pointer, Database, Forwarding, Partition };

        private SchemeType m_locationScheme;
        public SchemeType LocationScheme
        {
            get { return m_locationScheme; }
            set { m_locationScheme = value; }
        }

        /// <summary>
        /// The property below determines if this base station is
        /// the partition authority or not.  This is only useful if
        /// we are running the partition location scheme.
        /// </summary>

        private Boolean m_partitionAuth = false;
        public Boolean PartitionAuthority
        {
            get { return m_partitionAuth; }
            set { m_partitionAuth = value; }
        }

        /// <summary>
        /// Default Constructor.  Initialize data members.
        /// </summary>
        public BaseStation()
        {
            childStations = new List<SelectableObject>();
            mobileAgentDB = new List<mobileAgentDBListItem>();
            parentStation = this;
            BackgroundColor = System.Drawing.Color.LightBlue;
            m_locationScheme = SchemeType.Database;
            m_partitionAuth = false;
        }

        /// <summary>
        /// Overloaded constructor for use in new deployer methods
        /// </summary>
        /// <param name="LocationSchemeType"></param>
        /// <param name="PartitionAuthority"></param>
        public BaseStation(SchemeType LocationSchemeType, Boolean PartitionAuthority)
        {
            childStations = new List<SelectableObject>();
            mobileAgentDB = new List<mobileAgentDBListItem>();
            parentStation = this;
            BackgroundColor = System.Drawing.Color.LightBlue;
            m_locationScheme = LocationSchemeType;
            m_partitionAuth = PartitionAuthority;
        }
        /// <summary>
        /// Draw rectangle to represent baseStation
        /// </summary>
        /// <param name="bitmapGraphics"></param>
        /// <param name="rectViewport"></param>
        /// <param name="fScale"></param>
        public override void Draw(System.Drawing.Graphics bitmapGraphics, System.Drawing.Rectangle rectViewport, float fScale)
        {
            if (bitmapGraphics == null)
                return;

            Size = new System.Drawing.Size(5, 5);
            Brush b = Selected || (Parent != null && Parent.Selected) ? m_brushSelected : m_brushBack;

            Brush aSolidBrush = new SolidBrush(Color.LightGray);
            Pen aSolidPen = new Pen(aSolidBrush);
            aSolidPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            foreach (SelectableObject o in childStations)
            {
                bitmapGraphics.DrawLine(aSolidPen, (this.Position.X - rectViewport.X) * fScale, (this.Position.Y - rectViewport.Y) * fScale, (o.Position.X - rectViewport.X) * fScale, (o.Position.Y - rectViewport.Y) * fScale);
            }
            aSolidPen.Dispose();
            aSolidBrush.Dispose();

            DrawRectangle(bitmapGraphics, rectViewport, Position, new Size((int)Size.Width, (int)Size.Height), fScale, b, m_penFore);
        }

        /// <summary>
        /// Handle the tick of the clock for the Station object.
        /// </summary>
        public override void Tick()
        {
            base.Tick();
            processInbox();
        }

        private void processInbox()
        {
            foreach (Message m in Inbox)
            {
                
                switch (m.GetType().ToString())
                {
                    case "Sensors.updateMobileAgentRecordRoutedMessage":
                        updateMobileAgentRecord((UpdateMobileAgentRecordRoutedMessage)m);
                        break;
                    case "Sensors.releaseMobileAgentRecordRoutedMessage":
                        break;
                    default:
                        break;
                }
            }
        }

        private void updateMobileAgentRecord(UpdateMobileAgentRecordRoutedMessage m)
        {
            // check to see if this base station has existing record.
            int mobileAgentInList = doesMobileAgentRecordExist(m.MobileAgent);


            // if it has a record, update record, send de-register old branch
            if (mobileAgentInList >= 0)
            {
                Outbox.Add(new releaseMobileAgentRecordRoutedMessage(m, this, mobileAgentDB[mobileAgentInList].childPath, m.MobileAgent));
                
                // update existing record
                mobileAgentDBListItem tempMA;
                tempMA.location = m.newTower;
                tempMA.mobileAgent = mobileAgentDB[mobileAgentInList].mobileAgent;
                tempMA.childPath = m.Sender;
                mobileAgentDB[mobileAgentInList] = tempMA;

                // send remove to old branch


            }
            else
            {
                // add new record
                mobileAgentDBListItem tempMA = new mobileAgentDBListItem();
                tempMA.location = m.newTower;
                tempMA.mobileAgent = m.MobileAgent;
                tempMA.childPath = m.Sender;
                mobileAgentDB.Add(tempMA);
            }

            // send copy of registration up to parent if parent
            // exists
            if (!(this.parentStation.Label == this.Label))
            {
                Outbox.Add(new UpdateMobileAgentRecordRoutedMessage(m, this, this.parentStation, m.MobileAgent, m.newTower));

                //World.LogResults("BS:" + this.Label + " sent BS:" + this.parentStation.Label + " update for MA:" + m.mobileAgent.Label);
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

        private int doesMobileAgentRecordExist(SelectableObject mobileAgent)
        {
            int tmpO = -1;
            for (int i = 0; i < mobileAgentDB.Count; i++)
            {
                if (mobileAgentDB[i].mobileAgent.Label == mobileAgent.Label)
                {
                    tmpO = i;
                }
            }

            return tmpO;
        }
    }
}
