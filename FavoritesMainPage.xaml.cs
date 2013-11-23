using BuildPodcast.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DataPodcast;
using System.Text;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace BuildPodcast
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class FavoritesMainPage : BuildPodcast.Common.LayoutAwarePage
    {
        PodcastCollection favecasts = new PodcastCollection();
        private const string _name = "podBackgroundTask";

        public FavoritesMainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            setDataContext();
            // this.DefaultViewModel["Groups"] = groups;
            //this.DefaultViewModel["Items"] = favecasts.Items;
            DataContext = favecasts.Items;
            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                Debug.WriteLine("all bg tasks {0}: {1}", t.Value.TaskId, t.Value.Name);
            }

          //  var task = BackgroundTaskRegistration.AllTasks.Where(x => x.Value.Name == _name).FirstOrDefault();

            //if (task.Value == null)
            //{
            //    //task.Value.Unregister(true);
            //    var builder = new BackgroundTaskBuilder();
            //    builder.Name = _name;
            //    builder.TaskEntryPoint = "PeriodicTask.PeriodicTask";
            //    builder.SetTrigger(new MaintenanceTrigger(15, false));
            //    builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            //    builder.Register();
            //}
            //Items' property not found on 'System.Collections.ObjectModel.ObservableCollection<DataPodcast.Podcast>'. 
        }

        async void setDataContext()
        {
            favecasts = new PodcastCollection();
            //progressRing.IsActive = true;
            StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\data\Favorites.xml"); //OpenForReadAsync();
            //StorageFile defaultImage = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\face.jpg");
            string text = await FileIO.ReadTextAsync(file);

            //Debug.WriteLine(text);

            PodcastsXMLParser parser = new PodcastsXMLParser(text);
            Debug.WriteLine("total podcasts: " + parser.itemCount);

            foreach (Podcast pcast in parser.Podcasts)
            {
                //CHECK time w/o image 1969 with 2370 TDOD: add a async image download
                favecasts.addItem(pcast);
            }

            //foreach (Podcast pcast in favecasts.Items)
            //{
            //    string imageurl = pcast.imageurl;
            //    pcast.thumbnail = null;
            //    if (!imageurl.Equals(String.Empty) && (imageurl.StartsWith("http://") || imageurl.StartsWith("ms-appx:/")))
            //    {
            //        pcast.thumbnail = new BitmapImage(new Uri(imageurl));
            //        Debug.WriteLine("adding images with http");
            //    }
            //    if (pcast.thumbnail == null)
            //    {
            //        pcast.thumbnail = new BitmapImage(new Uri("ms-appx:/Assets/no-image-512-bg.png"));
            //        Debug.WriteLine("adding images no image");
            //    }
                
            //}
            //DEBUG
            
            //foreach (Podcast p in favecasts.Items)
            //{
            //    Debug.WriteLine(p.ToJson());

            //}
            string jsonStr = JsonSerialization.WriteFromObject(favecasts.Items);
            Debug.WriteLine("jason result" + jsonStr);

            IStorageFile fileout = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(@"data\fav.json", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(fileout, jsonStr);
            text = await FileIO.ReadTextAsync(fileout);

            var podObj = JsonSerialization.ReadToObject(text);
            Debug.WriteLine("fav count" + podObj.Count);
            Debug.WriteLine("first object" + podObj[0]);
            //progressRing.IsActive = false;
        }

        private void itemGridView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine("I was itemGridView_ItemClick_1 clicked "+ (e.ClickedItem as Podcast).id);
            if ((e.ClickedItem as Podcast).id == "0")
                this.Frame.Navigate(typeof(PodcastsPage));
            else
                this.Frame.Navigate(typeof(FavoriteDetailPage), e.ClickedItem);
        }
    }
}
