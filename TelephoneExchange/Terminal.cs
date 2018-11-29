using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Terminal
    {
        private Port _port;

        // false as default
        private bool _isConnected;

        private bool _isRinging;

        public Action<string> Log { get; set; } = null;

        // for UI
        public event EventHandler<IncomingCallEventArgs> IncomingCall;

        public void NotificationAboutIncomingCall(object sender, IncomingCallEventArgs e)
        {
            Log?.Invoke($"{e.SenderPhoneNumber} is calling {_port.PhoneNumber}");

            _isRinging = true;

            //OnIncomingCall(e);
        }

        public void Call(string number)
        {
            _port.Call(number);
        }

        // decline incoming call 
        public void Decline()
        {
            if (!_isRinging)
                return;

            _isRinging = false;
            _port.Decline();
        }

        // reject outgoing or call in progress
        public void Reject()
        {
            _port.RejectCall();
        }

        public void Answer()
        {
            if (!_isRinging)
                return;

            _port.Answer();
        }

        public void ConnectToPort(Port port)
        {
            if (_isConnected)
                return;

            _port = port;
            _isConnected = true;

            // TODO subscribe for necessary events
            _port.Incoming += NotificationAboutIncomingCall;
            _port.AbortIncoming += PortOnAbortIncoming;
            _port.OutgoingCallResult += CallResult;
            _port.ConnectWithTerminal();
        }

        private void CallResult(object sender, CallResultEventArgs e)
        {
            switch (e.AnswerType)
            {
                case AnswerType.Answered:
                    Log($"{e.ReceiverPhoneNumber} answered a call. Now call is in progress");
                    break;
                case AnswerType.Rejected:
                    Log($"{e.ReceiverPhoneNumber} rejected a call");
                    break;
                case AnswerType.NotAnswered:
                    Log($"{e.ReceiverPhoneNumber} did not answer");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PortOnAbortIncoming(object sender, IncomingCallEventArgs e)
        {
            Log($"Call from {e.SenderPhoneNumber} was aborted");

            _isRinging = false;
        }

        public void DisconnectFromPort()
        {
            if (!_isConnected)
                return;

            _port.Incoming -= NotificationAboutIncomingCall;
            _port.ConnectWithTerminal();

            _port = null;
            _isConnected = false;
        }

        protected virtual void OnIncomingCall(IncomingCallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }
    }
}
