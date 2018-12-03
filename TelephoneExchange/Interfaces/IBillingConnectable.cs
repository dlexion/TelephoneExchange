using System;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange.Interfaces
{
    public interface IBillingConnectable
    {
        event EventHandler<CallInfoEventArgs> CallCompleted;
    }
}