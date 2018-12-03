using BillingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BillingSystem;
using TelephoneExchange;
using TelephoneExchange.EventsArgs;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            #region initialization
            var station = new Station();

            var billing = new Billing(new BillingUnitOfWork(), station);
            var equipment1 = billing.Contract("Ivan", "Ivanov");

            var t1 = equipment1.Item1;
            t1.Log = Console.WriteLine;
            var port1 = equipment1.Item2;

            station.AddPort(port1);
            t1.ConnectToPort(port1);

            var equipment2 = billing.Contract("Dmitry", "Sidorod");

            var t2 = equipment2.Item1;
            t2.Log = Console.WriteLine;
            var port2 = equipment2.Item2;

            station.AddPort(port2);
            t2.ConnectToPort(port2);


            var equipment3 = billing.Contract("Petr", "Sidorod");

            var t3 = equipment3.Item1;
            t3.Log = Console.WriteLine;
            var port3 = equipment3.Item2;

            station.AddPort(port3);
            t3.ConnectToPort(port3);
            #endregion

            t1.Call(port2.PhoneNumber);

            t2.Answer();
            Thread.Sleep(1000);

            t2.Reject();

            t3.Call(port1.PhoneNumber);
            t1.Reject();

            Console.WriteLine(billing.GetReport(port1.PhoneNumber, x => x.Duration.Minutes >= 0));

            Console.ReadKey();
        }
    }
}
