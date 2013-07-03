using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;
using System.Drawing;

namespace Sensors
{
    public class ForwardingDeployer : HierarchicalDeployer
    {
        public ForwardingDeployer(World w)
            : base(w)
        {
        }
        public override string Info()
        {
            return "This deployer works the same as Heirarchical deployer except that it sets the Scheme Type to Forwarding.";
        }
        public override void Deploy()
        {
            base.Deploy();

            // identify all the station objects in the world
            foreach (SelectableObject o in World.AllObjects)
                if (o.GetType() == typeof(BaseStation))
                    ((BaseStation)o).LocationScheme = BaseStation.SchemeType.Forwarding;
        }
    }
}
