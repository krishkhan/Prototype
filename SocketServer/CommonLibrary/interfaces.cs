using System;

namespace ServerCommonLibrary
{

    /// <summary>
    /// Service input interface 
    /// </summary>
    public interface IServiceBase:IDisposable
    {
        void ParseNewRequest(RawRequest e);
    }

    /// <summary>
    /// server output interface  
    /// </summary>
    public interface IChannelOutput : IDisposable
    {
        void SendResponse(RawResponse e);
    }

    /// <summary>
    /// Provider interface
    /// </summary>
    public interface IServiceProvider_ : IDisposable
    {
        
        RawResponse GetResponse(RawRequest req);
        void InitProvider(int listenPort);        
    }

    /// <summary>
    /// Debugger interface
    /// </summary>
    public interface IDebugger
    {
        void trace(string log);
    }
}
