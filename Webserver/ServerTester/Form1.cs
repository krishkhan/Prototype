using System;
using System.Windows.Forms;
using ServerCommonLibrary;
using Server;
using Server.Services;

namespace webserver.tester
{
    public partial class WebServer : Form
    {
        //### server 
        WebServer<myLogger> server;

        public WebServer()
        {
            InitializeComponent();
            myLogger.OnNewMessage += new myLogger.TraceMessageHandler(Tracer_OnNewMessage);
            this.FormClosing += new FormClosingEventHandler(WebServer_FormClosing);
        }

        void WebServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormClosing -= (WebServer_FormClosing);
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                ///
                /// Create the server with the Http provider 
                ///
                server = new WebServer<myLogger>();
                server.AddService<HttpService<myLogger>>(int.Parse(txtPort.Text));
                btnStart.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        void Tracer_OnNewMessage(string message)
        {
            add2debug(message);
        }


        void add2debug(string message)
        {
            this.ltbDebug.Invoke((MethodInvoker)delegate
            {
                ltbDebug.Items.Add(message);
                ltbDebug.SelectedIndex = ltbDebug.Items.Count - 1;
            });
        }

        private void brnClear_Click(object sender, EventArgs e)
        {
            ltbDebug.Items.Clear();
        }
    }

    public class myLogger : IDebugger
    {
        public delegate void TraceMessageHandler(string message);
        public static event TraceMessageHandler OnNewMessage;

        public void trace(string log)
        {
            if (OnNewMessage != null)
                OnNewMessage(log);
        }
    }
}
