using System;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange.Interfaces
{
    public interface ITerminalConnectable
    {
        event EventHandler<CallEventArgs> AbortIncoming;
        event EventHandler<CallEventArgs> Incoming;
        event EventHandler<CallResultEventArgs> ReturnedCallResult;

        void ConnectWithTerminal();
        void DisconnectWithTerminal();
    }
}