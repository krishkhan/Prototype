using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yti.Yget.Communication
{
    public interface ICommsChannel
    {
        Boolean Connect();

        Boolean Disconnect();

        String Tranceive(out String data); 


    }
}
