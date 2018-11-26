using System;

namespace TelephoneExchange.EventsArgs
{
    public class CallEventArgs : EventArgs
    {
        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public CallEventArgs(string senderNumber, string receiverNumber)
        {
            SenderPhoneNumber = senderNumber;
            ReceiverPhoneNumber = receiverNumber;
        }
    }
}
