using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace Yti.Yget.RemoteClient
{
    public class ConfigurationManager
    {
        private String _filePath;
        private Boolean reloadXml = false;
        XPathDocument document;
        XPathNavigator navigator;
        private static ConfigurationManager configurationManager;

        private ConfigurationManager()
        {

        }

        public static ConfigurationManager GetInstance()
        {
            if (null == configurationManager)
            {
                configurationManager = new ConfigurationManager();
            }
            return configurationManager;

        }

        public String SetPath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                reloadXml = true;

            }
        }

        public void InitializeData()
        {
            document = new XPathDocument(SetPath);
            navigator = document.CreateNavigator();


        }

        public void GetAvailableChannels(out List<String> channelNames)
        {
            channelNames = new List<string>();
            if (reloadXml == true)
            {
                InitializeData();
                reloadXml = false;
            }

            if (null != navigator)
            {
                XPathNodeIterator xpathNodeIterator = navigator.Select(@"/ConnectionConfiguration/*");

                foreach (XPathNavigator node in xpathNodeIterator)
                {
                    String s = node.GetAttribute("name", String.Empty);
                    channelNames.Add(s);
                }
            }
        }

        public void GetChannelDetails(String channelName, out CommsObject commsInfo)
        {
            commsInfo = new CommsObject();
            if (reloadXml == true)
            {
                InitializeData();
                reloadXml = false;
            }
            if (null != navigator)
            {
                String query = String.Format("/ConnectionConfiguration/CommsChannel[@name='{0}']/Type", channelName);
                XPathNavigator nav = navigator.SelectSingleNode(query);

                commsInfo.Type = nav.InnerXml;

                query = String.Format("/ConnectionConfiguration/CommsChannel[@name='{0}']/ConnectionPoint", channelName);
                nav = navigator.SelectSingleNode(query);
                commsInfo.ConnectionPoint = nav.InnerXml;

                query = String.Format("/ConnectionConfiguration/CommsChannel[@name='{0}']/Port", channelName);
                nav = navigator.SelectSingleNode(query);
                commsInfo.Port = nav.InnerXml;

                commsInfo.ChannelName = channelName;

            }
        }

    }
}
