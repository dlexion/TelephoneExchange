using System;
using System.Collections.Generic;
using System.Linq;
using TelephoneExchange.Enums;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Station
    {
        List<Port> ports = new List<Port>();

        //List<Port> expectAnswer = new List<Port>();

        Dictionary<Port, Port> expectAnswer = new Dictionary<Port, Port>();

        Dictionary<Port, Port> callInProgress = new Dictionary<Port, Port>();

        public event EventHandler<CallEventArgs> Call;

        public event EventHandler<CallEventArgs> CallReject;

        public Station(List<Port> ports)
        {
            this.ports = ports;
        }

        public void ReportPortAboutIncomingCall(object sender, CallEventArgs e)
        {
            var receiverPort = ports.Find(x => x.PhoneNumber == e.ReceiverPhoneNumber);

            if (receiverPort.State == PortState.Online)
            {
                //mb switch places
                expectAnswer.Add((Port)sender, receiverPort);

                OnCall(e, receiverPort);
            }
            else
            {
                //say that receiving number is busy
            }
        }

        // TODO refactor if states and add event for billing
        public void ReportPortAboutCallReject(object sender, CallEventArgs e)
        {
            var receiverPort = (Port)sender;
            Port senderPort = null;
            //var senderPort = expectAnswer[receiverPort];

            //expectAnswer.Remove(receiverPort);

            //OnCallReject(e, senderPort);var myKey = types.FirstOrDefault(x => x.Value == "one").Key;

            if (expectAnswer.ContainsKey(receiverPort))
            {
                senderPort = expectAnswer[receiverPort];

                expectAnswer.Remove(receiverPort);

                e.SenderPhoneNumber = receiverPort.PhoneNumber;
            }
            else if (expectAnswer.ContainsValue(receiverPort))
            {
                // add check for default
                senderPort = expectAnswer.FirstOrDefault(x => x.Value == receiverPort).Key;

                expectAnswer.Remove(senderPort);

                e.SenderPhoneNumber = senderPort.PhoneNumber;
            }
            // TODO notify billing system in these cases
            else if (callInProgress.ContainsKey(receiverPort))
            {
                senderPort = callInProgress[receiverPort];

                callInProgress.Remove(receiverPort);

                e.SenderPhoneNumber = receiverPort.PhoneNumber;
            }
            else
            {
                senderPort = callInProgress.FirstOrDefault(x => x.Value == receiverPort).Key;

                callInProgress.Remove(senderPort);

                e.SenderPhoneNumber = senderPort.PhoneNumber;
            }


            OnCallReject(e, senderPort);
        }

        public void ReportPortAboutCallAnswer(object sender, CallEventArgs e)
        {
            var senderPort = expectAnswer.FirstOrDefault(x => x.Value == (Port)sender).Key;

            expectAnswer.Remove(senderPort);

            callInProgress.Add(senderPort, (Port)sender);
        }

        protected virtual void OnCall(CallEventArgs e, Port target)
        {
            // TODO check and create method not to duplicate code
            (Call?.GetInvocationList().First(x => x.Target == target) as EventHandler<CallEventArgs>)?.Invoke(this, e);
            //Call?.Invoke(this, e);
        }

        protected virtual void OnCallReject(CallEventArgs e, Port target)
        {
            (CallReject?.GetInvocationList().First(x => x.Target == target) as EventHandler<CallEventArgs>)?.Invoke(this, e);
        }
    }
}