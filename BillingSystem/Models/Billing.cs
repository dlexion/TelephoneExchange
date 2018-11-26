using TelephoneExchange;
using TelephoneExchange.EventsArgs;

namespace BillingSystem
{
    public class Billing
    {
        private UnitOfWork _data;

        public Billing(UnitOfWork data)
        {
            _data = data;
        }

        public void Contract(string firstName, string lastName, out Terminal terminal)
        {
            terminal = new Terminal()
            {
                PhoneNumber = GetPhoneNumberForClient()
            };
            _data.Clients.Add(new Client(firstName, lastName));
        }

        // TODO give uniq number and collect all given numbers
        private string GetPhoneNumberForClient()
        {
            return "1";
        }

        public void CollectCall(object sender, CallEventArgs e)
        {
            _data.Calls.Add(new Call(e.SenderPhoneNumber, e.ReceiverPhoneNumber, e.StartTime, e.EndTime));
        }
    }
}