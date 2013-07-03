using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;
using System.Diagnostics;

namespace TokenRing
{
    /// <summary>
    /// This class is the watcher for the objects in the Sensor project.
    /// </summary>
    public class TokenRingMessageWatcher : Watcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWatcher"/> class.
        /// </summary>
        /// <param name="w">The <see cref="World"/>.</param>
        public TokenRingMessageWatcher(World w)
            : base(w)
        {
            w.MessageSentEvent += new World.MessageSentDelegate(OnMessageSentEvent);
        }

        /// <summary>
        /// Handles the MessageSentEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.MessageSentEventArgs"/> instance containing the event data.</param>
        void OnMessageSentEvent(object sender, World.MessageSentEventArgs e)
        {
            // Uncomment the next line if you don't want to see the ping messages.
            if (e.Message.GetType().ToString() != "TokenRing.PingMessage" && e.Message.GetType().ToString() != "TokenRing.PingAckMessage")
            {
                //Trace.WriteLine(e.From.Label + "->" + e.To.Label + "," + e.Message.ToString());
                World.LogResults(e.From.Label + "->" + e.To.Label + "," + e.Message.ToString());
            }
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
