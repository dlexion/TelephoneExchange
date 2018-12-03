using System;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;
using TelephoneExchange.Interfaces;

namespace TelephoneExchange
{
    public class Port : IPort, ITerminalConnectable, IStationConnectable
    {
        private PortState _state = PortState.Offline;

        public Port(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public string PhoneNumber { get; }

        public PortState State
        {
            get => _state;
            set
            {
                var args = new StateChangedEventArgs(State);

                _state = value;

                OnStateChanged(args);
            }
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public event EventHandler<CallEventArgs> Incoming;

        public event EventHandler<CallEventArgs> AbortIncoming;

        public event EventHandler<CallResultEventArgs> ReturnedCallResult;

        public event EventHandler<CallResultEventArgs> IncomingCallResult;

        public event EventHandler<CallEventArgs> Outgoing;

        public void Call(string phoneNumber)
        {
            State = PortState.Busy;

            OnOutgoing(new CallEventArgs(PhoneNumber, phoneNumber));
        }

        public void ConnectWithTerminal()
        {
            State = PortState.Online;
        }

        public void DisconnectWithTerminal()
        {
            State = PortState.Offline;
        }

        public void IncomingCall(object sender, CallEventArgs e)
        {
            if (PhoneNumber == e.ReceiverPhoneNumber)
            {
                if (State == PortState.Online)
                {
                    State = PortState.Busy;
                    OnIncoming(e);
                }
            }
        }

        public void OutgoingCallResult(object sender, CallResultEventArgs e)
        {
            if (PhoneNumber == e.ReceiverPhoneNumber)
            {
                if (e.CallResult != CallResult.Answered)
                {
                    State = PortState.Online;
                }
                OnReturnedCallResult(e);
            }
        }

        public void AbortCall(object sender, CallEventArgs e)
        {
            if (PhoneNumber == e.ReceiverPhoneNumber)
            {
                State = PortState.Online;
                OnAbortIncoming(e);
            }
        }

        public void RejectCall()
        {
            if (State == PortState.Busy)
            {
                State = PortState.Online;
                OnIncomingCallResult(new CallResultEventArgs("", PhoneNumber, CallResult.Rejected)
                { EndTime = DateTime.Now });
            }
        }

        public void Answer()
        {
            OnIncomingCallResult(new CallResultEventArgs("", PhoneNumber, CallResult.Answered)
            { StartTime = DateTime.Now });
        }

        protected virtual void OnStateChanged(StateChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        protected virtual void OnIncoming(CallEventArgs e)
        {
            Incoming?.Invoke(this, e);
        }

        protected virtual void OnAbortIncoming(CallEventArgs e)
        {
            AbortIncoming?.Invoke(this, e);
        }

        protected virtual void OnReturnedCallResult(CallResultEventArgs e)
        {
            ReturnedCallResult?.Invoke(this, e);
        }

        protected virtual void OnIncomingCallResult(CallResultEventArgs e)
        {
            IncomingCallResult?.Invoke(this, e);
        }

        protected virtual void OnOutgoing(CallEventArgs e)
        {
            Outgoing?.Invoke(this, e);
        }
    }
}