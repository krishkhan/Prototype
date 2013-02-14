using System;
using AsyncSocketSample;
using ServerCommonLibrary;
using System.Net;

namespace ComunicationLayer
{
    /// <summary>
    /// This class is designed to provide abastraction of socketengine implementation ,expose IServerOuput interface for send dataRef to the web browser/client
    /// and accept IServerBase interface for comunicate input requests with the service.    
    /// </summary>
    public class SocketComunicator : IDisposable, IChannelOutput
    {

        //### socket engine component
        Server sockengine;

        //### server input handler
        IServiceBase serviceInterface;

        public SocketComunicator()
        {
            this.sockengine = new Server(10, 2048);
            ManageSocketEngineEvents(true);
            sockengine.Init();
        }


        /// <summary>
        /// Start listening at the specified port
        /// </summary>
        /// <param name="listenPort"></param>
        public void StartListen(int port)
        {
            this.sockengine.Start(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
        }

        /// <summary>
        /// Set the service interface
        /// </summary>
        /// <param name="wb"></param>
        public void SetServiceHandler(IServiceBase wb)
        {
            this.serviceInterface = wb;
        }

        /// <summary>
        /// New message incoming from the NET
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="dataRef"></param>
        void Socket_OnNewDataReceived(object sender, SocketConnection e, byte[] dataRef)
        {
            if (this.serviceInterface != null)
                this.serviceInterface.ParseNewRequest(new RawRequest() { Connection = e, RawData = dataRef });
        }



        protected void ManageSocketEngineEvents(bool add)
        {
            if (add) sockengine.OnNewDataReceived += new Server.SocketConnectionEventHandler(Socket_OnNewDataReceived);
            else sockengine.OnNewDataReceived -= (Socket_OnNewDataReceived);

        }

        /// <summary>
        /// Send a response through the socket
        /// </summary>
        /// <param name="e"></param>
        public void SendResponse(RawResponse e)
        {
            try
            {
                switch (e.Action)
                {
                    case ResponseAction.Disconnect:
                        e.Request.Connection.Socket.Disconnect(false);
                        break;
                    case ResponseAction.Send:
                        int sent = e.Request.Connection.Socket.Send(e.ElaborateData());
                        break;
                    case ResponseAction.Skip:
                        break;
                }
            }
            catch
            {

            }
        }
        
        public void Dispose()
        {
            if (sockengine != null)
            {
                //It's important remove handlers
                ManageSocketEngineEvents(false);
                //sockengine.Dispose(); MS in his example never implement the dispose, bad habit!!.
                sockengine = null;
            }
        }
    }
}
