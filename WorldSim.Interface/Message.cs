using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WorldSim.Interface
{
    // this is the base class for messages that can be sent
    // by agents.
    [Serializable]
    public abstract class Message
    {
        private Guid m_messageID;
        public Guid ID { get { return m_messageID; } }
        private Type m_originalMessageType;
        public Type OriginalMessageType { get { return m_originalMessageType; } }
        private SelectableObject m_sender;
        public SelectableObject Sender { get { return m_sender; } }
        public Message(SelectableObject sender)
        {
            m_messageID = Guid.NewGuid();
            m_originalMessageType = this.GetType();
            m_sender = sender;
        }
        public Message(Message m, SelectableObject sender)
        {
            m_messageID = m.m_messageID;
            m_originalMessageType = m.m_originalMessageType;
            m_sender = sender;
        }
        public override string ToString()
        {
            return "Message(" + ID.ToString() + ")," + OriginalMessageType.ToString() + "," + GetType().ToString() + "," + Sender.Label;
        }
    }

    // this is the base class for messages that can be sent
    // by agents.
    [Serializable]
    public abstract class RoutedMessage : Message
    {
        private SelectableObject m_recip;
        public SelectableObject Recipient { get { return m_recip; } }
        public RoutedMessage(SelectableObject sender, SelectableObject recip)
            : base(sender)
        {
            m_recip = recip;
        }
        public RoutedMessage(Message m, SelectableObject sender, SelectableObject recip)
            : base(m, sender)
        {
            m_recip = recip;
        }
        public override string ToString()
        {
            return base.ToString() + "," + Recipient.Label;
        }
    }
}
