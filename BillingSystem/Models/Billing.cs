using System;
using System.Linq;
using TelephoneExchange;
using TelephoneExchange.EventsArgs;

namespace BillingSystem.Models
{
    public class Billing
    {
        private UnitOfWork _data;

        public Billing(UnitOfWork data)
        {
            _data = data;
        }

        public Tuple<Terminal, Port> Contract(string firstName, string lastName)
        {
            var terminal = new Terminal()
            {
                PhoneNumber = GetPhoneNumberForNewClient()
            };

            var port = new Port();

            if (!_data.Clients.GetAll().Any(x => x.FirstName == firstName && x.LastName == lastName))
            {
                _data.Clients.Add(new Client(firstName, lastName));

                return new Tuple<Terminal, Port>(terminal, port);
            }

            return null;
        }

        public void CollectCall(object sender, CallEventArgs e)
        {
            _data.Calls.Add(new Call(e.SenderPhoneNumber, e.ReceiverPhoneNumber, e.StartTime, e.EndTime));
        }

        public decimal GetBalance(string phoneNumber)
        {
            var phone = _data.Phones.GetAll().FirstOrDefault(x => x.PhoneNumber == phoneNumber);

            return phone?.Balance ?? default(decimal);
        }

        private string GetPhoneNumberForNewClient()
        {
            string newNumber;
            do
            {
                newNumber = new Random().Next(1000, 9999).ToString();

            } while (!_data.PhoneNumbers.GetAll().Any(x => x.Equals(newNumber)));

            return newNumber;
        }
    }
}