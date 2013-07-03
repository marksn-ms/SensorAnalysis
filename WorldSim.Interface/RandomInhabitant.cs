using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WorldSim.Interface;

namespace WorldSim.Interface
{
    [Serializable]
    public class RandomInhabitant : Inhabitant
    {
        public RandomInhabitant()
            : base()
        {
            BackgroundColor = System.Drawing.Color.Aquamarine;
        }
        public override void Tick()
        {
            base.Tick();
            if (Parent != null)
                Action = (World.Actions)Parent.World.Random.Next((int)World.Actions.Max + 1); // returns 0-4
        }
        public override void Draw(Graphics g, Rectangle rectViewport, float scale)
        {
            Brush b = Selected || (Parent != null && Parent.Selected) ? m_brushSelected : m_brushBack;
            DrawTriangle(g, rectViewport, Position, new Size((int)Size.Width, (int)Size.Height), scale, b, m_penFore);
        }
        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public override string Info() { return "An inhabitant that picks a random action continuously."; }
    }
}
