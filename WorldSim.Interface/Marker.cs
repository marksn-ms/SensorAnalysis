using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WorldSim.Interface
{
    // this is the base class for markers that can be left/dropped
    // by agents.
    [Serializable]
    public class Marker : SelectableObject
    {
        private Guid m_markerID;
        public Guid ID { get { return m_markerID; } }
        
        public Marker()
        {
            m_markerID = Guid.NewGuid();
            Expires = 100;
        }

        public override void Tick()
        {
            Expires--;
        }

        public override void Draw(Graphics g, Rectangle rectViewport, float scale)
        {
            Point ptDraw1 = new Point((int)(Position.X * scale), (int)(Position.Y * scale));
            Point ptDraw2 = new Point(ptDraw1.X, ptDraw1.Y);
            ptDraw1.Offset(-rectViewport.X, -rectViewport.Y);
            ptDraw2.Offset(-rectViewport.X, -rectViewport.Y+1);
            g.DrawLine(Pens.LightGray, ptDraw1, ptDraw2);
        }
    }
}
