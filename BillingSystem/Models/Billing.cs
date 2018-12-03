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
        private BillingUnitOfWork _data;

        public Billing(BillingUnitOfWork data)
        {
            _data = data;
        }

        public Tuple<Terminal, Port> Contract(string firstName, string lastName)
        {
            var terminal = new Terminal();

            var port = new Port(GetPhoneNumberForNewClient());

            if (!_data.Clients.GetAll().Any(x => x.FirstName == firstName && x.LastName == lastName))
            {
                _data.Clients.Add(new Client(firstName, lastName)
                { Port = new PortModel(port.PhoneNumber) { Tariff = new Tariff() } });

                return new Tuple<Terminal, Port>(terminal, port);
            }

            return null;
        }

        public void CollectCall(object sender, CallInfoEventArgs e)
        {
            _data.Calls.Add(new Call(e.SenderPhoneNumber, e.ReceiverPhoneNumber, e.StartTime, e.EndTime));
        }

        public decimal GetBalance(string phoneNumber)
        {
            var phone = _data.Ports.GetAll().FirstOrDefault(x => x.PhoneNumber == phoneNumber);

            return phone?.Balance ?? default(decimal);
        }

        public void ReplenishmentBalance(string phoneNumber, decimal money)
        {
            var phone = _data.Ports.GetAll().FirstOrDefault(x => x.PhoneNumber == phoneNumber);

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
                info.Append(item).Append($" | {CalculateCallCost(item, phoneNumber)}").Append("\n");
            }

            return info.ToString();
        }

        private string GetPhoneNumberForNewClient()
        {
            string newNumber;
            do
            {
                newNumber = new Random().Next(1000, 9999).ToString();

            } while (_data.PhoneNumbers.GetAll().Any(x => x.Equals(newNumber)));

            _data.PhoneNumbers.Add(newNumber);

            return newNumber;
        }

        private decimal CalculateCallCost(Call call, string phoneNumber)
        {
            var client = _data.Clients.GetAll().FirstOrDefault(x => x.Port.PhoneNumber == phoneNumber);

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            //take seconds to show how it works
            return call.SenderPhoneNumber == phoneNumber
                ? (call.Duration.Seconds + 1) * client.Port.Tariff.CostPerMinute
                : 0;
        }
    }
}