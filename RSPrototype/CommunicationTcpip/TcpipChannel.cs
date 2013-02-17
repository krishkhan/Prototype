using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yti.Yget.Communication;     

namespace Yti.Yget.Communication
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

        public string Tranceive(out string data)
        {
            throw new NotImplementedException();
        }
    }
}
