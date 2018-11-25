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

        private Action<string> reportResult;

        public event EventHandler<CallEventArgs> OutgoingCall;

        public event EventHandler<CallEventArgs> IncomingCall;

        public event EventHandler<CallEventArgs> RejectCall;

        public event EventHandler<CallEventArgs> AnswerCall;

        public event EventHandler<ConnectionEventArgs> Connect;

        public event EventHandler Disconnect;

        public Terminal(Action<string> report)
        {
            reportResult = report;
        }

        public void NotificationAboutIncomingCall(object sender, CallEventArgs e)
        {
            reportResult($"{e.SenderPhoneNumber} is calling you");
        }

        public void Call(string number)
        {
            var args = new CallEventArgs (PhoneNumber, number);

            OnOutgoingCall(args);
        }

        // TODO
        public void Answer(object sender, CallEventArgs e)
        {
            e.StartTime = DateTime.UtcNow;

            OnAnswerCall(e);
        }

        public void Reject()
        {
            //Console.WriteLine("We have rejected a call");
            reportResult("We have rejected a call");

            //create new args class
            OnRejectCall(new CallEventArgs("", PhoneNumber));
        }

        public void CallWasRejected(object sender, CallEventArgs e)
        {
            reportResult("Call was rejected");
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
