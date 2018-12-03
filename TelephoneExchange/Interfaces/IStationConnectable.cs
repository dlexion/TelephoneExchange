using System;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange.Interfaces
{
    public interface IStationConnectable
    {
        event EventHandler<CallResultEventArgs> IncomingCallResult;
        event EventHandler<CallEventArgs> Outgoing;
        event EventHandler<StateChangedEventArgs> StateChanged;

        void AbortCall(object sender, CallEventArgs e);
        void IncomingCall(object sender, CallEventArgs e);
        void OutgoingCallResult(object sender, CallResultEventArgs e);
    }
}