using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;
using System.Drawing;

namespace Sensors
{
    public class HierarchicalDeployer : Deployer
    {
        public HierarchicalDeployer(World w)
            : base(w)
        {
        }
        public override string Info()
        {
            return "This deployer works against world inhabitants of type Station, creates new BaseStation type inhabitants such that a tree is established.  For every 2 Station objects, a new BaseStation is created as their parent, and for every 2 BaseStation objects, another is created, and so on, until there is one master BaseStation at the head of the tree.";
        }
        public override void Deploy()
        {
            // identify all the station objects in the world
            List<Station> lstStation = new List<Station>();
            foreach (SelectableObject o in World.AllObjects)
                if (o.GetType() == typeof(Station))
                    lstStation.Add((Station)o);

            // create enough base station objects to construct a tree
            List<BaseStation> bases = new List<BaseStation>();
            for (int i = 0; i < lstStation.Count; i += 2)
            {
                BaseStation b = new BaseStation();
                b.childStations.Add(lstStation[i]);
                lstStation[i].parentStation = b;
                if (i < lstStation.Count - 1)
                {
                    b.Position = new PointF((lstStation[i].Position.X + lstStation[i + 1].Position.X) / 2 - 10, (lstStation[i].Position.Y + lstStation[i + 1].Position.Y) / 2 - 10);
                    b.childStations.Add(lstStation[i + 1]);
                    lstStation[i + 1].parentStation = b;
                }
                else
                    b.Position = new PointF(lstStation[i].Position.X - 10, lstStation[i].Position.Y - 10);

                bases.Add(b);
                World.Add(b);
            }

            int nAdded = bases.Count;
            do
            {
                int nStart = bases.Count - nAdded;
                int nStop = bases.Count;
                nAdded = 0;
                for (int i = nStart; i < nStop; i += 2)
                {
                    BaseStation b = new BaseStation();
                    b.childStations.Add(bases[i]);
                    bases[i].parentStation = b;
                    if (i < bases.Count - 1)
                    {
                        b.Position = new PointF((bases[i].Position.X + bases[i + 1].Position.X) / 2 - 10, (bases[i].Position.Y + bases[i + 1].Position.Y) / 2 - 10);
                        b.childStations.Add(bases[i + 1]);
                        bases[i+1].parentStation = b;
                    }
                    else
                        b.Position = new PointF(bases[i].Position.X - 10, bases[i].Position.Y - 10);
                    bases.Add(b);
                    World.Add(b);
                    nAdded++;
                }
            } while (nAdded > 1);


            //  I did this below to make sure the parentStation fields were getting set correctly....
            //// identify all the base station objects in the world
            //List<BaseStation> lstBaseStation = new List<BaseStation>();
            //foreach (SelectableObject o in World.AllObjects)
            //    if (o.GetType() == typeof(BaseStation))
            //        lstBaseStation.Add((BaseStation)o);

            //foreach (BaseStation bs in lstBaseStation)
            //{
            //    for (int i = 0; i < bs.childStations.Count; i++)
            //    {
            //        if (bs.childStations[i].GetType().ToString() == "Sensors.BaseStation")
            //        {
            //            BaseStation tmpBS = (BaseStation)bs.childStations[i];
            //            tmpBS.parentStation = bs;
            //        }
                    
            //    }
            //}

        }
    }
}
