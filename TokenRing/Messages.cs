using System;
using System.Collections.Generic;
using System.Text;
using WorldSim.Interface;

namespace TokenRing
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
        public SelectableObject recipient
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
            return base.ToString() + "," + recipient.Label;
        }
    }

    /// <summary>
    /// This class is what is sent from a mobile agent to another mobile agent.
    /// </summary>
    public class TextMessage : Message
    {
        private string m_strText;
        public string Text { get { return m_strText; } set { m_strText = value; } }

        private string m_strRecipientLabel;
        public string RecipentLabel { get { return m_strRecipientLabel; } set { m_strRecipientLabel = value; } }

        public TextMessage(string strText, SelectableObject sender, string strRecipientLabel)
            : base(sender)
        {
            m_strText = strText;
            m_strRecipientLabel = strRecipientLabel;
        }
    }

    /// <summary>
    /// This is the message that is routed tower to tower to deliver a text message.
    /// </summary>
    public class TextRoutedMessage : RoutedMessage
    {
        private string m_strText;
        public string Text { get { return m_strText; } set { m_strText = value; } }

        private string m_strRecipientLabel;
        public string RecipentLabel { get { return m_strRecipientLabel; } set { m_strRecipientLabel = value; } }

        public TextRoutedMessage(string strText, SelectableObject sender, SelectableObject recip, string strRecipientLabel)
            : base(sender, recip)
        {
            m_strText = strText;
            m_strRecipientLabel = strRecipientLabel;
        }
    }

    /// <summary>
    /// This message is passed from tower to agent to pass the token, and
    /// then from agent back to tower to give it back.
    /// </summary>
    public class TheTokenIsYoursMessage : Message
    {
        private Guid m_token;
        public Guid Token
        {
            get { return m_token; }
            set { m_token = value; }
        }

        private SelectableObject m_recip;
        public SelectableObject Recipient { get { return m_recip; } set { m_recip = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheTokenIsYoursMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="token">The token.</param>
        /// <param name="recip">The recipient.</param>
        public TheTokenIsYoursMessage(SelectableObject sender, Guid token, SelectableObject recip)
            : base(sender)
        {
            m_token = token;
            m_recip = recip;
        }
    }

    /// <summary>
    /// This message is passed from tower to agent when an invalid token was
    /// specified in a message.
    /// </summary>
    public class InvalidTokenMessage : Message
    {
        private SelectableObject m_recip;
        public SelectableObject Recipient { get { return m_recip; } set { m_recip = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheTokenIsYoursMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recip">The recipient.</param>
        public InvalidTokenMessage(SelectableObject sender, SelectableObject recip)
            : base(sender)
        {
            m_recip = recip;
        }
    }

    /// <summary>
    /// This message is passed from tower to tower to pass the token.
    /// </summary>
    public class TheTokenIsYoursRoutedMessage : RoutedMessage
    {
        private Guid m_token;
        public Guid Token
        {
            get { return m_token; }
            set { m_token = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheTokenIsYoursRoutedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recip">The recipient.</param>
        /// <param name="token">The token.</param>
        public TheTokenIsYoursRoutedMessage(SelectableObject sender, SelectableObject recip, Guid token)
            : base(sender, recip)
        {
            Token = token;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheTokenIsYoursRoutedMessage"/> class.
        /// </summary>
        /// <param name="m">The original message when this message is being passed onward.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="recip">The recipient.</param>
        /// <param name="token">The token.</param>
        public TheTokenIsYoursRoutedMessage(Message m, SelectableObject sender, SelectableObject recip, Guid token)
            : base(m, sender, recip)
        {
            Token = token;
        }
    }
}
