using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.Serialization;


namespace DataPodcast
{
    public class Podcast
    {
        string _description;
        string _imageurl = "";
        public string id { set; get; }
        public string title { get; set; }
        public string description { get { return _description; } set { _description = stringCleanup(value); } }
        public string url { get; set; }
        public string link { get; set; }
        public string language { get; set; }
        public string keywords { get; set; }
        public string category { get; set; }
        public string pubdate { get; set; }
        public string author { get; set; }
        public string imageurl {
            get { return _imageurl; }
            set
            {
                if (value.Equals(String.Empty) || (!value.StartsWith("http://") && !value.StartsWith("ms-appx:/")))
                {
                    _imageurl = "ms-appx:/Assets/no-image-512-bg.png";
                }
                else 
                {
                    _imageurl = value;
                }
            } 
        }
       // [System.Runtime.Serialization.DataMember(EmitDefaultValue=true)]
        //[DataMember(EmitDefaultValue=true)]
       // public BitmapImage thumbnail { get; set; }

        public override string ToString()
        {
            
            return String.Format("id: {{{0}}} title: {1}  description{2} url: {3} link:{4}, language:{5}, keywords:{6}, category:{7}, pubdate:{8}, author:{9}, imageurl:{10}",
                id, title, description,  url, link, language, keywords, category, pubdate, author, imageurl
                //, (thumbnail == null)
             );
        }

        public string ToJson()
        {
            string qStr = "{{\n'id': {0}, \n'title': '{1}', \n'description': '{2}',\n'url': '{3}', \n'link': '{4}', \n'language': '{5}',\n'keywords': '{6}',\n'category': '{7}',\n'pubdate': '{8}',\n'author': '{9}',\n'imageurl': '{10}'\n}}";
            qStr = qStr.Replace('\'', '"');
            return String.Format(qStr, id, title, description, url, link, language, keywords, category, pubdate, author, imageurl);
   
        }

        string stringCleanup(string value)
        {
            value = value.Replace("\"", "'");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[\n\r\s]{2,}", " ");
            return System.Text.RegularExpressions.Regex.Replace(value, @"<[^>]*>", String.Empty);
        }
    }

    public class Episode
    {
        string _title;
        string _description;
        public string title 
        {
            get{ return _title;}
            set { _title = removeExtraSpaces(value);    }
        }

        public string description
        {
            get { return _description; }
            set{_description = removeExtraSpaces(value);   }
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{{\ntitle: {0}, ", title);
            sb.AppendFormat("\ndescription: {0}, ", description);
            sb.AppendFormat("\nsummary: {0}, ", summary);
            sb.AppendFormat("\nduration: {0}, ", duration);
            sb.AppendFormat("\nurl: {0}, ", url);
            sb.AppendFormat("\nlength: {0}, ", length);
            sb.AppendFormat("\ntype: {0}, ", type);
            sb.AppendFormat("\npubDate: {0}, ", pubDate);
            sb.AppendFormat("\nisExplicit: {0}, ", isExplicit);
            sb.AppendFormat("\nguid: {0}, ", guid);
            sb.AppendFormat("\nisPermalink: {0}, ", isPermalink);
            sb.AppendFormat("\nauthor: {0}, ", author);
            sb.AppendFormat("\nthumbnail: {0} }}", thumbnail);
            return sb.ToString();
        }


        string removeExtraSpaces(string value)
        {
           // Debug.WriteLine(value);
            return System.Text.RegularExpressions.Regex.Replace(value, @"[\s\n\r]", " ");
        }
    }

    public class PodcastCollection
    {

        private ObservableCollection<Podcast> _Items = new ObservableCollection<Podcast>();

        public ObservableCollection<Podcast> Items
        {
            get
            {
                return this._Items;
            }
        }

        public void addItem(Podcast podcast)
        {
            this.Items.Add(podcast);
        }

        public Podcast getItemFromId(string id)
        {
            return this.Items.First(x => x.id == id);
        }
    }

    public class PodcastEpisodes
    {

        private ObservableCollection<Episode> _items = new ObservableCollection<Episode>();

        public Podcast Channel { set; get; }

        public ObservableCollection<Episode> Items
        {
            get
            {
                return this._items;
            }
            set
            {
                _items = value;
            }
        }

        public void addItem(Episode episode)
        {
            _items.Add(episode);
        }
    }
}

