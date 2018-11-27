using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Terminal
    {
        public string PhoneNumber { get; set; }

        public Action<string> Log { get; set; } = null;

        public event EventHandler<CallEventArgs> OutgoingCall;

        public event EventHandler<CallEventArgs> IncomingCall;

        public event EventHandler<CallEventArgs> RejectCall;

        public event EventHandler<CallEventArgs> AnswerCall;

        public event EventHandler<ConnectionEventArgs> Connect;

        public event EventHandler Disconnect;

        public void NotificationAboutIncomingCall(object sender, CallEventArgs e)
        {
            Log?.Invoke($"{e.SenderPhoneNumber} is calling {PhoneNumber}");

            OnIncomingCall(e);
        }

        public void Call(string number)
        {
            var args = new CallEventArgs(PhoneNumber, number);

            OnOutgoingCall(args);
        }

        // TODO args
        public void Answer()
        {
            Log?.Invoke($"{PhoneNumber} have answered a call");

            var e = new CallEventArgs("", PhoneNumber)
            {
                StartTime = DateTime.UtcNow
            };

            OnAnswerCall(e);
        }

        // TODO add args for call reject
        public void Reject()
        {
            Log?.Invoke($"{PhoneNumber} have rejected a call");

            //create new args class
            OnRejectCall(new CallEventArgs("", PhoneNumber));
        }

        public void CallWasRejected(object sender, CallEventArgs e)
        {
            Log?.Invoke($"Call from {e.SenderPhoneNumber} was rejected by {e.ReceiverPhoneNumber}");
        }

        public void ConnectToPort()
        {
            var args = new ConnectionEventArgs(this.PhoneNumber);

            OnConnect(args);
        }

        public void DisconnectFromPort()
        {
            OnDisconnect();
        }

        protected virtual void OnConnect(ConnectionEventArgs e)
        {
            Connect?.Invoke(this, e);
        }

        protected virtual void OnDisconnect()
        {
            Disconnect?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnOutgoingCall(CallEventArgs e)
        {
            OutgoingCall?.Invoke(this, e);
        }

        protected virtual void OnIncomingCall(CallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }

        protected virtual void OnRejectCall(CallEventArgs e)
        {
            RejectCall?.Invoke(this, e);
        }

        protected virtual void OnAnswerCall(CallEventArgs e)
        {
            AnswerCall?.Invoke(this, e);
        }
    }
}
