using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Interface
{
    /// <summary>
    /// This is the base class for classes that want to monitor and
    /// report status on simulations.  This is based on the idea
    /// that the World class will raise events whenever anything
    /// interesting happens, and classes derived from Watcher will
    /// register for those events and will track, store, and make
    /// the results of these messages available as appropriate based
    /// on what they are watching and how they are to be interpreted.
    /// The base class doesn't really monitor anything, so it's up
    /// to derived classes to do the heavy lifting.
    /// </summary>
    public abstract class Watcher : IDisposable
    {
        private World m_world;
        public World World
        {
            get { return m_world; }
            set { m_world = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Watcher"/> class.  In derived classes, this is where
        /// the object attaches event handlers to the <see cref="World"/> class.
        /// </summary>
        /// <param name="w">The <see cref="World"/>.</param>
        public Watcher(World w)
        {
            World = w;
        }

        /// <summary>
        /// Removes the event handlers.
        /// </summary>
        public abstract void RemoveEventHandlers();

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// In this case, this means detaching all of the events that this class is attached to in the
        /// <see cref="World"/> class.
        /// </summary>
        public void Dispose()
        {
            RemoveEventHandlers();
        }

        #endregion
    }

    /// <summary>
    /// This is the <see cref="TestWatcher"/> class that simply writes a line to debugging
    /// console any time an event occurs.
    /// </summary>
    public class TestWatcher : Watcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestWatcher"/> class.
        /// </summary>
        /// <param name="w">The <see cref="World"/>.</param>
        public TestWatcher(World w)
            : base(w)
        {
            w.MessageSentEvent += new World.MessageSentDelegate(OnMessageSentEvent);
            w.PreTestRunEvent += new World.PreTestRunDelegate(OnPreTestRunEvent);
            w.PostTestRunEvent += new World.PostTestRunDelegate(OnPostTestRunEvent);
            w.PreStepEvent += new World.PreStepDelegate(OnPreStepEvent);
            w.PostStepEvent += new World.PostStepDelegate(OnPostStepEvent);
            w.PreTickEvent += new World.PreTickDelegate(OnPreTickEvent);
            w.PostTickEvent += new World.PostTickDelegate(OnPostTickEvent);
        }

        /// <summary>
        /// Handles the PostTick event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.PostTickEventArgs"/> instance containing the event data.</param>
        private void OnPostTickEvent(object sender, World.PostTickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("****** OnPostTickEvent.");
        }

        /// <summary>
        /// Handles the PreTick event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.PreTickEventArgs"/> instance containing the event data.</param>
        private void OnPreTickEvent(object sender, World.PreTickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("****** OnPreTickEvent.");
        }

        /// <summary>
        /// Handles the PostStep event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.PostStepEventArgs"/> instance containing the event data.</param>
        private void OnPostStepEvent(object sender, World.PostStepEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("**** OnPostStepEvent.");
        }

        /// <summary>
        /// Handles the MessageSentEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.MessageSentEventArgs"/> instance containing the event data.</param>
        void OnMessageSentEvent(object sender, World.MessageSentEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("******** OnMessageSentEvent.");
        }

        /// <summary>
        /// Handles the PreTestRunEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.PreTestRunEventArgs"/> instance containing the event data.</param>
        void OnPreTestRunEvent(object sender, World.PreTestRunEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("** OnPreTestRunEvent.");
        }

        /// <summary>
        /// Handles the PreStepEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.PreStepEventArgs"/> instance containing the event data.</param>
        void OnPreStepEvent(object sender, World.PreStepEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("**** OnPreStepEvent.");
        }

        /// <summary>
        /// Handles the PostTestRunEvent event of the w control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WorldSim.Interface.World.PostTestRunEventArgs"/> instance containing the event data.</param>
        void OnPostTestRunEvent(object sender, World.PostTestRunEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("** OnPostTestRunEvent.");
        }

        /// <summary>
        /// Removes the event handlers.
        /// </summary>
        public override void RemoveEventHandlers()
        {
            World.MessageSentEvent -= this.OnMessageSentEvent;
            World.PreTestRunEvent -= this.OnPreTestRunEvent;
            World.PostTestRunEvent -= this.OnPostTestRunEvent;
            World.PreStepEvent -= this.OnPreStepEvent;
            World.PostStepEvent -= this.OnPostStepEvent;
            World.PreTickEvent -= this.OnPreTickEvent;
            World.PostTickEvent -= this.OnPostTickEvent;
        }
    }
}
