namespace TelephoneExchange.EventsArgs
{
    // TODO remove if unused
    public class ConnectionEventArgs
    {
        public string PhoneNumber { get; set; }

        public ConnectionEventArgs(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}