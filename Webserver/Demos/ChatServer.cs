using System;
using System.Text;
using System.Collections.Concurrent;
using BizApplication;
using BizApplication.http;
using Newtonsoft.Json;
using ServerCommonLibrary;
using BizApplication.Http;


namespace Chat.HttpApplication
{

    /// <summary>
    /// Define the chatServer application settings 
    /// </summary>
    public class ChatServerConfiguration : ApplicationSettings
    {
        public ChatServerConfiguration()
        {
            this.SessionType = ApplicationSessionMode.BrowserSession;
            this.UniqueApplicationName = "chat";
            ///
            ///  Share is a way to comunicate with other istances
            ///
            this.ResponseMode = ApplicationResponseBehavior.ShareAndSend;
            this.InactivityTimeToLive = 10;
        }
    }

    /// <summary>
    /// ChatServer Application share a simple text message in json format to all connected users.
    ///     One session of ChatServer identify one logged user, chat messages broadcasting 
    ///     is possible using the ApplicationResponseBehavior in 'Share' mode that's mean every response sent to the browser is 
    ///     delivered to any ChatServer sesssion (see OnNewShareResponse(...)).  
    ///     
    ///     To enter in the chat room is require a foo login and password.
    /// 
    /// </summary>
    public class ChatServer : HttpApplicationBase, IDisposable
    {
        
        #region COMUNICATION DATA
        /// <summary>
        /// In this region are define the structure of the comunication messages 
        /// </summary>

       
        public enum MessageType : int { alert = 0, eval = 1, chatmessage = 2,adduser = 3,skip=4};

        /// <summary>
        /// Chat Message is used to comunicate with the browser.        
        ///     - MessageCode property define the type of the message, see  MessageType value.
        ///     - Value property change the purpose depending on the messagecode.
        ///         MessageCode alert   ->  the value is  the text to show
        ///         MessageCode eval  ->  the Value is js script to eval
        ///         MessageCode chatmessage ->  the Value is  the content of the message 
        ///         MessageCode Adduser ->  the Value is empty
        ///     - user property get the user involve in the message, result empty if the messageType is alert or eval.
        /// </summary>
        public class ChatMessage
        {
            public int MessageCode { get; set; }
            public string Value { get; set; }
            public string User { get; set; }
        }

        /// <summary>
        /// SharedChatMessage is designed to comunicate with others chatServer session.        
        /// </summary>
        public class SharedChatMessage : HttpResponse
        {
            public SharedChatMessage(byte[] response, ApplicationRequest request, ChatMessage message)
                : base(response, request)
            {
                this.Message = message;
            }

            public ChatMessage Message { get; set; }
        }

        #endregion 

        
        public static string loginPage = "index.html";
        public static string roomPage = "room2.html";

        //### User Nickname
        string currentUsername;
        //### User Password
        string currentPassowrd;
        //### IsValid User
        bool isValidUser;
        //### Send User
        bool sendAdduser;
        //### chat message queue
        ConcurrentQueue<ChatMessage> localqueuemessages;

        /// <summary>
        /// Chat application main directory
        /// </summary>
        /// <returns></returns>
        public override string ApplicationDirectory()
        {
            return Demos.Properties.Settings.Default.ChatServerRootDirectory;            
        }


        public ChatServer()
        {
            currentUsername = "";
            currentPassowrd = "";
            isValidUser = false;
            sendAdduser = false;
            localqueuemessages = new ConcurrentQueue<ChatMessage>();
        }

