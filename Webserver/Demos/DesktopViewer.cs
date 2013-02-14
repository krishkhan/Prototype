using System;
using BizApplication;
using BizApplication.http;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BizApplication.Http;

namespace DesktopViewer.Application
{

    /// <summary>
    /// Definition of the application settings
    /// </summary>
    public class DesktopViewerSettings : ApplicationSettings
    {
        public DesktopViewerSettings()
        {
            this.SessionType = ApplicationSessionMode.SingletonSession;
            this.ResponseMode = ApplicationResponseBehavior.Send;
            this.UniqueApplicationName = "desk";
            this.InactivityTimeToLive = 10;
        }
    }

    /// <summary>
    /// Desktop viewer application send screenshot images in jpeg format to the browser.
    /// </summary>
    public class DesktopViewer : HttpApplicationBase
    {

        #region Protocol Data
        public enum MessageType : int { skip = 0, getFrame = 1 };
        public class FrameIndex
        {
            public MessageType Code { get; set; }
            public string Img { get; set; }
            public int ZoomX { get; set; }
            public int ZoomY { get; set; }
        }
        #endregion



        string imgPrefix = "imgFrame_";
        string img_value_delimeter = "x";
        int globalcounter = 0;
        public DesktopViewer()
        {

        }

        public override string ApplicationDirectory()
        {
            return Demos.Properties.Settings.Default.DesktopViewerRootDirectory;
        }

        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="req"></param>
        protected override void PageLoad(HttpRequest req)
        {
            try
            {
                if (req.UrlParameters.Count > 0)
                {
                    ///
                    /// the client send 
                    ///

                    string operation = req.GetQueryStringValue("op");
                    switch (operation)
                    {
                        case "listen":
                            //  Thread.Sleep(1000);
                            string jsonsettings = req.GetQueryStringValue("set");
                            FrameIndex settings = JsonConvert.DeserializeObject<FrameIndex>(HttpHelper.CleanJsonString(jsonsettings));
                            string frame = imgPrefix + (globalcounter++) + img_value_delimeter + settings.ZoomX + img_value_delimeter + settings.ZoomY + img_value_delimeter + ".jpg";
                            SendMessage(new FrameIndex() { Code = MessageType.getFrame, Img = frame }, false);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// We want handle for our self the statics http requests
        /// 
        /// </summary>
        protected override void NewRequest()
        {
            switch (Request.Type)
            {
                case HttpRequestType.HttpPage:
                    base.NewRequest();
                    break;
                case HttpRequestType.HttpStaticRequest:
                    string absolutepath = "";
                    string static_file = Request.Paths[Request.Paths.Count - 1];
                    for (int i = 1; i < Request.Paths.Count - 1; i++) absolutepath += Request.Paths[1] + "//";
                    absolutepath += static_file;
                    BuildResponseFile(ApplicationDirectory() + "\\" + absolutepath, HttpHelper.GetResourceMime(static_file));
                    break;
            }

        }



        protected override void BuildResponseFile(string filepath, MimeType mime)
        {
            if (filepath.Contains(imgPrefix))
            {
                string[] rules = filepath.Split(new string[] { img_value_delimeter }, StringSplitOptions.RemoveEmptyEntries);
                int zoomX = int.Parse(rules[1]);
                int zoomY = int.Parse(rules[2]);


                Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width / zoomX, Screen.PrimaryScreen.Bounds.Height / zoomY);
                Graphics graphics = Graphics.FromImage(printscreen as Image);
                graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

                MemoryStream ms = new MemoryStream();

                printscreen.Save(ms, ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);
                byte[] frame = ms.ToArray();
                ms.Close();
                BuildResponse(frame, mime, false);
            }
            else
            {
                base.BuildResponseFile(filepath, mime);
            }
        }

        public void SendMessage(FrameIndex msg, bool drop_connection)
        {
            BuildResponse(JsonConvert.SerializeObject(msg), drop_connection);
        }

        public override ApplicationSettings Info
        {
            get
            {
                return new DesktopViewerSettings();
            }
        }


        public override void UnloadApplication()
        {

        }
    }
}
