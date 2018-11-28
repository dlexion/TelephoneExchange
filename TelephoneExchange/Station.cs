using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Station
    {

        private List<Port> ports = new List<Port>();

        //List<Port> expectAnswer = new List<Port>();

        private Dictionary<Port, Port> expectAnswer = new Dictionary<Port, Port>();

        private Dictionary<Port, Port> callInProgress = new Dictionary<Port, Port>();

        //subscribe here
        public Station(List<Port> ports)
        {
            this.ports = ports;
            InitialSubscribeToPorts(this.ports);
        }

        // check in port number
        private event EventHandler<IncomingCallEventArgs> IncomingCall;

        // check in port number
        private event EventHandler<CallResultEventArgs> OutgoingCallResult;

        public string GetPortsState()
        {
            var info = new StringBuilder();

            foreach (var port in ports)
            {
                info.Append(ports.IndexOf(port)).Append(" " + port.State).Append("\n");
            }

            return info.ToString();
        }


        public void ProcessIncomingCall(Port portFrom, string phoneNumberTo)
        {
            var args = new IncomingCallEventArgs(portFrom.PhoneNumber, phoneNumberTo);

            OnIncomingCall(args);

            // TODO
            var timer = new System.Timers.Timer(3000)
            {
                AutoReset = false
            };
            timer.Elapsed += (sender, eventArgs) =>
                {
                    //OnOutgoingCallResult(new CallResultEventArgs(phoneNumberTo, AnswerType.NotAnswered));
                    Console.WriteLine("TIMER!");
                };
            timer.Start();
        }

        private void InitialSubscribeToPorts(IEnumerable<Port> ports)
        {
            foreach (var port in ports)
            {
                InitialSubscribeToPort(port);
            }
        }

        private void InitialSubscribeToPort(Port port)
        {
            port.Station = this;
            port.StateChanged += ProcessPortState;
        }

        private void ProcessPortState(object sender, EventArgs e)
        {
            var port = (Port) sender;

            if (port.State == PortState.Online)
            {
                SubscribePort(port);
            }
            else if(port.State == PortState.Offline)
            {
                UnsubscribePort(port);
            }
        }

        private void UnsubscribePort(Port port)
        {
            throw new NotImplementedException();
        }

        private void SubscribePort(Port port)
        {
            this.IncomingCall += port.IncomingCall;
        }

        protected virtual void OnIncomingCall(IncomingCallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }

        protected virtual void OnOutgoingCallResult(CallResultEventArgs e)
        {
            OutgoingCallResult?.Invoke(this, e);
        }
    }
}