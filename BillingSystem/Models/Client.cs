using TelephoneExchange;

namespace BillingSystem
{
    public class Client
    {
        public string FirstName { get; }

        public string LastName { get; }

        public Terminal Terminal { get; set; }

        public Client(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}