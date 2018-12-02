using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Station
    {
        private const double AnswerDelay = 300;

        private List<Port> _ports = new List<Port>();

        // key is sender, value is receiver
        private Dictionary<string, string> expectAnswer = new Dictionary<string, string>();

        private Dictionary<string, Timer> timersToAbortCall = new Dictionary<string, Timer>();

        private Dictionary<string, string> callInProgress = new Dictionary<string, string>();

        //subscribe here
        public Station(List<Port> ports)
        {
            _ports = ports;
            InitialSubscribeToPorts(_ports);
        }

        public event EventHandler<CallEventArgs> IncomingCall;

        public event EventHandler<CallResultEventArgs> OutgoingCallResult;

        public event EventHandler<CallEventArgs> AbortIncomingCall;

        public string GetPortsState()
        {
            var info = new StringBuilder();

            foreach (var port in _ports)
            {
                info.Append(_ports.IndexOf(port)).Append(" " + port.State).Append("\n");
            }

            return info.ToString();
        }

        private bool Exists(string number)
        {
            foreach (var port in _ports)
            {
                if (number == port.PhoneNumber)
                    return true;
            }

            return false;
        }

        public void ProcessOutgoingCall(object sender, CallEventArgs e)//string senderPhoneNumber, string receiverPhoneNumber)
        {
            var senderPhoneNumber = e.SenderPhoneNumber;
            var receiverPhoneNumber = e.ReceiverPhoneNumber;

            // TODO check is receiver exists or busy
            if (!Exists(receiverPhoneNumber))
            {
                OnOutgoingCallResult(new CallResultEventArgs(receiverPhoneNumber, senderPhoneNumber,
                    CallResult.NotExists));
                return;
            }

            if (_ports.Find(x => x.PhoneNumber == receiverPhoneNumber).State == PortState.Busy)
            {
                OnOutgoingCallResult(new CallResultEventArgs(receiverPhoneNumber, senderPhoneNumber,
                    CallResult.Busy));
                return;
            }

            expectAnswer.Add(senderPhoneNumber, receiverPhoneNumber);

            OnIncomingCall(new CallEventArgs(senderPhoneNumber, receiverPhoneNumber));

            var timer = CreateTimer(senderPhoneNumber, receiverPhoneNumber);

            timersToAbortCall.Add(senderPhoneNumber, timer);
        }

        private Timer CreateTimer(string senderPhoneNumber, string receiverPhoneNumber)
        {
            var timer = new Timer(AnswerDelay)
            {
                AutoReset = false
            };
            timer.Elapsed += (sender, eventArgs) =>
            {
                OnOutgoingCallResult(new CallResultEventArgs(receiverPhoneNumber, senderPhoneNumber,
                    CallResult.NotAnswered));
                OnAbortIncomingCall(new CallEventArgs(senderPhoneNumber, receiverPhoneNumber));
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
            port.Outgoing += ProcessOutgoingCall;
            port.IncomingCallResult += ProcessIncomingCallResult;

            IncomingCall += port.IncomingCall;
            AbortIncomingCall += port.AbortCall;
            OutgoingCallResult += port.OutgoingCallResult;
        }

        private void ProcessIncomingCallResult(object sender, CallResultEventArgs e)
        {
            switch (e.CallResult)
            {
                case CallResult.Answered:
                    ProcessAnsweredCall(e.ReceiverPhoneNumber);
                    break;
                case CallResult.Rejected:
                    ProcessRejectedCall(e.ReceiverPhoneNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnIncomingCall(CallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }

        protected virtual void OnOutgoingCallResult(CallResultEventArgs e)
        {
            OutgoingCallResult?.Invoke(this, e);
        }

        protected virtual void OnAbortIncomingCall(CallEventArgs e)
        {
            AbortIncomingCall?.Invoke(this, e);
        }

        public void ProcessRejectedCall(string phoneNumber)
        {
            var senderNumber = string.Empty;
            var callResultReceiver = string.Empty;

            if (expectAnswer.ContainsKey(phoneNumber) || expectAnswer.ContainsValue(phoneNumber))
            {
                //sender reject a call
                if (expectAnswer.ContainsKey(phoneNumber))
                {
                    senderNumber = phoneNumber;
                    callResultReceiver = expectAnswer[phoneNumber];
                }
                // receiver rejected a call
                else
                {
                    senderNumber = expectAnswer.FirstOrDefault(x => x.Value == phoneNumber).Key;
                    callResultReceiver = senderNumber;
                }

                DisposeTimer(senderNumber);
                expectAnswer.Remove(senderNumber);
            }
            else if (callInProgress.ContainsKey(phoneNumber) || callInProgress.ContainsValue(phoneNumber))
            {
                if (callInProgress.ContainsKey(phoneNumber))
                {
                    senderNumber = phoneNumber;
                    callResultReceiver = callInProgress[phoneNumber];
                }
                else
                {
                    senderNumber = callInProgress.FirstOrDefault(x => x.Value == phoneNumber).Key;
                    callResultReceiver = senderNumber;
                }

                callInProgress.Remove(senderNumber);
            }

            OnOutgoingCallResult(new CallResultEventArgs(phoneNumber, callResultReceiver, CallResult.Rejected));
            //TODO collect call info
        }

        private void DisposeTimer(string key)
        {
            timersToAbortCall[key].Dispose();
            timersToAbortCall.Remove(key);
        }

        public void ProcessAnsweredCall(string receiverPhoneNumber)
        {
            var senderNumber = expectAnswer.FirstOrDefault(x => x.Value == receiverPhoneNumber).Key;

            if (expectAnswer.ContainsKey(senderNumber))
            {
                DisposeTimer(senderNumber);
                expectAnswer.Remove(senderNumber);

                callInProgress.Add(senderNumber, receiverPhoneNumber);

                OnOutgoingCallResult(new CallResultEventArgs(senderNumber, receiverPhoneNumber, CallResult.Answered));
            }
        }
    }
}