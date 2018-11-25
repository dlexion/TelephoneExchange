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
                expectAnswer.Add(receiverPort, (Port)sender);

                OnCall(e, receiverPort);
            }
            else
            {
                //say that receiving number is busy
            }
        }

        public void ReportPortAboutCallReject(object sender, CallEventArgs e)
        {
            // is it right
            //var senderPort = ports.Find(x => x.PhoneNumber == e.SenderPhoneNumber);

            var receiverPort = (Port)sender;

            var senderPort = expectAnswer[receiverPort];

            expectAnswer.Remove(receiverPort);

            OnCallReject(e, senderPort);
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