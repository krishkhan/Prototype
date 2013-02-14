using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

using ServerCommonLibrary;
using BizApplication.http;



namespace BizApplication.Http
{
    /// <summary>
    /// 
    /// HttpApplicationManager generate and keep alive sessions checking how many of them are expired, 
    /// contains method for validate an http request and determinates which application match with the request.
    ///     
    ///    
    /// </summary>
    public class HttpApplicationManager : IDisposable
    {

        //### Current server domain
        public static string CurrentDomain;
        //### Physical directory
        public static string RootDirectory;
        //### Default page
        public static string DefaultPage;
        //### Service Port
        public static int ServicePort;

        //### Session Dictionary  Key: ApplicationUniqueName/ Value: SessionManager
        ConcurrentDictionary<string, SessionManager> applications;

        //### Timer 
        Timer managerTimer;

        public HttpApplicationManager()
        {
            applications = new ConcurrentDictionary<string, SessionManager>();
            managerTimer = new Timer(ManagerTimerMain, applications, 1000, 1000);
        }

        /// <summary>
        /// Try to add an empty session manager 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public bool AddHttpApplication(SessionManager app)
        {
            if (applications.ContainsKey(app.Info.UniqueApplicationName)) 
                return false;
            return applications.TryAdd(app.Info.UniqueApplicationName, app);
        }

        /// <summary>
        /// The timer periodically check expired sessions 
        /// </summary>
        /// <param name="e"></param>
        protected void ManagerTimerMain(object e)
        {
            ConcurrentDictionary<string, SessionManager> manager = (ConcurrentDictionary<string, SessionManager>)e;
            foreach (var sessionMgr in manager.Values)
            {                
                //### check expired session
                sessionMgr.UnloadExpiredInstances();
            }
        }

        /// <summary>
        /// Return an application istance that correspond with the request.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="application"></param>
        /// <returns>false if not exist</returns>
        public bool TryGetApplicationInstance(HttpRequest e, out ApplicationInstanceBase application)
        {
            ///
            /// The logic to get the right application from a request is very simple,
            /// For identify the application we look the main path in the url, is the first after the domain domain.xxx ex:
            /// http://domain.xxc/maindir/dir1/dir2 so, dir1/dir2 are internal paths relatively at the main path maindir.
            /// The rule is the main path indicate the name of the application, so
            /// http://localhost:8080/chat/index.html identify the chat application 
            /// http://localhost:8080/chat/chat2/index.html identify the chat application 
            ///             
            ///

            application = null;
            
            string[] paths = e.Path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
           
            if (paths.Length == 0) return false;
           
            string mainPath = paths[0];
            ///
            /// Immediately check if exist an ApplicationUniqueName equal to mainpath
            ///
            if (!applications.ContainsKey(mainPath)) return false;
            //Ok the applicatoin Exist!
            
            SessionManager sessionMgr = applications[mainPath];
            
            ApplicationSettings settings = sessionMgr.Info;
            string sessionKey = string.Empty;
            ///
            /// The SessionManager is found now we check if already exist any session
            ///
            switch (settings.SessionType)
            {
                case ApplicationSessionMode.SingletonSession:
                    ///
                    /// SingletonSession 
                    ///
                    application = sessionMgr.GetOrCreateSingletonInstance();
                    return true;
                case ApplicationSessionMode.BrowserSession:
                    ///
                    /// We need a session key to identfy one particolare browser 
                    ///                    
                    sessionKey = e.Rawrequest.Connection.IP + "@" + e.Requests["User-Agent"];
                    break;
                case ApplicationSessionMode.IpSession:
                    ///
                    /// We need a session key to identfy one Ip address
                    ///                    
                    sessionKey = e.Rawrequest.Connection.IP.ToString();
                    break;
            }
           
            application = sessionMgr.GetOrCreateInstanceBySessionKey(sessionKey);

            return true;
        }

        /// <summary>
        /// Dispatch a response to the other sessions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="req"></param>
        public void ShareApplicationOutput(ApplicationInstanceBase sender, ApplicationResponse e, HttpRequest req)
        {
            //### verify if application exist
            if (applications.ContainsKey(sender.Info.UniqueApplicationName))
            {
                //### Session manager cointains all instances
                SessionManager session = applications[sender.Info.UniqueApplicationName];                
                foreach (var item in session.SessionList)
                {
                    //### Skip the sender
                    if (item.Value.ApplicationId != sender.ApplicationId)
                    {
                        item.Value.OnNewShareResponse(sender, e, req);
                    }
                }
            }
        }

