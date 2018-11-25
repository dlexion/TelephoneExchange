using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelephoneExchange;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Terminal(Console.WriteLine)
            {
                PhoneNumber = "1"
            };

            var port = new Port();

            var t2 = new Terminal(Console.WriteLine)
            {
                PhoneNumber = "2"
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

            #endregion

            t.Call("2");
            Thread.Sleep(3000);
            t2.Reject();
        }
    }
}
