using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DataPodcast;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BuildPodcast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PodList : Page
    {
        PodcastCollection podcasts = new PodcastCollection();

        public PodList()
        {
            this.InitializeComponent();            
            setDataContext();

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        async void setDataContext()
        {
            PodcastCollection pc = App.Current.Resources["podcastCollection"] as PodcastCollection;
            if (pc.Items.Count > 0)
            {
                this.DataContext = pc.Items;
                return;
            }

            StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\data\Podcast.xml"); //OpenForReadAsync();
            StorageFile defaultImage = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\face.jpg");

            progressRing.IsActive = true;
   
            //await showDialog("setDataContext");
            string text = await FileIO.ReadTextAsync(file);

            //Debug.WriteLine(text);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);

            HttpClient client = new HttpClient();

            Debug.WriteLine("total " + doc.GetElementsByTagName("channel").Count);

            this.DataContext = podcasts.Items;
            App.Current.Resources["podcastCollection"] = podcasts;
            foreach (XmlElement elem in doc.GetElementsByTagName("channel"))
            {
                


                BitmapImage bimage = null;
                string imgUrl = elem.GetElementsByTagName("imageurl")[0].InnerText;
                if (!imgUrl.Equals(String.Empty) && imgUrl.StartsWith("http://"))
                {
                    bimage = new BitmapImage(new Uri(imgUrl));
                }
                if (bimage == null)
                {
                    bimage = new BitmapImage(new Uri("ms-appx:/Assets/no-image-512-bg.png"));
                }

                //if (!imgUrl.Equals(String.Empty) && imgUrl.StartsWith("http://"))
                //{
                //    HttpResponseMessage response = await client.GetAsync(imgUrl);

                //    byte[] imgbytes = await response.Content.ReadAsByteArrayAsync();

                //    using (Windows.Storage.Streams.IRandomAccessStream stream = new AccessStream(imgbytes))
                //    {
                //        stream.Seek(0);
                //        BitmapImage b = new BitmapImage();
                //        await b.SetSourceAsync(stream);
                //    }

                //}

                podcasts.addItem(new Podcast
                {
                    title = elem.GetElementsByTagName("title")[0].InnerText,
                    id = elem.GetElementsByTagName("id")[0].InnerText,
                    url = elem.GetElementsByTagName("url")[0].InnerText,
                    link = elem.GetElementsByTagName("link")[0].InnerText,
                    author = elem.GetElementsByTagName("author")[0].InnerText,
                    category = elem.GetElementsByTagName("category")[0].InnerText,
                    description = elem.GetElementsByTagName("description")[0].InnerText,
                    keywords = elem.GetElementsByTagName("keywords")[0].InnerText,
                    imageurl = elem.GetElementsByTagName("imageurl")[0].InnerText,
                    language = elem.GetElementsByTagName("language")[0].InnerText,
                    pubdate = elem.GetElementsByTagName("pubdate")[0].InnerText,
                   // thumbnail = bimage
                }
                    );
            }

            foreach (Podcast p in podcasts.Items)
            {
                Debug.WriteLine(p);

            }
           
            progressRing.IsActive = false;
        }

        private void GridView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            string id = ((Podcast)e.ClickedItem).id;

            this.Frame.Navigate(typeof(GroupDetailPage), id);
            //showPopup("GridView_ItemClick_1");
        }
        
    }
}
