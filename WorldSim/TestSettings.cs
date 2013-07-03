using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WorldSim
{
    [Serializable]
    public class TestSettings
    {
        // environment settings
        public Size Tiles;                  // number of tiles
        public Size TileSize;               // size of individual tiles
        public string TileShape;            // Rectangle or Hexagon

        // world setup
        public ICollection<string> Instructions;       // instructions for preparing world for test run

        // agent settings
        public int[] Population;            // number of agents of type m_Agent
        public string[] Agent;              // type of agent, like "XCSAgent"
        public int[] SensorRange;           // how far an agent can "see"
        public bool PointDeployment;        // true=put all inhabitants on the same point
        
        // deployer settings
        public string[] Deployer;           // which deployers to user

        // incident settings
        public int Incident;                         // an "incident" or event
        public int IncidentMaxTurnsBeforeMove;       // how often the incidents move locations
        public bool NonUniformIncidentDistribution;  // true=establish distribution and roulette wheel out the incident placement

        // simulation settings
        public int Duration;                // how many steps to run the test
        public int LogFrequency;            // how many steps to go before logging results
        public int Repeats;                 // how many times to repeat the test
        public double RewardScaleP_s;
        public double RewardScaleP_e;
        public double RewardScaleP_n;

        public TestSettings()
        {
            Tiles = new Size(10, 10);
            TileShape = "Rectangle";
            TileSize = new Size(100, 100);

            Population = new int[] { 1 };
            Agent = new string[0];
            SensorRange = new int[] { 100 };
            Instructions = new List<string>();
            PointDeployment = false;
            RewardScaleP_s = 1;
            RewardScaleP_e = 1;
            RewardScaleP_n = 1;

            Incident = 0;
            IncidentMaxTurnsBeforeMove = 0;
            NonUniformIncidentDistribution = true;

            Repeats = 1;
            Duration = 100000;
            LogFrequency = 100;
        }
    } 
}
