﻿
namespace ServerCommonLibrary
{
    
    public enum ResponseAction{ 
        /// <summary>
        /// Send the response data 
        /// </summary>
        Send, 
        /// <summary>
        /// Do nothing
        /// </summary>
        Skip,
        /// <summary>
        /// Disconnect the connection
        /// </summary>
        Disconnect }


    /// <summary>
    /// Response generated by the service is sent back to client
    ///  - prefix Raw because manage low level format data
    /// </summary>
    public class RawResponse
    {
        /// <summary>
        /// - costructor -
        /// </summary>
        /// <param name="req">initial request</param>
        public RawResponse(RawRequest req)
        {
            this.Request = req;
        }

        //#### initial request 
        public RawRequest Request { get; set; }
        //#### response action 
        public ResponseAction Action { get; set; }
        //#### response data
        public byte[] ResponseData { get; set; }

        public virtual byte[] ElaborateData()
        {
            return this.ResponseData;
        }
    }
}
