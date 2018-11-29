using System;
using TelephoneExchange.Enums;

namespace TelephoneExchange.EventsArgs
{
    public class CallResultEventArgs : EventArgs
    {
        public CallResultEventArgs(string senderPhoneNumber, AnswerType answerType)
        {
            SenderPhoneNumber = senderPhoneNumber;
            AnswerType = answerType;
        }

        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }

        public AnswerType AnswerType { get; set; }

    }
}