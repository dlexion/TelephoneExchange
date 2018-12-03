using System;

namespace TelephoneExchange.EventsArgs
{
    public class CallInfoEventArgs : EventArgs
    {
        public CallInfoEventArgs(string senderPhoneNumber, string receiverPhoneNumber, DateTime startTime, DateTime endTime)
        {
            SenderPhoneNumber = senderPhoneNumber;
            ReceiverPhoneNumber = receiverPhoneNumber;
            StartTime = startTime;
            EndTime = endTime;
        }

        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}