
namespace BizApplication
{

    /// <summary>
    /// ApplicationSessionMode specify how the application live in the service.
    /// </summary>
    public enum ApplicationSessionMode
    {
        /// <summary>
        /// The application exist in only one instance in the service
        /// </summary>
        SingletonSession,
        /// <summary>
        /// The service generate as many application instances as each ip request
        /// </summary>
        IpSession,
        /// <summary>
        /// the service generate as many application instances as each browser request
        /// </summary>
        BrowserSession,

    }

    /// <summary>
    /// ApplicationResponseBehavior specify the behavior of the application response
    /// </summary>
    public enum ApplicationResponseBehavior
    {
        /// <summary>
        /// The response of each application istance are share with the other instances before sent to the client
        /// </summary>
        ShareAndSend,
        /// <summary>
        /// the response is immediately sent
        /// </summary>
        Send,
        /// <summary>
        /// the response is only shared
        /// </summary>
        OnlyShare

    }

     




}
