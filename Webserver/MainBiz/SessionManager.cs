using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace BizApplication
{
    /// <summary>
    /// SessionManager handle the sessions of one application type, expose method to create new istances.    
    /// 
    /// </summary>
    public class SessionManager
    {
        //### Application Info
        private ApplicationSettings info;
        //### Application Type 
        private Type applicationType;
        //### Contains all instances Key: SessionId/ Value: ApplicationInstance
        private ConcurrentDictionary<string, ApplicationInstanceBase> sessionDictionary;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">application settings</param>
        /// <param name="applicationType">The type of application</param>
        public SessionManager(ApplicationSettings settings, Type applicationType)
        {
            this.info = settings;
            this.applicationType=applicationType;
            this.sessionDictionary = new ConcurrentDictionary<string, ApplicationInstanceBase>();
        }

        /// <summary>
        /// Create a Session
        /// </summary>
        /// <returns></returns>
        protected ApplicationInstanceBase GetNewInstance()
        {         
            ApplicationInstanceBase app=(ApplicationInstanceBase)Activator.CreateInstance(this.applicationType);
            return app;
        }

        /// <summary>
        /// Get or create Singleton Instance
        /// </summary>
        /// <returns></returns>
        public ApplicationInstanceBase GetOrCreateSingletonInstance()
        {
            ///
            ///  Return or create if miss, one singleton session 
            ///
            ApplicationInstanceBase app = null;
            if (sessionDictionary.Count == 0)
            {            
                app = (ApplicationInstanceBase)Activator.CreateInstance(this.applicationType);                
                sessionDictionary.TryAdd("singleton", app);
            }
            else                
                app = sessionDictionary["singleton"];
            return app;
        }

        /// <summary>
        /// Get or create a new session by sessionKey
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ApplicationInstanceBase GetOrCreateInstanceBySessionKey(string sessionKey)
        {            
            if (sessionDictionary.ContainsKey(sessionKey))
            {                
                return sessionDictionary[sessionKey];
            }            
            ApplicationInstanceBase newSession = GetNewInstance();
            sessionDictionary.TryAdd(sessionKey, newSession);
            return newSession;
        }

        /// <summary>
        /// Get the sessions
        /// </summary>
        public ConcurrentDictionary<string, ApplicationInstanceBase> SessionList
        {
            get { return sessionDictionary; }
        }

        /// <summary>
        /// Get the applicatoin info
        /// </summary>
        public ApplicationSettings Info
        {
            get { return info; }
        }

        /// <summary>
        /// Unload expired sessions
        /// </summary>
        public void UnloadExpiredInstances()
        {

            ///
            /// Check how many seccond are elapsed from the last activity
            ///
            
            IList<ApplicationInstanceBase> applications = SessionList.Values.ToList();
            ///
            /// Max inactivity seconds
            ///
            uint ttl = Info.InactivityTimeToLive;
            
            foreach (ApplicationInstanceBase app in applications)
            {                
                if ((DateTime.Now - app.LastRequest).TotalSeconds > ttl)
                {             
                    ApplicationInstanceBase _app=null;            
                    string sessionkey=sessionDictionary.First(x => x.Value.ApplicationId == app.ApplicationId).Key;            
                    if (sessionDictionary.TryRemove(sessionkey, out _app))
                    {
                        ///
                        /// Call Unload event for ralease memory
                        /// 
                        _app.UnloadApplication();
                        _app = null;
                        System.Diagnostics.Debug.Write("Application instance:  " + sessionkey + " is disposed");
                    }
                }
            }
        }
    }
}
