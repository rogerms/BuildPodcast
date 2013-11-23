using BuildPodcast.Common;
using DataPodcast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Syndication;

namespace BuildPodcast
{
    class Utils
    {
        public async static Task showDialog(string msg, string title = "Alert")
        {

            MessageDialog dialog = new MessageDialog("message: " + msg, title);
            //dialog.Commands.Add(new UICommand("Yes", OnCommand, 0));
            //dialog.Commands.Add(new UICommand("No", OnCommand, 1));
            dialog.Commands.Add(new UICommand("Close", OnCommand, 2));
            dialog.DefaultCommandIndex = 2;

            await dialog.ShowAsync();
        }

        private static void OnCommand(IUICommand command)
        {
            Debug.WriteLine("command choosen: " + command.Id);
        }

        public void showPopup(string msg)
        {
            Popup popup = new Popup();
            Button bt = new Button { Content = "message: " + msg };
            popup.Child = bt;
            popup.IsOpen = true;
            bt.Click += bt_Click;
        }

        void bt_Click(object sender, RoutedEventArgs e)
        {
            Popup popup = (sender as Button).Parent as Popup;
            popup.IsOpen = false;
        }

        public async Task showPopupMenu(string msg)
        {

            PopupMenu dialog = new PopupMenu();
            dialog.Commands.Add(new UICommand(msg, OnCommand, 0));
            dialog.Commands.Add(new UICommand("No", OnCommand, 1));
            dialog.Commands.Add(new UICommandSeparator());
            dialog.Commands.Add(new UICommand("Close", OnCommand, 2));
            await dialog.ShowAsync(new Point(10, 10));
        }

        public static async void downloadImageToLocalFolderAsync(string filename, string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            byte[] img = await response.Content.ReadAsByteArrayAsync();
            //Windows.Storage.Streams.InMemoryRandomAccessStream randomAccessStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            //DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
            //writer.WriteBytes(img);

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            if (file != null)
            {
                await FileIO.WriteBytesAsync(file, img);
            }
        }



        public static async Task<BitmapImage> getBitmapImageAsync(string imageurl)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(imageurl);
            byte[] img = await response.Content.ReadAsByteArrayAsync();
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
            writer.WriteBytes(img);
            await writer.StoreAsync();
            BitmapImage b = new BitmapImage();
            await b.SetSourceAsync(randomAccessStream);
            return b;
        }

        public static async Task<List<string>> getSearchSuggestionList()
        {
            List<string> _list = new List<string>();
            StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\data\Podcast.xml");
            string xmlString = await FileIO.ReadTextAsync(file);
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlNodeList titles = doc.GetElementsByTagName("title");
                int itemCount = titles.Count;

                foreach (XmlElement elem in titles)
                {
                    _list.Add(elem.InnerText.ToLower());
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("Error on createSearchSuggestionList: " + e);
            }
            return _list;
        }
        public static async Task<List<Podcast>> loadLocalDataAsync()
        {
            //progressRing.IsActive = true;
            StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\data\Podcast.xml"); 
            string xmlString = await FileIO.ReadTextAsync(file);

            //Debug.WriteLine(text);

            PodcastsXMLParser parser = new PodcastsXMLParser(xmlString);
            Debug.WriteLine("total podcasts: " + parser.itemCount);
            return parser.Podcasts;
        }
    }
}
