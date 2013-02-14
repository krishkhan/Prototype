using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerCommonLibrary;

namespace BizApplication
{

    /// <summary>
    /// Contains high level application request.
    /// </summary>
    public class ApplicationResponse : RawResponse
    {
        protected ApplicationRequest apprequest;

        public ApplicationResponse(ApplicationRequest request)
            :base(request.Rawrequest)
        {
            this.apprequest = request;
        }

        //### Application request
        public ApplicationRequest AppRequest
        {
            get { return apprequest; }
        }
    }
}
