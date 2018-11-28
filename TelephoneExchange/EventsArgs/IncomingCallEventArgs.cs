using System;

namespace TelephoneExchange.EventsArgs
{
    public class IncomingCallEventArgs : EventArgs
    {
        public IncomingCallEventArgs(string senderPhoneNumber, string receiverPhoneNumber)
        {
            SenderPhoneNumber = senderPhoneNumber;
            ReceiverPhoneNumber = receiverPhoneNumber;
        }

        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }
    }
}