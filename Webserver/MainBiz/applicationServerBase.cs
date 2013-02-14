using System;

namespace BizApplication
{
    /// <summary>
    /// Base class to build server side applications.
    /// Contains methods for interact with the server and for share data.    
    /// </summary>
    public abstract class ApplicationInstanceBase
    {
        //#### Declaration        
        private Guid applicationId;
        protected ApplicationResponse response;
        protected ApplicationRequest request;

        //#### Constructor
        public ApplicationInstanceBase()
        {
            this.applicationId = Guid.NewGuid();
        }

        /// <summary>
        /// Entry point for all client requests.        
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApplicationResponse ProcessRequest(ApplicationRequest req)
        {
            ///
            /// Firstable we clean the application data for the new request            
            /// 
            
            this.request = req;            
            this.response = null;            
            LastRequest = DateTime.Now;
      
            NewRequest();
            
            
            return this.response;
        }
       
        protected abstract void NewRequest();
        
        /// <summary>
        /// Incomging shared response from an application 
        /// </summary>
        /// <param name="sender">the application that elaborate the response</param>
        /// <param name="response"></param>
        /// <param name="req">the initial request</param>
        public abstract void OnNewShareResponse(ApplicationInstanceBase sender, ApplicationResponse response, ApplicationRequest req);

        /// <summary>
        /// Called by the server when disposed
        /// </summary>
        public abstract void UnloadApplication();

        //#### Return the application setting
        public abstract ApplicationSettings Info { get; }

        //#### Unique identifier application Id
        public Guid ApplicationId{get { return applicationId; } }

        //#### Last datetime request 
        public DateTime LastRequest { get; set; }
    }
}
