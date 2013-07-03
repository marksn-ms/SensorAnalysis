using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;
using System.Drawing;

namespace Sensors
{
    public class PartitionDeployer : HierarchicalDeployer
    {
        public PartitionDeployer(World w)
            : base(w)
        {
        }
        public override string Info()
        {
            return "This deployer finds all BaseStation type inhabitants and makes 2nd level (grandparents of Station objects) a Partition Authority.";
        }
        public override void Deploy()
        {
            base.Deploy();

            // make all 2nd level base stations partition authorities

            // identify all the station objects in the world
            List<BaseStation> lstStation = new List<BaseStation>();
            foreach (SelectableObject o in World.AllObjects)
                if (o.GetType() == typeof(BaseStation))
                    lstStation.Add((BaseStation)o);

            foreach (BaseStation b in lstStation)
            {
                b.LocationScheme = BaseStation.SchemeType.Partition;

                if (b.childStations.Count > 0) // a 2nd level partition authority must have children
                {
                    if (b.childStations[0].GetType() == typeof(BaseStation)) // 2nd level's child stations should be base stations
                    {
                        BaseStation bb = (BaseStation)b.childStations[0];
                        if (bb.childStations.Count > 0) // 2nd level's child should have children
                        {
                            if (bb.childStations[0].GetType() == typeof(Station)) // 2nd level's child's children should be stations
                            {
                                b.PartitionAuthority = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
