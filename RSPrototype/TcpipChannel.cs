using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yti.Yget.RemoteClient
{
    class TcpipChannel: ICommsChannel 
    {
        private TcpipChannel _tcpipChannel; 

        public ICommsChannel CreateInstance(String channelName)
        {
            return _tcpipChannel;
        }

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool Send(string data)
        {
            throw new NotImplementedException();
        }

        public bool Receive(out string data)
        {
            throw new NotImplementedException();
        }
    }
}
