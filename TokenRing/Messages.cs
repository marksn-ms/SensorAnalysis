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
        public SelectableObject Recipient { get; private set; }

        public RegisterMobileAgentMessage(SelectableObject sender, SelectableObject recip)
            : base(sender)
        {
            Recipient = recip;
        }

        public RegisterMobileAgentMessage(Message m, SelectableObject sender, SelectableObject recip)
            : base(m, sender)
        {
            Recipient = recip;
        }

        public override string ToString()
        {
            return base.ToString() + "," + Recipient.Label;
        }
    }

    /// <summary>
    /// This class is what is sent from a mobile agent to another mobile agent.
    /// </summary>
    public class TextMessage : Message
    {
        public string Text { get; set; }
        public string RecipientLabel { get; set; }

        public TextMessage(string strText, SelectableObject sender, string strRecipientLabel)
            : base(sender)
        {
            Text = strText;
            RecipientLabel = strRecipientLabel;
        }
    }

    /// <summary>
    /// This is the message that is routed tower to tower to deliver a text message.
    /// </summary>
    public class TextRoutedMessage : RoutedMessage
    {
        public string Text { get; set; }
        public string RecipientLabel { get; set; }

        public TextRoutedMessage(string strText, SelectableObject sender, SelectableObject recip, string strRecipientLabel)
            : base(sender, recip)
        {
            Text = strText;
            RecipientLabel = strRecipientLabel;
        }
    }

    /// <summary>
    /// This message is passed from tower to agent to pass the token, and
    /// then from agent back to tower to give it back.
    /// </summary>
    public class TheTokenIsYoursMessage : Message
    {
        public Guid Token { get; set; }
        public SelectableObject Recipient { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheTokenIsYoursMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="token">The token.</param>
        /// <param name="recip">The recipient.</param>
        public TheTokenIsYoursMessage(SelectableObject sender, Guid token, SelectableObject recip)
            : base(sender)
        {
            Token = token;
            Recipient = recip;
        }
    }

    /// <summary>
    /// This message is passed from tower to agent when an invalid token was
    /// specified in a message.
    /// </summary>
    public class InvalidTokenMessage : Message
    {
        public SelectableObject Recipient { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheTokenIsYoursMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recip">The recipient.</param>
        public InvalidTokenMessage(SelectableObject sender, SelectableObject recip)
            : base(sender)
        {
            Recipient = recip;
        }
    }

    /// <summary>
    /// This message is passed from tower to tower to pass the token.
    /// </summary>
    public class TheTokenIsYoursRoutedMessage : RoutedMessage
    {
        public Guid Token { get; set; }

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
