using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Windows;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Yti.Yget.RemoteClient
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private String _logText;
        private byte[] bytes = new byte[1024];
        private String _configFilePath;
        private List<String>  _channelNames;
        private CommsObject _selectedChannelDetails;
        private int _selectedChannelIndex;

        public ICommand ConnectCommand { get; set; }

        public ICommand SelectFileCommand { get; set; }

        public List<String> ChannelNames
        {
            get
            {
                return _channelNames;
            }
            set
            {
                _channelNames = value;
            }
           
        }

        public CommsObject SelectedChannelDetails
        {
            get
            {
                return _selectedChannelDetails;
            }
            set
            {
                _selectedChannelDetails = value;
                OnPropertyChanged("SelectedChannelDetails");
            }
        }

        public int SelectedChannelIndex
        {
            get
            {
                return _selectedChannelIndex;
            }
            set
            {
                _selectedChannelIndex = value;
                OnPropertyChanged("SelectedChannel");
                if (_selectedChannelIndex >= 0)
                {
                    CommsObject commsObj;
                    ConfigurationManager.GetInstance().GetChannelDetails(_channelNames[SelectedChannelIndex],out commsObj );
                    SelectedChannelDetails = commsObj;
                }

            }
        }

        public String ConfigFilePath { get { return _configFilePath ;}
            set
            { 
                _configFilePath =value  ;
                OnPropertyChanged("ConfigFilePath");
            }
        }
        
        
        public void SetSelectedConfigurationFile(String fileName)
        {
            ConfigurationManager.GetInstance().SetPath = fileName;
            List<String> channelNamesList;
            ConfigurationManager.GetInstance().GetAvailableChannels(out channelNamesList);
            ChannelNames = channelNamesList;
            ConfigFilePath = fileName;
            SelectedChannelIndex = 0;

        }
        public ConnectionViewModel()
        {
            this.ConnectCommand = new ConnectCommand(this);
            this.SelectFileCommand = new SelectFileCommand(this);
            _channelNames = new List<String>() { "Local", "Remote" }; 
            PushToLogWindow = "SCS is not connected";
            PushToLogWindow = "Connect to socket";
        }

        public bool Connect()
        {
            Boolean connectionStatus = false;
            //MessageBox.Show("Connection Initiated");
            StartClient();
            return connectionStatus;
        }

        public String PushToLogWindow
        {
            get
            {
                return "\n" + _logText;
            }
            set
            {
                _logText += value + Environment.NewLine;

                OnPropertyChanged("PushToLogWindow");
            }
        }

        

        private void StartClient()
        {
            // Data buffer for incoming data.


            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    
                    sender.Connect(remoteEP);

                    PushToLogWindow = (String.Format("Socket connected to {0}", sender.RemoteEndPoint.ToString()));

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);

                    PushToLogWindow = (String.Format("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec)));

                    // Release the socket.
                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    PushToLogWindow = String.Format("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    PushToLogWindow = String.Format("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    PushToLogWindow = String.Format("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class ConnectCommand : ICommand
    {
        private readonly ConnectionViewModel _cvm;

        public ConnectCommand(ConnectionViewModel connectionViewModel)
        {
            _cvm = connectionViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _cvm.Connect();
        }

    }


    public class SelectFileCommand : ICommand
    {
        private readonly ConnectionViewModel _cvm;

        public SelectFileCommand(ConnectionViewModel connectionViewModel)
        {
            _cvm = connectionViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            OpenFileDialog browseReport = new OpenFileDialog();
            browseReport.DefaultExt = ".xlsx";
            browseReport.Filter = "Settings(.xml)|*.xml";

            Nullable<bool> result = browseReport.ShowDialog();
            _cvm.SetSelectedConfigurationFile(browseReport.FileName);  

        }
    }

    //public static class CommsCommands
    //{
    //    public static RoutedCommand ConnectToPort = new RoutedCommand();
    //    public static RoutedCommand SelectFile = new RoutedCommand();
    //}


    public class CommsObject
    {
        public String ChannelName { set; get; }

        public String TimeOut { set; get; }

        public String ConnectionPoint { set; get; }

        public List<String> SCS { get; set; }

        public String Port { get; set; }

        public String Type { get; set; }
        
    }


    
}