        /// <summary>
        /// This method try to find a resource in a ResourceDirectory path and return a response.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApplicationResponse ResponseStaticResource(HttpRequest request)
        {
            string resource = request.Path;
            ApplicationResponse output = null;
            if (resource[0] != '/') resource = "/" + resource;
            //Get resource mime type
            MimeType type = HttpHelper.GetResourceMime(resource);
            //Check if Gzip compression is enable
            bool gzip = request.isGZIPSupported();
            //Get a file
            byte[] file = Helper.GetFile(RootDirectory + request.Path);
            if (file != null)
            {
                //compress data id Gzip is supported
                file = gzip ? HttpHelper.CompressGZIP(file) : file;
                //get a pageHeader
                byte[] header = HttpHelper.GetHeader(file.Length, type, true, gzip);
                //build the complete response
                byte[] response = header.Concat(file);
                //create the response
                output = new HttpResponse(response, request);
            }
            else
            {
                //file not found: return 404 response
                output = new HttpResponse(HttpHelper.GetHtml404Header(0, type), request);
            }
            return output;
        }

        /// <summary>
        /// This function check if the RawRequest be received from the browser is well formed.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool TryValidate(RawRequest e, out HttpRequest req)
        {
            HttpRequestType request = HttpRequestType.HttpPage;
            try
            {
                ///
                /// There are a lot of tecnique to extract the information in an http browser request,
                /// this solutions split the request string and analize each blocks.
                /// 

                req = new HttpRequest(e);
                ///
                ///Decode the bytes in Utf8 chars and Split the string request by "\r\n" 
                ///
                req.CompleteRequest = new String(Encoding.UTF8.GetChars(e.RawData));
                
                string[] groups = req.CompleteRequest.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < groups.Length; i++)
                {
                    string headerblock = groups[i];

                    if (i > 0 && headerblock.Contains(":"))
                    {
                        //From the second block we have fileds with the pattern <name:value>
                        string[] block = headerblock.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        req.Requests.Add(block[0], block[1]);
                    }
                    else
                    {
                        ///
                        /// The first block always include the request path ,the method and the protocol http.
                        /// ex. GET /path/resource.ext HTTP/1.1 
                        /// 

                        
                        string[] subparts = headerblock.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        //copy the path 
                        req.CompletePath = subparts[1];                        
                        if (subparts[0] == "GET") req.Method = HttpMethodType.Get;
                        if (subparts[0] == "POST") req.Method = HttpMethodType.Post;                        
                        if (!String.IsNullOrEmpty(subparts[1]))
                        {
                            //split the path in "directories"
                            string[] resourcePaths = subparts[1].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                            if (resourcePaths.Length > 0)
                            {
                                ///
                                /// We make a distinction between HttpStaticRequest and HttpPage 
                                ///
                                
                                string resourceFullName = resourcePaths[resourcePaths.Length - 1];
                                
                                if (HttpHelper.IsStaticResource(resourceFullName))
                                {
                                    request = HttpRequestType.HttpStaticRequest;
                                }
                            }
                        }
                        else throw new InvalidOperationException("Invalid Request : " + req.CompleteRequest);

                        ///
                        /// separate the request path from possibly query-url 
                        /// 
                        string[] subPaths = subparts[1].Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries);
                        
                        req.Path = subPaths[0];                        
                        req.Paths = subPaths[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                }

                //Save the query url parts in a dictionary : req.UrlParameters
                string[] queryparams = HttpHelper.GetUrlQueries(HttpHelper.RemoveToken(req.CompletePath));
                foreach (string p in queryparams)
                {
                    string[] query = p.Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (query.Length == 2)
                    {
                        req.UrlParameters.Add(query[0], query[1]);
                    }
                }
            }
            catch (Exception)
            {
                //If somethig goes wrong the validation return false.
                req = null;
                return false;
            }
            req.Type = request;
            return true;
        }

        public void Dispose()
        {
            if (managerTimer != null)
            {
                managerTimer.Change(-1, -1);
                managerTimer.Dispose();
                managerTimer = null;
            }
        }


    }
}
