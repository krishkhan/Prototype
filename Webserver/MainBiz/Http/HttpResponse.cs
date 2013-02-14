
namespace BizApplication.http
{
    /// <summary>
    /// Response sent back to web browser
    /// </summary>
    public class HttpResponse : ApplicationResponse
    {
        public HttpResponse(byte[] response,ApplicationRequest request)
            : base(request)
        {            
            this.ResponseData = response;
        }

        public override byte[] ElaborateData()
        {
            return base.ElaborateData();
        }
    }
}
