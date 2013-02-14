using ServerCommonLibrary;

namespace Server
{
    /// <summary>
    /// Service provide request/response model for any kind of services.
    /// </summary>
    /// <typeparam name="PROVIDER">Elaborate the request and return a response</typeparam>
    /// <typeparam name="OUTPUTCHANNEL">Send the response to the client</typeparam>    
    public class Service<PROVIDER,OUTPUTCHANNEL> : IServiceBase
        where OUTPUTCHANNEL : IChannelOutput
        where PROVIDER : ServerCommonLibrary.IServiceProvider_
    {
 
        OUTPUTCHANNEL sender;
        PROVIDER provider;

        public Service(OUTPUTCHANNEL sender, PROVIDER provider)
        {
            this.sender = sender;
            this.provider = provider;            
        }

        public void ParseNewRequest(RawRequest e)
        {         
            sender.SendResponse(provider.GetResponse(e));
        }

        public void Dispose()
        {
           
            if (sender != null)
            {
                sender.Dispose();                
            }
            if (provider != null)
            {
                provider.Dispose();
            }
        }
    }
}
