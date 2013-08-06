#define WORLD_NO_INERTIA

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WorldSim.Interface;

namespace MobileSensorAgents
{
    [Serializable]
    public class ModalInhabitant : Inhabitant
    {
        private bool m_bExplore; // explore/exploit mode depending on the number of neighbors
        private int m_nTicksExplore; // number of ticks to continue in chosen direction
        private World.Actions m_actLast; // last action chosen

        public ModalInhabitant()
            : base()
        {
            BackgroundColor = Color.CadetBlue;
            m_pts = new PointF[(int)World.Actions.Max - (int)World.Actions.Min + 1];
            for (int i = 0; i < (int)World.Actions.Max - (int)World.Actions.Min + 1; i++)
            {
                m_pts[i] = new PointF((float)(4.0f * Math.Sin(i * 22.5f * Math.PI / 180)), (float)(4.0f * Math.Cos(i * 22.5f * Math.PI / 180)));
            }

            // start out exploring
            m_bExplore = true;
            m_nTicksExplore = 0;
            m_actLast = World.Actions.actStay;
        }

        private PointF[] m_pts;
        public override void Tick()
        {
            base.Tick();
            
            //double dDiff = 0.0d;
            //double dRadiansGoal = Math.Atan2(this.Position.Y - this.Goal.Center.Y, this.Position.X - this.Goal.Center.X);
            //double dRadians = Math.Atan2(-Velocity.Y, -Velocity.X);
            //dDiff = dRadiansGoal - dRadians;
            //if (dDiff > Math.PI - dDiff)
            //    dDiff = -Math.PI + dDiff;
            //double dSpeed = World.Distance(new PointF(0, 0), Velocity);
            //double dDistance = World.Distance(Center, Goal.Center);
            
#if WORLD_NO_INERTIA
            // TODO: reimplement intentionally smart agent when there is no inertia
            
            // Get a list of all the inhabitants and incidents in the neighborhood
            List<Inhabitant> iNeighbors = new List<Inhabitant>();
            List<Incident> incNeighbors = new List<Incident>();
            foreach (SelectableObject o in Parent.NearbyObjects(typeof(Inhabitant)))
                iNeighbors.Add((Inhabitant)o);
            foreach (SelectableObject o in Parent.NearbyObjects(typeof(Incident)))
                incNeighbors.Add((Incident)o);

            m_bExplore = (iNeighbors.Count-1 + incNeighbors.Count == 0) || (iNeighbors.Count > 6);

            if (m_bExplore)
            {
                if (m_nTicksExplore-- > 0)
                    Action = m_actLast;
                else
                {
                    Action = (World.Actions)World.Random.Next((int)World.Actions.Min, (int)World.Actions.Max); // when exploring, don't pick 'stay'
                    m_actLast = Action;
                    m_nTicksExplore = World.Random.Next((int)(SensorRange * .5), (int)(SensorRange * 1.5));
                }
            }
            else
            {
#if WORLD_Modal_GREEDY
                double dScoreBestMove = World.Reward(this, iNeighbors);
                World.Actions aBestMove = World.Actions.actStay;
                PointF ptOld = Position;
                for (int m = 0; m < m_pts.GetUpperBound(0); m++)
                {
                    PointF ptMove = new PointF((int)m_pts[m].X, (int)m_pts[m].Y);
                    Position = new PointF(Position.X + ptMove.X, Position.Y + ptMove.Y);
                    //double dScoreNew = World.ScoreForP_s(this, iNeighbors) + World.ScoreForP_e(this, incNeighbors);
                    double dScoreNew = World.Reward(this, iNeighbors);
                    if (dScoreNew > dScoreBestMove)
                    {
                        aBestMove = World.Actions.Min + m;
                        dScoreBestMove = dScoreNew;
                    }
                }
                Position = ptOld;
                Action = aBestMove;

#else
                // use roulette wheel to pick move
                RouletteWheel<World.Actions> rw = new RouletteWheel<World.Actions>(Parent.World.Random);
                rw.Add(0.1d + Parent.World.Reward(this, iNeighbors, null), World.Actions.actStay);
                PointF ptOld = Position;
                for (int m = 0; m < m_pts.GetUpperBound(0); m++)
                {
                    PointF ptMove = new PointF((int)m_pts[m].X, (int)m_pts[m].Y);
                    Position = new PointF(Position.X + ptMove.X, Position.Y + ptMove.Y);
                    rw.Add(0.1d + Parent.World.Reward(this, iNeighbors, null), World.Actions.Min + m);
                }
                Position = ptOld;
                Action = rw.Choice;
#endif
            }
#else
            int nRandom = World.Random.Next(3);
            if (Math.Abs(dDiff) < 0.1)
                Action = (nRandom == 0 ? World.Actions.actContinue : World.Actions.actSpeedUp);
            else if (dDiff > 0 && dDiff < Math.PI / 4)
                Action = (nRandom != 0 ? World.Actions.actTurnLeft : World.Actions.actSlowDown);
            else if (dDiff < 0 && dDiff > -Math.PI / 4)
                Action = (nRandom != 0 ? World.Actions.actTurnRight : World.Actions.actSlowDown);
            else if (dDiff > 0)
                Action = World.Actions.actTurnLeft;
            else
                Action = World.Actions.actTurnRight;
#endif
        }

        public override void Draw(Graphics g, Rectangle rectViewport, float scale)
        {
            Brush b = (Selected || (Parent != null && Parent.Selected)) ? m_brushSelected : m_brushBack;
            DrawTriangle(g, rectViewport, Position, new Size((int)Size.Width, (int)Size.Height), scale, b, m_penFore);
        }
    }
}
