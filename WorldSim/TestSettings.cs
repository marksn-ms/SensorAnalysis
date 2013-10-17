using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace WorldSim
{
    [Serializable]
    public class Variable
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }

    [Serializable]
    public class TestSettings
    {
        public String Title { get; set; }

        // environment settings
        public Size Tiles { get; set; }                  // number of tiles
        public Size TileSize { get; set; }               // size of individual tiles
        public string TileShape { get; set; }            // Rectangle or Hexagon

        public List<string> Instructions { get; set; }   // instructions for preparing world for test run

        public List<Variable> Variables { get; set; }    // replacement variables

        // TODO: get rid of these in favor of instructions
        public int Incident { get; set; }                         // an "incident" or event
        public int IncidentMaxTurnsBeforeMove { get; set; }       // how often the incidents move locations
        public bool NonUniformIncidentDistribution { get; set; }  // true=establish distribution and roulette wheel out the incident placement

        // simulation settings
        public int Duration { get; set; }                // how many steps to run the test
        public int Repeats { get; set; }                 // how many times to repeat the test

        public TestSettings()
        {
            Tiles = new Size(10, 10);
            TileShape = "Rectangle";
            TileSize = new Size(100, 100);

            Instructions = new List<string>();
            Variables = new List<Variable>();

            Incident = 0;
            IncidentMaxTurnsBeforeMove = 0;
            NonUniformIncidentDistribution = true;

            Repeats = 100;
            Duration = 1000;
        }
    } 
}
