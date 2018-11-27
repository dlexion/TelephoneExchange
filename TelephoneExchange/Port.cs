using System;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Port
    {
        // TODO should initialization be in constructor?
        public PortState State { get; set; } = PortState.Offline;

        public string PhoneNumber { get; private set; }

        public event EventHandler<CallEventArgs> Outgoing;

        public event EventHandler<CallEventArgs> Incoming;

        public event EventHandler<CallEventArgs> CallReject;

        public event EventHandler<CallEventArgs> CallAnswer;

        public event EventHandler<CallEventArgs> CallWasRejectedFromReceiver;

        public void ReportStationAboutOutgoingCall(object sender, CallEventArgs e)
        {
            if (State == PortState.Online)
            {
                State = PortState.Busy;
                OnOutgoing(e);
            }
        }

        public void ReportTerminalAboutIncomingCall(object sender, CallEventArgs e)
        {
            State = PortState.Busy;

            OnIncoming(e);
        }

        public void ReportStationAboutCallReject(object sender, CallEventArgs e)
        {
            if (State == PortState.Busy)
            {
                State = PortState.Online;

                OnCallReject(e);
            }
        }

        public void ReportTerminalAboutCallReject(object sender, CallEventArgs e)
        {
            State = PortState.Online;

            OnCallWasRejectedFromReceiver(e);
        }

        public void ReportStationAboutCallAnswer(object sender, CallEventArgs e)
        {
            OnCallAnswer(e);
        }

        public void ConnectWithTerminal(object sender, ConnectionEventArgs e)
        {
            PhoneNumber = e.PhoneNumber;

            State = PortState.Online;
        }

        public void DisconnectWithTerminal(object sender, EventArgs e)
        {
            State = PortState.Offline;
        }

        private void OnOutgoing(CallEventArgs e)
        {
            Outgoing?.Invoke(this, e);
        }

        protected virtual void OnIncoming(CallEventArgs e)
        {
            Incoming?.Invoke(this, e);
        }

        protected virtual void OnCallReject(CallEventArgs e)
        {
            CallReject?.Invoke(this, e);
        }

        protected virtual void OnCallWasRejectedFromReceiver(CallEventArgs e)
        {
            CallWasRejectedFromReceiver?.Invoke(this, e);
        }

        protected virtual void OnCallAnswer(CallEventArgs e)
        {
            CallAnswer?.Invoke(this, e);
        }
    }
}