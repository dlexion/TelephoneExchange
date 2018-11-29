using System;
using TelephoneExchange.Enums;

namespace TelephoneExchange.EventsArgs
{
    public class StateChangedEventArgs : EventArgs
    {
        public StateChangedEventArgs(PortState previousState)
        {
            PreviousState = previousState;
        }

        public PortState PreviousState { get; set; }
    }
}