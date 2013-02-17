using System;
using System.Net.Sockets;
using System.Net;

namespace ServerCommonLibrary
{


    /// <summary>
    /// this class rappresent a socket connection 
    /// </summary>
    public class SocketConnection : EventArgs
    {

        public SocketConnection(Socket sock)
        {
            this.Socket = sock;

        }

        public void SendData(byte[] data)
        {
            if (Socket != null && Socket.Connected)
            {
                Socket.Send(data);
            }
        }


        //#### Socket instance
        public Socket Socket { get; set; }

        //#### IPAdress request from
        public IPAddress IP
        {
            get { return Socket == null ? null : ((IPEndPoint)Socket.RemoteEndPoint).Address; }
        }
    }
}