        /// <summary>
        /// Page load event.                        
        /// </summary>
        /// <param name="req">
        /// The request from the browser.
        /// </param>
        protected override void PageLoad(HttpRequest req)
        {

            ///           
            /// The ChatServer application is strictly relate with the javascript implementation, 
            /// so if the request contains query url string parameters means that invoking by the XMLHttpRequest object (ajax pattern).
            ///   You can choose a different way to distinguish 'pure' pageload event (called in Asp.net postback) from ajax event.                       
            /// 
            ///  The query url request has this pattern: ?op=<operation>&<par1>=<value1>&<par2>=<value2> where op stands for 'operation'.                        
            ///
            string page = Request.Paths[Request.Paths.Count - 1];

            if (req.UrlParameters.Count > 0)
            {
                SharedChatMessage sharedresponse = null;
                ChatMessage message = null;
                string operation = req.GetQueryStringValue("op");
                switch (operation)
                {
                    case "login":            
                        ///
                        /// Login operation process steps:                        
                        ///         1) Check username and password , if one of these is empty we respond with an alert
                        ///         2) Validation of the user 
                        ///         3) Respond with a redirect 
                        ///
                        string username = req.GetQueryStringValue("username");
                        string password = req.GetQueryStringValue("password");
                        if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                        {
                            BuildChatResponse(new ChatMessage() { MessageCode = (int)MessageType.alert, Value = "Login request error." }, true);
                            return;
                        }
                        currentUsername = username;
                        currentPassowrd = password;
                        isValidUser = true;
                        BuildChatResponse(new ChatMessage() { MessageCode = (int)MessageType.eval, Value = "window.location=\"" + roomPage + "\"" }, true);
                        return;
                    case "listen":
                        ///
                        ///  When the room page is loaded start to send 'listen' operation request in loop,
                        ///  at every listen request we respond with a chatmessage getting from the queue or,if is empty ,with a skip action message.
                        ///    But firstable we sent adduser action message with SharedChatMessage envelop for notify the new user.                        
                        ///
                        if (!sendAdduser)
                        {
                            sendAdduser = true;
                            message = new ChatMessage() { MessageCode = (int)MessageType.adduser, User = currentUsername, };
                            BuildChatResponse(message, false);
                            this.response = sharedresponse = new SharedChatMessage(Response.ResponseData, Response.AppRequest, message);
                            return;
                        }
                        if (localqueuemessages.Count == 0)
                        {                            
                            System.Threading.Thread.Sleep(500);
                            BuildChatResponse(new ChatMessage() { MessageCode = (int)MessageType.skip, Value = "" }, false);
                        }
                        else
                        {
                            ChatMessage msg = null;
                            if (localqueuemessages.TryDequeue(out msg))
                                BuildChatResponse(msg, false);
                        }
                        return;
                    case "message":
                        ///
                        /// A chat message is sent by the user, 
                        ///     firstable we build ChatMessage packet replacing the response with SharedChatMessage envelop, 
                        ///     that is why SharedChatMessage is visible to the other session (see OnNewShareResponse).
                        ///                             
                        ///

                        string value = req.GetQueryStringValue("value");
                        message = new ChatMessage() { MessageCode = (int)MessageType.chatmessage, Value = value, User = currentUsername };
                        BuildChatResponse(message, false);
                        sharedresponse = new SharedChatMessage(Response.ResponseData, Response.AppRequest, message);
                        Response = sharedresponse;
                        return;
                    
                    default:
                        throw new InvalidOperationException("Invalid request");
                }
            }            
            if (page == roomPage)
            {
                /// 
                /// if the user not perform the login the roomPage will not be loaded, will be sent a login page                
                ///                
                if (!isValidUser)
                {
                    BuildResponseFile(ApplicationDirectory() + "\\" + loginPage,MimeType.text_html);
                    return;
                }
                else
                {
                    byte[] room = Helper.GetFile(ApplicationDirectory() + "\\" + roomPage);
                    string msg = new string(Encoding.UTF8.GetChars(room));
                    msg = msg.Replace("<%ws_username%>", this.currentUsername);
                    BuildResponse(msg);
                }
            }
        }

        /// <summary>
        /// An Application instace share a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        /// <param name="req"></param>
        public override void OnNewShareResponse(ApplicationInstanceBase sender, ApplicationResponse response, ApplicationRequest req)
        {
            if (response is SharedChatMessage)
            {
                ///
                /// if the response type is SharedChatMessage we simple get the chat message and put it into the queue 
                ///
                SharedChatMessage sharedresponse = (SharedChatMessage)response;
                localqueuemessages.Enqueue(sharedresponse.Message);
            }
        }


        public HttpResponse BuildChatResponse(ChatMessage msg, bool drop_connection)
        {
            BuildResponse(JsonConvert.SerializeObject(msg), drop_connection);
            return (HttpResponse)this.response;
        }



        public override ApplicationSettings Info
        {
            get
            {
                return new ChatServerConfiguration();
            }
        }

        public void Dispose()
        {
            localqueuemessages = null;
        }
    }



}
