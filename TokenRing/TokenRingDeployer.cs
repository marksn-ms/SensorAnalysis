using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;
using System.Drawing;

namespace TokenRing
{
    public class TokenRingDeployer : Deployer
    {
        public TokenRingDeployer(World w)
            : base(w)
        {
        }
        public override string Info()
        {
            return "This deployer works against world inhabitants of type Station, and hooks up each Station to a next Station until finally the last Station is connected to the first again, such that the connections form a ring.";
        }
        public override void Deploy()
        {
            // identify all the station objects in the world
            List<Station> lstStations = new List<Station>();
            foreach (SelectableObject o in World.AllObjects)
                if (o.GetType() == typeof(Station))
                    lstStations.Add((Station)o);

            if (lstStations.Count > 0)
            {
                // hook up each station in the list to the next station, and the last
                // station in the list to the first in the list
                for (int i = 0; i < lstStations.Count - 1; i++)
                    lstStations[i].Next = lstStations[i + 1];
                lstStations[lstStations.Count - 1].Next = lstStations[0];

                // give the token initially to the first station in the list
                lstStations[0].Inbox.Add(new TheTokenIsYoursRoutedMessage(lstStations[0], lstStations[0], Guid.NewGuid()));
            }
        }
    }
}
