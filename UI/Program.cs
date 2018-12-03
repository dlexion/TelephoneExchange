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
            Billing billing = new Billing(new BillingUnitOfWork());
            var equipment1 = billing.Contract("Ivan", "Ivanov");

            var t1 = equipment1.Item1;
            t1.Log = Console.WriteLine;
            var port1 = equipment1.Item2;

            var equipment2 = billing.Contract("Dmitry", "Sidorod");

            var t2 = equipment2.Item1;
            t2.Log = Console.WriteLine;
            var port2 = equipment2.Item2;

            var equipment3 = billing.Contract("Petr", "Sidorod");

            var t3 = equipment3.Item1;
            t3.Log = Console.WriteLine;
            var port3 = equipment3.Item2;

            var station = new Station(new List<Port>()
            {
                port1,
                port2,
                port3
            });

            station.CallCompleted += billing.CollectCall;

            t1.ConnectToPort(port1);
            t2.ConnectToPort(port2);
            t3.ConnectToPort(port3);

            Console.WriteLine(station.GetPortsState());

            t1.Call(port2.PhoneNumber);

            Console.WriteLine(station.GetPortsState());

            //t3.Call(port2.PhoneNumber);
            //t2.Reject();

            t2.Answer();
            Thread.Sleep(1000);

            Console.WriteLine(station.GetPortsState());

            t2.Reject();
            Console.WriteLine(station.GetPortsState());
            t3.Call(port1.PhoneNumber);
            t1.Reject();

            Console.WriteLine(billing.GetReport(port1.PhoneNumber, x => x.Duration.Minutes >= 0));
            //t3.DisconnectFromPort();
            //t2.Call(port3.PhoneNumber);
            //t3.Answer();
            //Console.ReadKey();
        }

        // TODO give user ability to choose what to do with incoming call
        static void RejectOrAnswer(object sender, CallEventArgs e)
        {
            var terminal = (Terminal)sender;

            //terminal.Answer();
        }
    }
}
