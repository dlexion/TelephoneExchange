using System;
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
        public event EventHandler<CallEventArgs> IncomingCall;

        public void NotificationAboutIncomingCall(object sender, CallEventArgs e)
        {
            Log?.Invoke($"{e.SenderPhoneNumber} is calling {_port.PhoneNumber}");

            _isRinging = true;

            OnIncomingCall(e);
        }

        public void Call(string number)
        {
            _port.Call(number);
        }

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

            _port.Incoming += NotificationAboutIncomingCall;
            _port.AbortIncoming += PortOnAbortIncoming;
            _port.ReturnedCallResult += OutgoingCallResult;
            _port.ConnectWithTerminal();
        }

        private void OutgoingCallResult(object sender, CallResultEventArgs e)
        {
            switch (e.CallResult)
            {
                case CallResult.Answered:
                    Log?.Invoke($"{e.ReceiverPhoneNumber} answered a call. Now call is in progress");
                    break;
                case CallResult.Rejected:
                    Log?.Invoke($"{e.SenderPhoneNumber} rejected a call");
                    break;
                case CallResult.NotAnswered:
                    Log?.Invoke($"{e.SenderPhoneNumber} did not answer");
                    break;
                case CallResult.NotExists:
                    Log?.Invoke($"Number {e.SenderPhoneNumber} is wrong or offline");
                    break;
                case CallResult.Busy:
                    Log?.Invoke($"Number {e.SenderPhoneNumber} is busy");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PortOnAbortIncoming(object sender, CallEventArgs e)
        {
            Log?.Invoke($"Call from {e.SenderPhoneNumber} was aborted");

            _isRinging = false;
        }

        public void DisconnectFromPort()
        {
            if (!_isConnected)
                return;

            _port.Incoming -= NotificationAboutIncomingCall;
            _port.AbortIncoming -= PortOnAbortIncoming;
            _port.ReturnedCallResult -= OutgoingCallResult;
            _port.DisconnectWithTerminal();

            _port = null;
            _isConnected = false;
        }

        protected virtual void OnIncomingCall(CallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }
    }
}
