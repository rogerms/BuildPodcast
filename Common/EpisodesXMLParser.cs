using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Data.Xml.Dom;
using DataPodcast;

namespace BuildPodcast.Common
{
    class EpisodesXMLParser
    {
        XmlDocument doc = null;
        string xmlStr = null;
        List<Episode> _episodes =  new List<Episode>();

        public EpisodesXMLParser(string xml)
        {
            doc = new XmlDocument();
            xmlStr = xml.Replace("itunes:", "itunes-");
            doc.LoadXml(xmlStr);
            parse();
        }

        public List<Episode> Episodes
        {
            get
            {
                return _episodes;
            }
        }

        public int itemCount { set; get; }

        void parse()
        {
            itemCount = doc.GetElementsByTagName("item").Count;
           
            foreach (XmlElement elem in doc.GetElementsByTagName("item"))
            {
                Episodes.Add(new Episode
                {
                    title = getNodeValue(elem, "title"),
                    description = getNodeValue(elem, "description"),
                    summary = getNodeValue(elem, "itunes-summary"),

                    duration = getNodeValue(elem, "itunes-duration"),

                    url = getNodeAttribute(elem, "enclosure", "url"),
                    length = getNodeAttribute(elem, "enclosure", "length"),
                    type = getNodeAttribute(elem, "enclosure", "type"),

                    pubDate = getNodeValue(elem, "pubDate"),
                    isExplicit = getNodeValue(elem, "itunes-explicit"),
                    isPermalink = getNodeAttribute(elem, "guid", "isPermaLink"), 
                    author = getNodeValue(elem, "itunes-author"),
              
                }
                    );
            }

            //foreach (Episode e in _episodes)
            //{
            //    Debug.WriteLine(e);
            //}
        }

        string getNodeValue(XmlElement element, string name)
        {
            try
            {
                if (element.GetElementsByTagName(name).Count >  0)
                {
                    return element.GetElementsByTagName(name)[0].InnerText;
                }
                return "";
            }
            catch(Exception e)
            {
                Debug.WriteLine("Problem geting value " + name + "element null" + (element == null) + " =" + (element.GetElementsByTagName(name) == null));
                Debug.WriteLine("element "+element.GetXml());
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
                Debug.WriteLine("Problem geting value " + attribute );
                Debug.WriteLine("element " + element.GetXml());
                return e.Message;
            }
        }
    }
}
