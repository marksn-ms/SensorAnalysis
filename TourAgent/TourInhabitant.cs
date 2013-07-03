using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using WorldSim.Interface;

namespace TourAgent
{
    /// <summary>
    /// This agent chooses its initial starting point as "home".  It then creates
    /// a route that will allow it to traverse all points within a given area
    /// </summary>
    public class TourInhabitant : Inhabitant
    {
        enum Direction
        {
            Southeast = 30,
            South = 90,
            Southwest = 150,
            Northwest = 210,
            North = 270,
            Northeast = 330,
        }
        List<PointF> m_ptTour;
        private int m_nPointNext;
        private PointF[] m_pts;

        public TourInhabitant()
        {
            m_pts = new PointF[(int)World.Actions.Max - (int)World.Actions.Min + 1];
            for (int i = 0; i < (int)World.Actions.Max - (int)World.Actions.Min + 1; i++)
            {
                m_pts[i] = new PointF((float)(4.0f * Math.Sin(i * 22.5f * Math.PI / 180)), (float)(4.0f * Math.Cos(i * 22.5f * Math.PI / 180)));
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (m_ptTour == null)
                GetPointList();

            // pick the action that takes us most toward the next point
            double dDistanceBest = double.PositiveInfinity;
            int nActionBest = -1;
            for (int m = 0; m < m_pts.GetUpperBound(0); m++)
            {
                PointF ptMove = new PointF((int)m_pts[m].X, (int)m_pts[m].Y);
                PointF ptHypothetical = new PointF(Position.X + ptMove.X, Position.Y + ptMove.Y);
                double dDistanceToNextPoint = World.Distance(ptHypothetical, m_ptTour[m_nPointNext]);
                if (dDistanceToNextPoint < dDistanceBest)
                {
                    dDistanceBest = dDistanceToNextPoint;
                    nActionBest = m;
                }
            }
            if (dDistanceBest < 2)
                m_nPointNext = (m_nPointNext + 1) % m_ptTour.Count;
            Action = (World.Actions)nActionBest;
        }

        private void GetPointList()
        {
            m_ptTour = new List<PointF>();

            int r = 2; // hard-coded number of circles
            int n = 0;
            PointF pt = Position;
            while (n < r)
            {
                if (n == 0)
                    // enter next ring
                    m_ptTour.Add(pt = MovePoint(pt, Direction.Northwest));
                else
                    // enter next ring
                    m_ptTour.Add(pt = MovePoint(pt, Direction.North));

                n = n + 1;
                if (n % 2 == 0)
                    pt = go_cw(pt, n, r);
                else
                    pt = go_ccw(pt, n, r);
            }
            for (int i = 0; i < r; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.South));
        }

        private PointF MovePoint(PointF pt, double fDistance, double theta)
        {
            // update position
            return new PointF((Parent.World.Width + pt.X + (int)(fDistance * Math.Cos(theta))) % Parent.World.Width,
                (Parent.World.Height + pt.Y + (int)(Math.Round(fDistance * Math.Sin(theta)))) % Parent.World.Height);
        }

        private PointF MovePoint(PointF pt, Direction direction)
        {
            // from the current point, move in the direction specified by direction
            return MovePoint(pt, SensorRange * 2, (double)direction * Math.PI / 180);            
        }

        public PointF go_cw(PointF pt, int n, int r)
        {
            //go SW, r-1 times
            for (int i = 0; i < n - 1; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Southeast));
            //go S, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.South));
            //go SE, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Southwest));
            //go NE, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Northwest));
            //go N, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.North));
            //go NW, r-1 times
            for (int i = 0; i < n - 1; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Northeast));
            if (n == r)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Northeast));
            return pt;
        }
        public PointF go_ccw(PointF pt, int n, int r)
        {
            //go SE, r-1 times
            for (int i = 0; i < n - 1; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Southwest));
            //go S, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.South));
            //go SW, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Southeast));
            //go NW, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Northeast));
            //go N, r times
            for (int i = 0; i < n; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.North));
            //go NE, r-1 times
            for (int i = 0; i < n - 1; i++)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Northwest));
            if (n == r)
                m_ptTour.Add(pt = MovePoint(pt, Direction.Northwest));
            return pt;
        }
    }
}
