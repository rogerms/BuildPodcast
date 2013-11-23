using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Data.Xml.Dom;
using DataPodcast;

namespace BuildPodcast.Common
{
    class PodcastsXMLParser
    {
        XmlDocument doc = null;
        List<Podcast> _podcasts = new List<Podcast>();

        public PodcastsXMLParser(string xml)
        {
            doc = new XmlDocument();
            doc.LoadXml(xml);
            parse();
        }

        public List<Podcast> Podcasts
        {
            get
            {
                return _podcasts;
            }
        }

        public int itemCount { set; get; }

        void parse()
        {
            XmlNodeList channels =  doc.GetElementsByTagName("channel");
            itemCount = channels.Count;

            foreach (XmlElement elem in channels)
            {
                _podcasts.Add(new Podcast
                {
                    title = getNodeValue(elem, "title"),
                    id = getNodeValue(elem,"id"),
                    url = getNodeValue(elem,"url"),
                    link = getNodeValue(elem,"link"),
                    author = getNodeValue(elem,"author"),
                    category = getNodeValue(elem,"category"),
                    description = getNodeValue(elem,"description"),
                    keywords = getNodeValue(elem,"keywords"),
                    imageurl = getNodeValue(elem,"imageurl"),
                    language = getNodeValue(elem,"language"),
                    pubdate = getNodeValue(elem,"pubdate"),
                }
                    );
            }

            //foreach (Podcast e in _podcasts)
            //{
            //    Debug.WriteLine(e);
            //}
        }

        string getNodeValue(XmlElement element, string name)
        {
            try
            {
                if (element.GetElementsByTagName(name).Count > 0)
                {
                    return element.GetElementsByTagName(name)[0].InnerText;
                }
                return "";
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem geting value " + name );
                Debug.WriteLine("element " + element.GetXml());
                return e.Message;
            }
        }

        string getNodeAttribute(XmlElement element, string node, string attribute)
        {
            try
            {
                if (element.GetElementsByTagName(node).Count > 0)
                {
                    if (element.GetElementsByTagName(node)[0].Attributes.GetNamedItem(attribute) != null)
                    {
                        return element.GetElementsByTagName(node)[0].Attributes.GetNamedItem(attribute).NodeValue.ToString();
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem geting value " + attribute);
                Debug.WriteLine("element " + element.GetXml());
                return e.Message;
            }
        }
    }
}
