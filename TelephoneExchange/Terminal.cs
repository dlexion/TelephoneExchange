using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelephoneExchange.EventsArgs;

namespace TelephoneExchange
{
    public class Terminal
    {
        private Port _port;

        private bool _isConnected = false;

        private bool _isRinging;

        public Action<string> Log { get; set; } = null;

        public event EventHandler<IncomingCallEventArgs> IncomingCall;

        public void NotificationAboutIncomingCall(object sender, IncomingCallEventArgs e)
        {
            Log?.Invoke($"{e.SenderPhoneNumber} is calling {_port.PhoneNumber}");

            OnIncomingCall(e);
        }

        public void Call(string number)
        {
            _port.Call(number);
        }

        // decline incoming call 
        public void Decline()
        {

        }

        // reject outgoing or call in progress
        public void Reject()
        {

        }

        public void ConnectToPort(Port port)
        {
            if(_isConnected)
                return;

            _port = port;
            _isConnected = true;

            // TODO subscribe for necessary events
            _port.Incoming += NotificationAboutIncomingCall;
            _port.ConnectWithTerminal();
        }

        public void DisconnectFromPort()
        {
            if(!_isConnected)
                return;

            _port.Incoming -= NotificationAboutIncomingCall;
            _port.ConnectWithTerminal();

            _port = null;
            _isConnected = false;
        }

        protected virtual void OnIncomingCall(IncomingCallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }
    }
}
