﻿using System;
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
                var args = new StateChangedEventArgs(State);

                _state = value;

                OnStateChanged(args);
            }
        }

        // TODO args with previous state
        public event EventHandler<StateChangedEventArgs> StateChanged;

        public event EventHandler<IncomingCallEventArgs> Incoming;

        public event EventHandler<IncomingCallEventArgs> AbortIncoming;

        public event EventHandler<CallResultEventArgs> OutgoingCallResult;

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

        public void CallResult(object sender, CallResultEventArgs e)
        {
            if (PhoneNumber == e.SenderPhoneNumber)
            {
                if (e.AnswerType != AnswerType.Answered)
                {
                    State = PortState.Online;
                }
                OnOutgoingCallResult(e);
            }
        }

        protected virtual void OnStateChanged(StateChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        protected virtual void OnIncoming(IncomingCallEventArgs e)
        {
            Incoming?.Invoke(this, e);
        }

        protected virtual void OnAbortIncoming(IncomingCallEventArgs e)
        {
            AbortIncoming?.Invoke(this, e);
        }

        public void AbortCall(object sender, IncomingCallEventArgs e)
        {
            if (PhoneNumber == e.ReceiverPhoneNumber)
            {
                if (State == PortState.Busy)
                {
                    State = PortState.Online;
                    OnAbortIncoming(e);
                }
            }
        }

        protected virtual void OnOutgoingCallResult(CallResultEventArgs e)
        {
            OutgoingCallResult?.Invoke(this, e);
        }
    }
}