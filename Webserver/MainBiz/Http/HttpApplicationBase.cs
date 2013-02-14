using System;
using System.Text;
using System.IO;
using ServerCommonLibrary;
using BizApplication.Http;

namespace BizApplication.http
{
    /// <summary>
    /// HttpApplicationBase give a baseclase to create web server applications.
    /// When new request is coming in the application try to find the resource looking in the application-directory then
    /// if is it marked 'HttpStaticRequest' the response is automatically sent back to the browser, if
    /// it's  marked 'HttpPage' the response is build in the PageLoad event.   
    /// HttpApplicationBase includes helper methods for common task.
    /// </summary>
    public abstract class HttpApplicationBase : ApplicationInstanceBase
    {
        /// <summary>
        /// return the physical path of resources
        /// </summary>
        /// <returns></returns>
        public abstract string ApplicationDirectory();

        //### Up casting of request type.
        public HttpRequest Request
        {
            get { return (HttpRequest)this.request; }
        }

        //### Up casting of response type
        public HttpResponse Response
        {
            get { return this.response as HttpResponse; }
            protected set { this.response = value; }
        }

        /// <summary>
        /// Requests entry point.
        /// </summary>
        protected override void NewRequest()
        {
            ///
            /// Firstable we check the request type,  
            /// If the type is 'HttpStaticRequest' we try to find the file joining the virtual 
            /// request path with the physical application directory and sending back the response, we throw an exception if the file not exist;
            /// this why we don't care when the browser demand files like css or js.
            /// If type is 'HttpPage' we try to find the file too, but forwarding after that the request into pageLoad() function,                        
            ///
            string absolutepath="";
            switch (Request.Type)
            {
                case HttpRequestType.HttpPage:
                  
                    string page = Request.Paths[Request.Paths.Count - 1];
                    ///
                    /// if the request end with '/' we assume index.html is omitted
                    /// 
                    if (Request.Path.EndsWith("/"))
                    {
                        page = "index.html";
                        Request.Path += page;
                        Request.Paths.Add(page);
                    }
                    ///
                    ///  Building the virtual path
                    ///
                    for (int i = 1; i < Request.Paths.Count - 1; i++) absolutepath += Request.Paths[i] + "//";
                    absolutepath += page;                    
                    if (File.Exists(ApplicationDirectory() + "\\" + absolutepath))
                    {                        
                        PageLoad(Request);
                        if (response == null)
                        {
                            ///
                            /// if no response is processed we try to find the file anyway
                            ///                            
                            BuildResponseFile(ApplicationDirectory() + "\\" + absolutepath, HttpHelper.GetResourceMime(page));                            
                        }
                    }
                    else
                    {                       
                        throw new InvalidOperationException("File Not Found");
                    }
                    break;
                case HttpRequestType.HttpStaticRequest:
                    ///
                    ///  Building the absolute file path
                    ///
                    string static_file=Request.Paths[Request.Paths.Count - 1];
                    for (int i = 1; i < Request.Paths.Count-1; i++) absolutepath += Request.Paths[1] + "//";
                    absolutepath += static_file;
                    if (File.Exists(ApplicationDirectory() + "\\" + absolutepath))
                    {
                        BuildResponseFile(ApplicationDirectory() + "\\" + absolutepath, HttpHelper.GetResourceMime(static_file));
                    }
                    else {
                        throw new InvalidOperationException("File Not Found");
                    }
                    break;
            }
        }

        /// <summary>
        /// This Events is triggered when a page loads.
        /// </summary>
        /// <param name="req"></param>
        protected abstract void PageLoad(HttpRequest req);

        /// <summary>
        /// This Events is triggered every time one session share a response.
        ///     Requiere ResponseMode = Share
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        /// <param name="req"></param>
        public override void OnNewShareResponse(ApplicationInstanceBase sender, ApplicationResponse response, ApplicationRequest req)
        {
        }

        /// <summary>
        /// Build response with the file data content  
        /// </summary>
        /// <param name="fullphysicalpath"></param>
        /// <param name="mime"></param>
        protected virtual void BuildResponseFile(string fullphysicalpath, MimeType mime)
        {
            byte[] binfile=Helper.GetFile(fullphysicalpath);
            if(binfile!=null)
                BuildResponse(binfile, mime, true);            
        }

        /// <summary>
        /// Build a response with generic data-string
        /// </summary>
        /// <param name="reponse"></param>
        /// <param name="dropConn"></param>
        protected virtual void BuildResponse(string reponse, bool dropConn = true)
        {
            BuildResponse(Encoding.UTF8.GetBytes(reponse), MimeType.text_html, dropConn);            
        }

        /// <summary>
        /// Build a response 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mime"></param>
        /// <param name="dropConn"></param>
        protected virtual void BuildResponse(byte[] data, MimeType mime, bool dropConn = true)
        {
            bool gzip = Request.isGZIPSupported();            
            data = gzip ? HttpHelper.CompressGZIP(data) : data;
            byte[] header = HttpHelper.GetHeader(data.Length, mime, dropConn, gzip);
            byte[] complete = header.Concat(data);
            this.response = new HttpResponse(complete, Request);
        }

        /// <summary>
        /// Invoked when the session is removed
        /// </summary>
        public override void UnloadApplication()
        {
        }

      

        

    }

}