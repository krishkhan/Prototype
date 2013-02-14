
namespace BizApplication
{

    /// <summary>
    /// Contains a group of settings which describe how the application will be managed by the service.        
    /// 
    ///     SessionType :   see ApplicationSessionMode description.
    ///     
    ///     ResponseMode :   see ResponseMode description.
    ///     
    ///     UniqueApplicationName: every application have unique name.
    ///         for example: this property is use by the httpservice to find the correct application 
    ///         
    ///     SessionTimeToLiveSec: 
    ///         seconds of inactivity allow by the service before disposing
    ///         
    /// </summary>
    public abstract class ApplicationSettings
    {

        public ApplicationSettings()
        { 

        }
        
        //### Type of session
        public ApplicationSessionMode SessionType
        {
            get;set;
        }

        //### Behavior response
        public ApplicationResponseBehavior ResponseMode
        {
          get;set;
        }

        //### Unique application name
        public string UniqueApplicationName
        {
            get;set;
        }

        //### how many second of inactvity are allow
        public uint InactivityTimeToLive { get; set; }


        
    }
}
