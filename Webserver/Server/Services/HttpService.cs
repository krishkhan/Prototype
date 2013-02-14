using System;
using ServerCommonLibrary;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using BizApplication;
using BizApplication.http;
using BizApplication.Http;

namespace Server.Services
{
    /// <summary>
    /// 
    /// HttpService provide request/response model for accesssing dataRef via http protocol,
    /// the requests are handled by custom applications that are dealt in session by the HttpApplicationManager class.    
    /// 
    /// </summary>
    /// <typeparam name="LOGGER"></typeparam>
    public class HttpService<LOGGER> : IServiceProvider_
        where LOGGER : IDebugger, new()
    {

        private LOGGER tracer;
        private HttpApplicationManager appManager;

        public HttpService()
        {         
            this.tracer = new LOGGER();
            this.appManager = new HttpApplicationManager();

        }

        /// <summary>
        /// Additional information for the provider
        /// </summary>
        /// <param name="port"></param>
        public void InitProvider(int port)
        {
            HttpApplicationManager.ServicePort = port;

            ///
            /// Server Settings 
            ///
            HttpApplicationManager.RootDirectory = Properties.Settings.Default.HttpServiceRoot;
            HttpApplicationManager.DefaultPage = Properties.Settings.Default.DefaultPage;
            HttpApplicationManager.CurrentDomain = Properties.Settings.Default.CurrentDomain;
            ParseApplicationXml();
        }

        /// <summary>
        /// !!!!!!!!!!!!!!!!!!!!!!!!!
        /// 
        ///  Main entry for all requests.
        ///  
        /// !!!!!!!!!!!!!!!!!!!!!!!!!
        ///         
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public RawResponse GetResponse(RawRequest req)
        {
            ///
            /// We first try to validate the request dataRef
            ///
            RawResponse service_output = null;
            HttpRequest httpreq = null;
            if (HttpApplicationManager.TryValidate(req, out httpreq))
            {
                this.tracer.trace("New request: " + httpreq.CompleteRequest);
                service_output = ManageHttpRequest(httpreq);
            }
            else
            {
                ///
                /// if the request is not valid we disconnect the client.
                ///
                this.tracer.trace("Invalid request.");
                service_output = new RawResponse(req) { Action = ResponseAction.Disconnect };
            }
            return service_output;
        }

        /// <summary>
        /// ManageHttpRequest forward the request to the correct session application and
        /// in case of exception send the custom page error filled with the error details. 
        ///         
        /// </summary>
        /// <param name="reqhttp"></param>
        /// <returns></returns>
        public RawResponse ManageHttpRequest(HttpRequest reqhttp)
        {
            ApplicationResponse output = null;
            try
            {
                ApplicationInstanceBase session = null;
                if (this.appManager.TryGetApplicationInstance(reqhttp, out session))
                {
                    output = session.ProcessRequest(reqhttp);
                    
                    if (output == null)
                        ///
                        /// application error
                        ///
                        throw new InvalidOperationException("Application " + reqhttp.Path + " not responding.");
                    if (reqhttp.Type == HttpRequestType.HttpPage)
                    {
                   
                        switch (session.Info.ResponseMode)
                        {
                            case ApplicationResponseBehavior.ShareAndSend:
                                ///
                                ///  We delivered the application response with others session
                                ///
                                this.appManager.ShareApplicationOutput(session, output, reqhttp);
                                break;
                        }
                    }
                }
                else
                {
                    
                    switch (reqhttp.Type)
                    {
                        case HttpRequestType.HttpPage:
                            if (reqhttp.Paths.Count > 0)
                            {
                                throw new InvalidOperationException("Application " + reqhttp.Path + " not exist");
                            }
                            else
                            {
                                output = HttpHelper.Generate404Page(reqhttp, "","Welcome :)","Server Home Page");
                            }
                            break;
                        case HttpRequestType.HttpStaticRequest:                            
                            output = this.appManager.ResponseStaticResource(reqhttp);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.tracer.trace("ERROR" + ex.Message + "::" + ex.StackTrace);
                output = HttpHelper.Generate404Page(reqhttp, "<b>"+ex.Message+"</b>::"+ex.StackTrace, "Error occured parsing " + reqhttp.Path);
            }
            return output;
        }

        protected void ParseApplicationXml()
        {
            try
            {
                ///
                /// The ApplicationXmlFile contains the informations for load the applications metadata
                ///    ApplicationXmlFile is composed in reocords structured as follow:
                ///     Application -> unique name  
                ///     Assembly -> dll file path that contains the application class
                ///     ApplicationSettingsClass -> the full name of the ApplicationSettings class (see ApplicationSettings)
                ///     ApplicationClass->the full name of the application class                 
                ///

                IList<ApplicationRecordInfo> applicationsToLoad = new List<ApplicationRecordInfo>();
                if (File.Exists(Properties.Settings.Default.ApplicationXmlFile))
                {
                    XmlTextReader reader = new XmlTextReader(Properties.Settings.Default.ApplicationXmlFile);
                    ApplicationRecordInfo app = null;
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "Application")
                                {
                                    app = new ApplicationRecordInfo();
                                    applicationsToLoad.Add(app);
                                }
                                if (reader.Name == "Name")
                                {
                                    reader.Read();
                                    app.Name = reader.Value;
                                }
                                if (reader.Name == "AssemblyPath")
                                {
                                    reader.Read();
                                    app.AssemblyPath = reader.Value;
                                }
                                if (reader.Name == "ApplicationSettingsClass")
                                {
                                    reader.Read();
                                    app.ApplicationSettingsClass = reader.Value;
                                }
                                if (reader.Name == "ApplicationClass")
                                {
                                    reader.Read();
                                    app.ApplicationClass = reader.Value;
                                }
                                break;
                        }
                    }
                    foreach (ApplicationRecordInfo info in applicationsToLoad)
                    {
                        try
                        {
                            if (File.Exists(info.AssemblyPath))
                            {
                                ///
                                /// We are ready to put the application metadata in the application menager using the AddHttpApplication method 
                                ///
                                Assembly commandAssembly = Assembly.UnsafeLoadFrom(info.AssemblyPath);
                                ApplicationSettings settings = (ApplicationSettings)commandAssembly.CreateInstance(info.ApplicationSettingsClass);
                                Type type = (Type)commandAssembly.GetType(info.ApplicationClass);
                                this.appManager.AddHttpApplication(new SessionManager(settings, type));
                                tracer.trace("Load Application: " + info.Name);
                            }
                            else {
                                tracer.trace("Application: " + info.Name+" not found in: "+info.AssemblyPath);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    tracer.trace("Applications xml page: " + Properties.Settings.Default.ApplicationXmlFile + " not found.");
                }
            }
            catch (Exception)
            {


            }
        }


        public void Dispose()
        {
            if (appManager != null)
            {
                appManager.Dispose();
                appManager = null;
            }
        }


    }
}
