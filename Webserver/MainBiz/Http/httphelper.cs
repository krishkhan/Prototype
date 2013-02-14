using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using BizApplication.http;
using ServerCommonLibrary;



namespace BizApplication.Http
{
    /// <summary>
    /// Generic helper functions for HTTP
    /// </summary>
    public class HttpHelper
    {

        /// <summary>
        /// Return true if the resource is part of the static resource group
        /// </summary>
        /// <param name="resource">resource name</param>
        /// <returns></returns>
        public static bool IsStaticResource(string resource)
        {
            return Regex.IsMatch(resource, @"(.*?)\.(ico|css|gif|jpg|jpeg|png|js|xml)$");
        }

        /// <summary>
        /// Return true if the resource is part of the dynamic resource group
        /// </summary>
        /// <param name="resource">resource name</param>
        /// <returns></returns>
        public static bool IsDynamicResource(string resource)
        {
            return Regex.IsMatch(resource, @"(.*?)\.(htm|html|xhtml|dhtml)$");
        }

        /// <summary>
        /// return Http 404 pageHeader in byte format.
        /// </summary>
        /// <param name="response_data_length"></param>
        /// <param name="response_data"></param>
        /// <returns></returns>
        public static byte[] GetHtml404Header(int response_data_length, MimeType type, string response_data = null)
        {
            return Encoding.UTF8.GetBytes(
                "HTTP/1.1 404 Not Found" +
                "\r\n" + "Date: " + String.Format("{0:r}", DateTime.Now) +
                "\r\n" + "Server: my" +
                (response_data != null ?
                "\r\n" + "Content-Length: " + response_data_length +
                "\r\n" + "Location: " + response_data +
                "\r\n" + "Content-Location: " + response_data
                : string.Empty) +
                "\r\n" + "Content-Type: "+GetStringMimeType(type)+
                "\r\n\r\n");
        }

        /// <summary>
        /// Return a tiny Http response pageHeader in bytes format.        
        /// </summary>
        /// <param name="lenght"></param>
        /// <param name="type"></param>
        /// <param name="dropConnection"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static byte[] GetHeader(int lenght, MimeType type, bool dropConnection, bool gzip)
        {
            string cutOffConnection = "";
            if (dropConnection)
            {
                cutOffConnection = "\r\n" + "Connection: close";
            }
            string _type = GetStringMimeType(type);

            string header = "HTTP/1.1 200 OK" +
             "\r\n" + "Cache-Control: private" +
             "\r\n" + "Server: my" +
             "\r\n" + "Content-Type: " + _type +
             "\r\n" + "Content-Length: " + lenght +
             (gzip ? "\r\n" + "Content-Encoding: gzip" : "") +
             "\r\n" + "Server: vws" +
             "\r\n" + "Date: " + String.Format("{0:r}", DateTime.Now) +
                // _trunoffConnection +
                //"\r\n" + "Last-Modified : " + System.DateTime.Now + " GMT"+
             "\r\n\r\n";
            return Encoding.UTF8.GetBytes(header);
        }

        /// <summary>
        /// This function return a byte array compressed by GZIP algorithm.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CompressGZIP(byte[] data)
        {
            System.IO.MemoryStream streamoutput = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(streamoutput, System.IO.Compression.CompressionMode.Compress, false);
            gzip.Write(data, 0, data.Length);
            gzip.Close();
            return streamoutput.ToArray();
        }

        /// <summary>
        /// This function return the default error page as Application Response.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ApplicationResponse Generate404Page(HttpRequest request, string bodyMessage,string pageHeader,string title="Error Page")
        {
            //Check if the defaultPage exist
            if (!File.Exists(HttpApplicationManager.RootDirectory + "\\" + HttpApplicationManager.DefaultPage))
            {
                //No data to sent back so the connection will be close.
                return new ApplicationResponse(request) { Action = ResponseAction.Disconnect };
            }
            //Get the file
            byte[] page = Helper.GetFile(HttpApplicationManager.RootDirectory + "\\" + HttpApplicationManager.DefaultPage);
            string page_str = new String(Encoding.UTF8.GetChars(page));
            //fill the page with exception information
            page_str = page_str.Replace("<%ws_title%>", title);
            page_str = page_str.Replace("<%ws_domain%>", HttpApplicationManager.CurrentDomain + ":" + HttpApplicationManager.ServicePort);
            page_str = page_str.Replace("<%ws_header%>", pageHeader);
            page_str = page_str.Replace("<%ws_message%>", bodyMessage);
            page = Encoding.UTF8.GetBytes(page_str);
            //Get the pageHeader
            byte[] binheader = GetHeader(page.Length,MimeType.text_html,true,false);
            //build the response
            byte[] completeResponse = binheader.Concat(page);            
            ApplicationResponse response = new HttpResponse(completeResponse, request);
            return response;
        }

        /// <summary>
        /// Extract the MimeType from a resource
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MimeType GetResourceMime(string request)
        {
            if (string.IsNullOrEmpty(request)) return MimeType.none;
            string[] filepats = request.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (filepats.Length > 0)
            {
                string extension = filepats[filepats.Length - 1];
                return GetContentTypeByExtension(extension);
            }
            else
                return MimeType.none;
        }


        public static string GetStringMimeType(MimeType mime)
        {
            string strMime = "";
            switch (mime)
            {
                case MimeType.text_html: strMime = "text/html; charset=utf-8"; break;
                case MimeType.text_javascript: strMime = "text/javascript"; break;
                case MimeType.multipart_xmixedreplace:
                    strMime = "multipart/x-mixed-replace; boundary=rnA00A"; break;
                case MimeType.application_xml_charsetutf8:
                    strMime = "application/xml; charset=utf-8";
                    break;
                default:
                    strMime = mime.ToString().Replace("_", "/");
                    break;
            }
            return strMime;
        }

        public static MimeType GetContentTypeByExtension(string extension)
        {
            switch (extension)
            {
                case "css":
                    return MimeType.text_css;
                case "gif":
                    return MimeType.image_gif;
                case "jpg":
                case "jpeg":
                    return MimeType.image_jpeg;
                case "ico":
                case "png":
                    return MimeType.image_png;
                case "htm":
                case "html":
                case "xhtml":
                case "dhtml":
                    return MimeType.text_html;
                case "js":
                    return MimeType.text_javascript;
                case "xml":
                    return MimeType.multipart_xmixedreplace;
                default:
                    return MimeType.none;
            }
        }

        public static string CleanJsonString(string url)
        {
            if (url == null) return null;
            return url.Replace("%27", "\"").Replace("%35", "#").Replace("%20", " ").Replace("%61", "=").Replace("%63", "?");
        }

        public static string[] GetUrlQueries(string queryUrlString)
        {
            return queryUrlString.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
        }

        #region Custom

        /*
         * In my demos each asynchronous ajax request have a token like : "__minute_millisecond" at the end of the string, for avoid browser cache
         */

        public static string RemoveToken(string completeReq)
        {
            if (completeReq.Contains("__"))
            {
                completeReq = completeReq.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            string[] _pReq = completeReq.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries);
            if (_pReq.Length < 2) return string.Empty;
            else return _pReq[1];
        }

        
        #endregion
    }
}
