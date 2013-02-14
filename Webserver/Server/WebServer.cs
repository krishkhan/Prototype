using System;
using ServerCommonLibrary;
using System.Collections.Concurrent;
using ComunicationLayer;

namespace Server
{

    /// <summary>
    /// WebServer can host many services in differents listen ports
    /// </summary>
    /// <typeparam name="TRACER"></typeparam>
    public class WebServer<TRACER> : IDisposable
        where TRACER : IDebugger, new()
    {

        //### services dictionary
        ConcurrentDictionary<int, IServiceBase> services;
        //### logger
        TRACER tracer;


        public WebServer()
        {
            this.tracer = new TRACER();
            this.services = new ConcurrentDictionary<int, IServiceBase>();
        }

        /// <summary>
        /// Add a Service in the server
        /// </summary>
        /// <typeparam name="PROVIDER">The new provider</typeparam>
        /// <param name="port"></param>
        public void AddService<SERVICE>(int port)
            where SERVICE : ServerCommonLibrary.IServiceProvider_, new()
        {
            ///
            /// Firstable we check if the listen port is free, 
            /// then we create a new In_Out_channel and an instance of provider host that bind the PROVIDER with the In_Out_channel.
            ///
            if (services.ContainsKey(port))
            {
                tracer.trace("Service can't start beacuse the listen port " + port + " already used");
                return;
            }
            ///
            /// New Listenter
            ///
            SocketComunicator In_Out_channel = new SocketComunicator();
            SERVICE service = new SERVICE();
            Service<SERVICE, SocketComunicator> servicehost = new Service<SERVICE, SocketComunicator>(In_Out_channel, service);
            service.InitProvider(port);
            In_Out_channel.SetServiceHandler(servicehost);
            In_Out_channel.StartListen(port);

            ///
            /// Add the host in the dictionary
            ///
            services.TryAdd(port, servicehost);
            ///
            /// StartListen Listen
            ///
     
            tracer.trace("Service started @ listen port " + port);

        }

        public void Dispose()
        {
            if (services != null)
            {
                foreach (var item in services) item.Value.Dispose();
                services.Clear();
                services = null;
            }
        }
    }
}

