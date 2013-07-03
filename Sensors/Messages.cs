using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;

namespace Sensors
{
    /// <summary>
    /// This message is sent to anyone in the area to see if they will answer.
    /// </summary>
    public class PingMessage : Message
    {
        public PingMessage(SelectableObject sender)
            : base(sender)
        {
        }
    }

    /// <summary>
    /// Acknowledgement message for the PingMessage class.
    /// </summary>
    public class PingAckMessage : Message
    {
        public PingAckMessage(SelectableObject sender)
            : base(sender)
        {
        }
    }

    /// <summary>
    /// This is used by the mobile agent to register with a new tower
    /// </summary>

    public class RegisterMobileAgentMessage : Message
    {
        private SelectableObject m_recipient;
        public SelectableObject Recipient
        {
            get { return m_recipient; }
        }

        public RegisterMobileAgentMessage(SelectableObject sender, SelectableObject recip)
            : base(sender)
        {
            m_recipient = recip;
        }

        public RegisterMobileAgentMessage(Message m, SelectableObject sender, SelectableObject recip)
            : base(m, sender)
        {
            m_recipient = recip;
        }

        public override string ToString()
        {
            return base.ToString() + "," + Recipient.Label;
        }
    }

    public class UpdateMobileAgentRecordRoutedMessage : RoutedMessage
    {
        private SelectableObject m_mobileAgent;
        public SelectableObject MobileAgent
        {
            get { return m_mobileAgent; }
        }

        private SelectableObject m_newTower;
        public SelectableObject newTower
        {
            get { return m_newTower; }
        }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="recip"></param>
        /// <param name="thisMobileAgent"></param>
        /// <param name="thisNewStation"></param>
        public UpdateMobileAgentRecordRoutedMessage(SelectableObject sender, SelectableObject recip, SelectableObject thisMobileAgent, SelectableObject thisNewStation)
            : base(sender, recip)
        {
            m_mobileAgent = thisMobileAgent;
            m_newTower = thisNewStation;
        }
        /// <summary>
        /// override constructor
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sender"></param>
        /// <param name="recip"></param>
        /// <param name="thisMobileAgent"></param>
        /// <param name="thisNewStation"></param>
        public UpdateMobileAgentRecordRoutedMessage(Message m, SelectableObject sender, SelectableObject recip, SelectableObject thisMobileAgent, SelectableObject thisNewStation)
            : base(m, sender, recip)
        {
            m_mobileAgent = thisMobileAgent;
            m_newTower = thisNewStation;
        }
    }

    /// <summary>
    /// </summary>
    public class releaseMobileAgentRecordRoutedMessage : RoutedMessage
    {
        private SelectableObject m_mobileAgent;
        public SelectableObject mobileAgent
        {
            get { return m_mobileAgent; }
        }
        public releaseMobileAgentRecordRoutedMessage(SelectableObject sender, SelectableObject recip, SelectableObject mobileAgent)
            : base(sender, recip)
        {
            m_mobileAgent = mobileAgent;
        }

        public releaseMobileAgentRecordRoutedMessage(Message m, SelectableObject sender, SelectableObject recip, SelectableObject mobileAgent)
            : base(m, sender, recip)
        {
            m_mobileAgent = mobileAgent;
        }
    }
}
