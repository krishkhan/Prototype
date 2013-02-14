using System.Collections.Generic;
using ServerCommonLibrary;
using BizApplication.Http;

namespace BizApplication.http
{
   
    /// <summary>
    /// Contains server http request information
    /// </summary>
    public class HttpRequest : ApplicationRequest
    {

        protected HttpRequestType type;

        public HttpRequest(RawRequest request)
            : base(request)
        {
            type = HttpRequestType.HttpPage;
            Requests = new Dictionary<string, string>();
            UrlParameters = new Dictionary<string, string>();
        }

        //### Http Method Get/Post
        public HttpMethodType Method { get; set; }
        //### Http Type
        public HttpRequestType Type
        {
            get { return type; }
            set { type = value; }
        }

        //### Http Complete request ex: 
        public string CompleteRequest { get; set; }
        //### Request Full Path
        public string CompletePath { get; set; }
        //### Request Path
        public string Path { get; set; }
        //### Request Paths
        public IList<string> Paths { get; set; }
        //### Http Requests 
        public Dictionary<string, string> Requests;
        //### Query Url parameters
        public Dictionary<string, string> UrlParameters;        
        /// <summary>
        /// return true if Gzip compression is supported
        /// </summary>
        /// <returns></returns>
        public bool isGZIPSupported()
        {
            return (Requests != null && Requests.ContainsKey("Accept-Encoding") && Requests["Accept-Encoding"].Contains("gzip")) ? true : false;
        }
        /// <summary>
        /// Return query strin value, return "" if not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetQueryStringValue(string key)
        {
            if (UrlParameters == null || UrlParameters.Count == 0 || !UrlParameters.ContainsKey(key)) return string.Empty;
            return UrlParameters[key];
        }

    }
}
