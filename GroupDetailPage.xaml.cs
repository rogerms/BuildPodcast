using BuildPodcast.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DataPodcast;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace BuildPodcast
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class GroupDetailPage : BuildPodcast.Common.LayoutAwarePage
    {
        PodcastEpisodes episodes = new PodcastEpisodes();
        public GroupDetailPage()
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
            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]

            string id = (string)navigationParameter;
            PodcastCollection pc = App.Current.Resources["podcastCollection"] as PodcastCollection;
            Podcast podcast = pc.getItemFromId(id);
            episodes.Channel = podcast;
            getEpisodes(podcast.url);

            this.DefaultViewModel["Group"] = podcast;
            this.DefaultViewModel["Items"] = episodes.Items;
            
        }

        async void getEpisodes(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                string xmlStr = await response.Content.ReadAsStringAsync();

                EpisodesXMLParser parser = new EpisodesXMLParser(xmlStr);

                foreach (Episode e in parser.Episodes)
                {
                    //e.thumbnail = episodes.Channel.thumbnail;
                    e.imageurl = episodes.Channel.imageurl;
                    episodes.addItem(e);
                    Debug.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception was " + e.StackTrace);
            }
        }

        private void itemGridView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(MediaPage), e.ClickedItem);
        }

        async Task showDialog(string msg, string title = "Alert")
        {

            MessageDialog dialog = new MessageDialog("message: " + msg, title);
            //dialog.Commands.Add(new UICommand("Yes", OnCommand, 0));
            //dialog.Commands.Add(new UICommand("No", OnCommand, 1));
            dialog.Commands.Add(new UICommand("Close", null, 2));
            dialog.DefaultCommandIndex = 2;

            await dialog.ShowAsync();
        }
    }
}
