using System;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Port
    {
        // TODO should initialization be in constructor?
        private PortState _state = PortState.Offline;

        public Port(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public string PhoneNumber { get; }

        public Station Station { get; set; }

        public PortState State
        {
            get => _state;
            set
            {
                _state = value;
                OnStateChanged();
            }
        }

        public event EventHandler StateChanged;

        public event EventHandler<IncomingCallEventArgs> Incoming;

        public void Call(string phoneNumber)
        {
            Station.ProcessIncomingCall(this, phoneNumber);

            State = PortState.Busy;
        }

        public void ConnectWithTerminal()
        {
            State = PortState.Online;
        }

        public void DisconnectWithTerminal()
        {
            State = PortState.Offline;
        }

        public void IncomingCall(object sender, IncomingCallEventArgs e)
        {
            if (PhoneNumber == e.ReceiverPhoneNumber)
            {
                State = PortState.Busy;
                OnIncoming(e);
            }
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnIncoming(IncomingCallEventArgs e)
        {
            Incoming?.Invoke(this, e);
        }
    }
}