using TelephoneExchange.Enums;

namespace TelephoneExchange.Interfaces
{
    public interface IPort/* : ITerminalConnectable*/
    {
        string PhoneNumber { get; }
        PortState State { get; }

        void Answer();
        void Call(string phoneNumber);
        void RejectCall();
    }
}