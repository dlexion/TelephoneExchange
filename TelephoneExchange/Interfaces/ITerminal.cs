using System;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange.Interfaces
{
    public interface ITerminal
    {
        Action<string> Log { get; set; }

        event EventHandler<CallEventArgs> IncomingCall;

        void Answer();
        void Call(string number);
        void ConnectToPort(ITerminalConnectable port);
        void DisconnectFromPort();
        void Reject();
    }
}