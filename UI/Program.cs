using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelephoneExchange;
using TelephoneExchange.EventsArgs;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO create Person instance and give him terminal throw billing
            var t = new Terminal()
            {
                Log = Console.WriteLine
            };

            var port = new Port("1");

            var t2 = new Terminal()
            {
                Log = Console.WriteLine
            };

            var port2 = new Port("2");

            var station = new Station(new List<Port>()
            {
                port,
                port2
            });

            t.ConnectToPort(port);
            t2.ConnectToPort(port2);

            Console.WriteLine(station.GetPortsState());

            t.Call(port2.PhoneNumber);

            Console.WriteLine(station.GetPortsState());

            Thread.Sleep(1000);


            //t.Reject();

            Console.WriteLine(station.GetPortsState());
            Console.ReadKey();
        }

        // TODO give user ability to choose what to do with incoming call
        static void RejectOrAnswer(object sender, CallEventArgs e)
        {
            var terminal = (Terminal)sender;

            //terminal.Answer();
        }
    }
}
