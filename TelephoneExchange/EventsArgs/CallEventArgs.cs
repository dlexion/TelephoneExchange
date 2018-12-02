using System;

namespace TelephoneExchange.EventsArgs
{
    public class CallEventArgs : EventArgs
    {
        public CallEventArgs(string senderPhoneNumber, string receiverPhoneNumber)
        {
            SenderPhoneNumber = senderPhoneNumber;
            ReceiverPhoneNumber = receiverPhoneNumber;
        }

        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }
    }
}