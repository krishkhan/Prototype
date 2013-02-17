
namespace ServerCommonLibrary
{
    /// <summary>
    /// RawRequest holds the informations about a request 
    /// </summary>
    public class RawRequest
    {
        //#### Client connection 
        public SocketConnection Connection { get; set; }
        //#### the request data
        public byte[] RawData { get; set; }
    }
}
