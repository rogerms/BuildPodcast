using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using BuildPodcast.Common;
using DataPodcast;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace BuildPodcast
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class PodcastsPage : BuildPodcast.Common.LayoutAwarePage
    {
        ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
        PodcastCollection podcasts = new PodcastCollection();
        public PodcastsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
            //this.DefaultViewModel["Items"] = podcasts.Items;
            //this.DataContext = podcasts.Items;
            
            setDataContext();
            this.DataContext = podcasts.Items;
            string isSet = (string)settings.Values["isSet"];
            if (isSet == "true")
            {
                itemGridView.SelectedItem = settings.Values["listItemSelected"];
            
            }
        }

        async void setDataContext()
        {
            PodcastCollection pc = App.Current.Resources["podcastCollection"] as PodcastCollection;
            if (pc.Items.Count > 0)
            {
                this.podcasts = pc;
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();

            List<Podcast> podList = await Utils.loadLocalDataAsync();

            foreach (Podcast pcast in podList)
            {
                //CHECK time w/o image 1969 with 2370 TDOD: add a async image download

               podcasts.addItem(pcast);
              // getEpisodes(pcast.url);
            }
            App.Current.Resources["podcastCollection"] = podcasts;

            

            //Dictionary<string,string> podlist = new Dictionary<string,string>();
            //List<Podcast> duplicated = new List<Podcast>();


            //List<Podcast> result = parser.Podcasts.FindAll(p => !p.imageurl.StartsWith("http"));
            //foreach (Podcast p in result)
            //{
            //    if (!podlist.ContainsKey(p.title))
            //        podlist.Add(p.title, "");
            //}
            foreach (Podcast pcast in podcasts.Items)
            {
                string imageurl = pcast.imageurl;
                //if (!imageurl.Equals(String.Empty) && imageurl.StartsWith("http://"))
                //{
                //    pcast.thumbnail =  new BitmapImage(new Uri(imageurl));
                //}
                //if (pcast.thumbnail == null)
                //{
                //    pcast.thumbnail = new BitmapImage(new Uri("ms-appx:/Assets/no-image-512-bg.png"));
                //}
            }
            sw.Stop();
            
            //DEBUG
            //foreach (Podcast p in podcasts.Items)
            //{
            //    Debug.WriteLine(p);

            //}

            Debug.WriteLine("Total time: " + sw.ElapsedMilliseconds);
            //Debug.WriteLine("Total repeated : "+ podlist.Count);
            //foreach (KeyValuePair<string, string> p in podlist)
            //{
            //    Debug.WriteLine("-> : " + p.Key +" "+p.Value);
            //}
            //progressRing.IsActive = false;
        }

        private void itemGridView_ItemClick_1(object sender, ItemClickEventArgs e)
        {

            //settings.Values["listItemSelected"] = e.ClickedItem;
            //settings.Values["isSet"] = "true";
            string id = ((Podcast)e.ClickedItem).id;

            this.Frame.Navigate(typeof(GroupDetailPage), id);
        }

        async void getEpisodes(string url)
        {
            PodcastEpisodes episodes = new PodcastEpisodes();

            Windows.Web.Syndication.SyndicationClient client1 = new Windows.Web.Syndication.SyndicationClient();
            Uri feedUri = new Uri(url);

            try
            {
        //    HttpClient client = new HttpClient();

        ////    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.17 Safari/537.36");
        //        HttpResponseMessage response = await client.GetAsync(url);
        //        string xmlStr = await response.Content.ReadAsStringAsync();

        //        EpisodesXMLParser parser = new EpisodesXMLParser(xmlStr);

                //foreach (Episode e in parser.Episodes)
                //{
                //    episodes.addItem(e);
                //    //Debug.WriteLine(e);
                //}
                Windows.Web.Syndication.SyndicationFeed feed = await client1.RetrieveFeedAsync(feedUri);
                Debug.WriteLine("{0} total  feeds: {1}", url, feed.Items.Count);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception (getEpisodes) "+url+"\n"+ e.Message);
            }
        }

    }
}
