using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yti.Yget.RemoteClient
{
    public interface ICommsChannel
    {
        Boolean Connect();

        Boolean Disconnect();

        Boolean Send(String data);

        Boolean Receive(out String data); 
    }
}
