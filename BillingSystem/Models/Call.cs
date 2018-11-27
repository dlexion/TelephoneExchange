using System;
using System.Text;

namespace BillingSystem.Models
{
    public class Call
    {
        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Duration => EndTime - StartTime;

        public Call(string senderPhoneNumber, string receiverPhoneNumber, DateTime startTime, DateTime endTime)
        {
            SenderPhoneNumber = senderPhoneNumber;

            ReceiverPhoneNumber = receiverPhoneNumber;

            StartTime = startTime;

            EndTime = endTime;
        }

        public override string ToString()
        {
            return $"{SenderPhoneNumber} | {ReceiverPhoneNumber} | {StartTime} | {EndTime} | {Duration}";
        }
    }
}