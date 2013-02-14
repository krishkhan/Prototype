using ServerCommonLibrary;

namespace BizApplication
{

    /// <summary>    
    /// RawRequest wrapper
    /// </summary>
    public abstract class ApplicationRequest
    {

        
        private RawRequest rawrequest;
 
        public ApplicationRequest(RawRequest request)
        {
            this.rawrequest = request;
        }

        //### RawRequest
        public RawRequest Rawrequest
        {
            get { return rawrequest; }
        }
    }
}
