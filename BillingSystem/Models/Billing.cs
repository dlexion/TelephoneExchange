using System;
using System.Linq;
using System.Text;
using TelephoneExchange;
using TelephoneExchange.EventsArgs;

namespace BillingSystem.Models
{
    // TODO reduce balance once per month
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
                _data.Clients.Add(new Client(firstName, lastName) { Terminal = new Phone(terminal.PhoneNumber) });

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

        public void ReplenishmentBalance(string phoneNumber, decimal money)
        {
            var phone = _data.Phones.GetAll().FirstOrDefault(x => x.PhoneNumber == phoneNumber);

            if (phone != null)
            {
                phone.Balance += money;
            }
        }

        public string GetReport(string phoneNumber, Func<Call, bool> selector = null)
        {
            StringBuilder info = new StringBuilder();

            var q = selector != null
                ? _data.Calls.GetAll()
                    .Where(x => x.SenderPhoneNumber == phoneNumber || x.ReceiverPhoneNumber == phoneNumber)
                    .Where(selector).ToList()
                : _data.Calls.GetAll()
                    .Where(x => x.SenderPhoneNumber == phoneNumber || x.ReceiverPhoneNumber == phoneNumber).ToList();

            foreach (var item in q)
            {
                info.Append(item);
            }

            return info.ToString();
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