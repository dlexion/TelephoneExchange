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
                PhoneNumber = "1",
                Log = Console.WriteLine
            };

            var port = new Port();

            var t2 = new Terminal()
            {
                PhoneNumber = "2",
                Log = Console.WriteLine
            };

            var port2 = new Port();

            var station = new Station(new List<Port>()
            {
                port,
                port2
            });

            #region first port connection

            t.Connect += port.ConnectWithTerminal;
            t.Disconnect += port.DisconnectWithTerminal;

            t.ConnectToPort();

            t.OutgoingCall += port.ReportStationAboutOutgoingCall;

            port.Outgoing += station.ReportPortAboutIncomingCall;
            station.Call += port.ReportTerminalAboutIncomingCall;

            port.Incoming += t.NotificationAboutIncomingCall;

            t.RejectCall += port.ReportStationAboutCallReject;
            port.CallReject += station.ReportPortAboutCallReject;
            station.CallReject += port.ReportTerminalAboutCallReject;

            port.CallWasRejectedFromReceiver += t.CallWasRejected;

            t.AnswerCall += port.ReportStationAboutCallAnswer;
            port.CallAnswer += station.ReportPortAboutCallAnswer;

            #endregion

            #region second port connection
            t2.Connect += port2.ConnectWithTerminal;
            t2.Disconnect += port2.DisconnectWithTerminal;

            t2.ConnectToPort();

            t2.OutgoingCall += port2.ReportStationAboutOutgoingCall;

            port2.Outgoing += station.ReportPortAboutIncomingCall;

            station.Call += port2.ReportTerminalAboutIncomingCall;

            port2.Incoming += t2.NotificationAboutIncomingCall;

            t2.RejectCall += port2.ReportStationAboutCallReject;
            port2.CallReject += station.ReportPortAboutCallReject;
            station.CallReject += port2.ReportTerminalAboutCallReject;
            port2.CallWasRejectedFromReceiver += t2.CallWasRejected;

            t2.AnswerCall += port2.ReportStationAboutCallAnswer;
            port2.CallAnswer += station.ReportPortAboutCallAnswer;

            #endregion

            t.IncomingCall += RejectOrAnswer;
            t2.IncomingCall += RejectOrAnswer;

            t.Call("2");
            //t2.Answer();

            //Thread.Sleep(1000);

            t.Reject();
        }

        // TODO give user ability to choose what to do with incoming call
        static void RejectOrAnswer(object sender, CallEventArgs e)
        {
            var terminal = (Terminal)sender;

            terminal.Answer();
        }
    }
}
