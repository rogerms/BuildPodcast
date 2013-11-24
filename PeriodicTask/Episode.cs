using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace PeriodicTask
{
    public sealed class Episode
    {
        string _title;
        string _description;
        public string title
        {
            get { return _title; }
            set { _title = removeExtraSpaces(value); }
        }

        public string description
        {
            get { return _description; }
            set { _description = removeExtraSpaces(value); }
        }

        public string summary { get; set; }
        public string duration { get; set; }
        public string url { get; set; }
        public string length { get; set; }
        public string type { get; set; }
        public string pubDate { get; set; }
        public string isExplicit { get; set; }
        public string guid { get; set; }
        public string isPermalink { get; set; }
        public string author { get; set; }
        public BitmapImage thumbnail { get; set; }
        public string imageurl { get; set; }

        public string Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("title: {0}, ", title);
            sb.AppendFormat("description: {0}, ", description);
            sb.AppendFormat("summary: {0}, ", summary);
            sb.AppendFormat("duration: {0}, ", duration);
            sb.AppendFormat("url: {0}, ", url);
            sb.AppendFormat("length: {0}, ", length);
            sb.AppendFormat("type: {0}, ", type);
            sb.AppendFormat("pubDate: {0}, ", pubDate);
            sb.AppendFormat("isExplicit: {0}, ", isExplicit);
            sb.AppendFormat("guid: {0}, ", guid);
            sb.AppendFormat("isPermalink: {0}, ", isPermalink);
            sb.AppendFormat("author: {0}, ", author);
            sb.AppendFormat("thumbnail: {0} ", thumbnail);
            return sb.ToString();
        }


        string removeExtraSpaces(string value)
        {
            // Debug.WriteLine(value);
            return System.Text.RegularExpressions.Regex.Replace(value, @"[\s\n\r]", " ");
        }
    }
}
