using System;
using TelephoneExchange.Enums;

namespace TelephoneExchange.EventsArgs
{
    public class CallResultEventArgs : EventArgs
    {
        public CallResultEventArgs(string senderPhoneNumber, string receiverPhoneNumber, CallResult callResult)
        {
            SenderPhoneNumber = senderPhoneNumber;
            ReceiverPhoneNumber = receiverPhoneNumber;
            CallResult = callResult;
        }

        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }

        public CallResult CallResult { get; set; }
    }
}