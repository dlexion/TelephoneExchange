using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;
using System.Timers;

namespace TelephoneExchange
{
    public class Station
    {
        private const double AnswerDelay = 300;

        private List<Port> _ports = new List<Port>();

        // key is sender, value is receiver
        private Dictionary<string, string> expectAnswer = new Dictionary<string, string>();

        private Dictionary<string, Timer> timersToAbort = new Dictionary<string, Timer>();

        private Dictionary<Port, Port> callInProgress = new Dictionary<Port, Port>();

        //subscribe here
        public Station(List<Port> ports)
        {
            this._ports = ports;
            InitialSubscribeToPorts(this._ports);
        }

        // check in port number
        private event EventHandler<IncomingCallEventArgs> IncomingCall;

        // check in port number
        private event EventHandler<CallResultEventArgs> OutgoingCallResult;

        private event EventHandler<IncomingCallEventArgs> AbortIncomingCall;

        public string GetPortsState()
        {
            var info = new StringBuilder();

            foreach (var port in _ports)
            {
                info.Append(_ports.IndexOf(port)).Append(" " + port.State).Append("\n");
            }

            return info.ToString();
        }

        // TODO phone number instead of Port instance
        public void ProcessIncomingCall(string phoneNumberFrom, string phoneNumberTo)
        {
            expectAnswer.Add(phoneNumberFrom, phoneNumberTo);

            var args = new IncomingCallEventArgs(phoneNumberFrom, phoneNumberTo);

            OnIncomingCall(args);

            var timer = CreateTimer(phoneNumberFrom, phoneNumberTo, args);

            timersToAbort.Add(phoneNumberFrom, timer);
        }

        private Timer CreateTimer(string phoneNumberFrom, string phoneNumberTo, IncomingCallEventArgs args)
        {
            // TODO
            var timer = new Timer(AnswerDelay)
            {
                AutoReset = false
            };
            timer.Elapsed += (sender, eventArgs) =>
            {
                OnAbortIncomingCall(args);
                OnOutgoingCallResult(new CallResultEventArgs(phoneNumberFrom, AnswerType.NotAnswered)
                { ReceiverPhoneNumber = phoneNumberTo });
                Console.WriteLine("TIMER!");
            };
            timer.Start();

            return timer;
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

        private void ProcessPortState(object sender, StateChangedEventArgs e)
        {
            var port = (Port)sender;

            if (port.State == PortState.Online && e.PreviousState == PortState.Offline)
            {
                SubscribePort(port);
            }
            else if (port.State == PortState.Offline)
            {
                UnsubscribePort(port);
            }
        }

        // TODO
        private void UnsubscribePort(Port port)
        {
            throw new NotImplementedException();
        }

        private void SubscribePort(Port port)
        {
            this.IncomingCall += port.IncomingCall;
            this.AbortIncomingCall += port.AbortCall;
            this.OutgoingCallResult += port.CallResult;
        }

        protected virtual void OnIncomingCall(IncomingCallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }

        protected virtual void OnOutgoingCallResult(CallResultEventArgs e)
        {
            OutgoingCallResult?.Invoke(this, e);
        }

        protected virtual void OnAbortIncomingCall(IncomingCallEventArgs e)
        {
            AbortIncomingCall?.Invoke(this, e);
        }

        public void ProcessDecliningCall(Port declinedPort)
        {
            var senderNumber = expectAnswer.FirstOrDefault(x => x.Value == declinedPort.PhoneNumber).Key;

            // dispose timer
            timersToAbort[senderNumber].Dispose();
            timersToAbort.Remove(senderNumber);

            expectAnswer.Remove(senderNumber);

            OnOutgoingCallResult(new CallResultEventArgs(senderNumber, AnswerType.Rejected)
            { ReceiverPhoneNumber = declinedPort.PhoneNumber });
        }
    }
}