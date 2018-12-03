using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;
using TelephoneExchange.Interfaces;

namespace TelephoneExchange
{
    public class Station : IBillingConnectable
    {
        private const double AnswerDelay = 300;

        private readonly List<IPort> _ports;

        // key is sender, value is receiver
        private readonly Dictionary<string, string> _expectAnswer = new Dictionary<string, string>();

        private readonly Dictionary<string, Timer> _timersToAbortCall = new Dictionary<string, Timer>();

        private readonly Dictionary<string, string> _callsInProgress = new Dictionary<string, string>();

        private readonly Dictionary<string, DateTime> _timeCallStarted = new Dictionary<string, DateTime>();

        public Station()
        {
            _ports = new List<IPort>();
        }

        //subscribe here
        public Station(List<IPort> ports)
        {
            _ports = ports;
            InitialSubscribeToPorts(_ports.OfType<IStationConnectable>());
        }

        public event EventHandler<CallEventArgs> IncomingCall;

        public event EventHandler<CallResultEventArgs> OutgoingCallResult;

        public event EventHandler<CallEventArgs> AbortIncomingCall;

        public event EventHandler<CallInfoEventArgs> CallCompleted;

        public void AddPort(Port port)
        {
            _ports.Add(port);
            InitialSubscribeToPort(port);
        }

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

        public void ProcessOutgoingCall(object sender, CallEventArgs e)
        {
            var senderPhoneNumber = e.SenderPhoneNumber;
            var receiverPhoneNumber = e.ReceiverPhoneNumber;

            var receiverState = _ports.Find(x => x.PhoneNumber == receiverPhoneNumber).State;

            if (!Exists(receiverPhoneNumber) || receiverState == PortState.Offline)
            {
                OnOutgoingCallResult(new CallResultEventArgs(receiverPhoneNumber, senderPhoneNumber,
                    CallResult.NotExists));
                return;
            }

            if (receiverState == PortState.Busy)
            {
                OnOutgoingCallResult(new CallResultEventArgs(receiverPhoneNumber, senderPhoneNumber,
                    CallResult.Busy));
                return;
            }


            _expectAnswer.Add(senderPhoneNumber, receiverPhoneNumber);

            OnIncomingCall(new CallEventArgs(senderPhoneNumber, receiverPhoneNumber));

            var timer = CreateTimer(senderPhoneNumber, receiverPhoneNumber);
            _timersToAbortCall.Add(senderPhoneNumber, timer);
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
                DisposeTimer(senderPhoneNumber);
                _expectAnswer.Remove(senderPhoneNumber);
            };
            timer.Start();

            return timer;
        }

        private void InitialSubscribeToPorts(IEnumerable<IStationConnectable> ports)
        {
            foreach (var port in ports)
            {
                InitialSubscribeToPort(port);
            }
        }

        private void InitialSubscribeToPort(IStationConnectable port)
        {
            port.StateChanged += ProcessPortState;
        }

        private void ProcessPortState(object sender, StateChangedEventArgs e)
        {
            var port = (IPort)sender;

            if (port.State == PortState.Online && e.PreviousState == PortState.Offline)
            {
                SubscribePort(port as IStationConnectable);
            }
            else if (port.State == PortState.Offline)
            {
                UnsubscribePort(port as IStationConnectable);
            }
        }

        private void UnsubscribePort(IStationConnectable port)
        {
            port.Outgoing -= ProcessOutgoingCall;
            port.IncomingCallResult -= ProcessIncomingCallResult;

            IncomingCall -= port.IncomingCall;
            AbortIncomingCall -= port.AbortCall;
            OutgoingCallResult -= port.OutgoingCallResult;
        }

        private void SubscribePort(IStationConnectable port)
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
                    ProcessAnsweredCall(e.ReceiverPhoneNumber, e.StartTime);
                    break;
                case CallResult.Rejected:
                    ProcessRejectedCall(e.ReceiverPhoneNumber, e.EndTime);
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

        public void ProcessRejectedCall(string phoneNumber, DateTime endTime)
        {
            var senderNumber = string.Empty;
            var callResultReceiver = string.Empty;

            if (_expectAnswer.ContainsKey(phoneNumber) || _expectAnswer.ContainsValue(phoneNumber))
            {
                ProcessRejectedCall(phoneNumber, out senderNumber, out callResultReceiver, _expectAnswer);
                DisposeTimer(senderNumber);
            }
            else if (_callsInProgress.ContainsKey(phoneNumber) || _callsInProgress.ContainsValue(phoneNumber))
            {

                ProcessRejectedCall(phoneNumber, out senderNumber, out callResultReceiver, _callsInProgress);
            }

            OnOutgoingCallResult(new CallResultEventArgs(phoneNumber, callResultReceiver, CallResult.Rejected));

            DateTime startTime;

            if (!_timeCallStarted.ContainsKey(senderNumber))
            {
                startTime = endTime;
            }
            else
            {
                startTime = _timeCallStarted[senderNumber];
                _timeCallStarted.Remove(senderNumber);
            }

            var receiverNumber = senderNumber != phoneNumber ? phoneNumber : callResultReceiver;
            OnCallCompleted(new CallInfoEventArgs(senderNumber, receiverNumber, startTime, endTime));
        }

        private void ProcessRejectedCall(string phoneNumber, out string senderNumber,
            out string callResultReceiver, Dictionary<string, string> dictionary)
        {
            //sender reject a call
            if (dictionary.ContainsKey(phoneNumber))
            {
                senderNumber = phoneNumber;
                callResultReceiver = dictionary[phoneNumber];
            }
            // receiver reject a call
            else
            {
                senderNumber = dictionary.FirstOrDefault(x => x.Value == phoneNumber).Key;
                callResultReceiver = senderNumber;
            }

            dictionary.Remove(senderNumber);
        }

        private void DisposeTimer(string key)
        {
            _timersToAbortCall[key].Dispose();
            _timersToAbortCall.Remove(key);
        }

        public void ProcessAnsweredCall(string receiverPhoneNumber, DateTime startTime)
        {
            var senderNumber = _expectAnswer.FirstOrDefault(x => x.Value == receiverPhoneNumber).Key;

            if (_expectAnswer.ContainsKey(senderNumber))
            {
                DisposeTimer(senderNumber);
                _expectAnswer.Remove(senderNumber);

                _callsInProgress.Add(senderNumber, receiverPhoneNumber);
                _timeCallStarted.Add(senderNumber, startTime);

                OnOutgoingCallResult(new CallResultEventArgs(senderNumber, receiverPhoneNumber, CallResult.Answered));
            }
        }

        protected virtual void OnCallCompleted(CallInfoEventArgs e)
        {
            CallCompleted?.Invoke(this, e);
        }
    }
}