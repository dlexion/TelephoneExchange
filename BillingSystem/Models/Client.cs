using TelephoneExchange;

namespace BillingSystem.Models
{
    public class Client
    {
        public string FirstName { get; }

        public string LastName { get; }

        public PortModel Port { get; set; }

        public Client(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}