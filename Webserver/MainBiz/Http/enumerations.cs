
namespace BizApplication.Http
{

    /// <summary>
    /// The server mark the request as one of this two types: 
    ///     - HttpPage
    ///     - HttpStaticRequest
    /// Each HttpStaticRequest is elaborate automaticaly by HttpServerApplication,
    /// instead HttpPage request require a custom elaboration process by the application.
    /// </summary>
    public enum HttpRequestType { HttpPage, HttpStaticRequest }

    /// <summary>
    /// Http methods
    /// </summary>
    public enum HttpMethodType { Get, Post }

    /// <summary>
    /// http://en.wikipedia.org/wiki/Mime_type
    /// </summary>
    public enum MimeType
    {        
        text_html,
        text_javascript,
        text_css,
        multipart_xmixedreplace,
        application_xml_charsetutf8,
        image_gif,
        image_jpeg,
        image_png,
        none
    }
}
