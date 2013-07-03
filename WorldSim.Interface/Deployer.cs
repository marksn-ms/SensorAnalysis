using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Interface
{
    /// <summary>
    /// This class is used to define methods of deployment of objects
    /// within the environment.
    /// </summary>
    public abstract class Deployer
    {
        private World m_world;
        public World World { get { return m_world; } }

        /// <summary>
        /// Constructor for the deployer object.
        /// </summary>
        /// <param name="w"></param>
        public Deployer(World w)
        {
            m_world = w;
        }

        /// <summary>
        /// This method is to be implemented by anyone that wishes to 
        /// do custom deployment.
        /// </summary>
        public abstract void Deploy();

        /// <summary>
        /// This method is used to return a string describing the deployer
        /// which may be of use to the UI to explain what a deployer does.
        /// </summary>
        /// <returns>End-user description of the deployer.</returns>
        public abstract string Info();
        
    }
}
