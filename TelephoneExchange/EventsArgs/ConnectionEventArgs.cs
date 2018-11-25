namespace TelephoneExchange.EventsArgs
{
    public class ConnectionEventArgs
    {
        public string PhoneNumber { get; set; }

        public ConnectionEventArgs(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}