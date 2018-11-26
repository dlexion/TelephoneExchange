﻿using System;

namespace BillingSystem
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
    }
}