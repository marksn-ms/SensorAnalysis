using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;

namespace MobileSensorAgents
{
    /// <summary>
    /// This is the <see cref="Watcher"/> class for the MobileSensor project.
    /// </summary>
    public class MobileSensorWatcher : Watcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSensorWatcher"/> class.
        /// </summary>
        /// <param name="w">The <see cref="World"/>.</param>
        public MobileSensorWatcher(World world)
            : base(world)
        {
            if (world != null)
                world.MessageSentEvent += new World.MessageSentDelegate(OnMessageSentEvent);
        }

        /// <summary>
        /// Handles the MessageSentEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.MessageSentEventArgs"/> instance containing the event data.</param>
        void OnMessageSentEvent(object sender, World.MessageSentEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the event handlers.
        /// </summary>
        public override void RemoveEventHandlers()
        {
            World.MessageSentEvent -= this.OnMessageSentEvent;
        }
    }

}
