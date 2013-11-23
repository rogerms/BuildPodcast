using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DataPodcast;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BuildPodcast
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MediaPage : BuildPodcast.Common.LayoutAwarePage
    {
        string localMediaImage = "playing_media.png";

        public MediaPage()
        {
            this.InitializeComponent();
            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;
            mediaGrid.Width = Window.Current.Bounds.Width;
            mediaGrid.Height = Window.Current.Bounds.Height;
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
            Episode episode = navigationParameter as Episode;

            //size mediaplayer
            //myMediaPlayer.Width = Window.Current.Bounds.Width-20;
            //myMediaPlayer.Height = Window.Current.Bounds.Height - 50;

            Window.Current.SizeChanged += Current_SizeChanged;
            if (!episode.imageurl.Equals(String.Empty) && episode.imageurl.StartsWith("http://"))
            {
                Utils.downloadImageToLocalFolderAsync(localMediaImage, episode.imageurl);
                MediaControl.AlbumArt = new Uri("ms-appdata:///local/" + localMediaImage);
            }
            else
            {
                MediaControl.AlbumArt = new Uri("ms-appx:///Assets/no-image-512-bg.png");
            }

            MediaControl.TrackName = episode.title;
            MediaControl.ArtistName = episode.author;
            myMediaPlayer.Source = new Uri(episode.url);
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                myMediaPlayer.Width = Window.Current.Bounds.Width;
                //myMediaPlayer.Height = Window.Current.Bounds.Height / 3;
            }
            else
            {
                myMediaPlayer.Width = Window.Current.Bounds.Width - 20;
                //myMediaPlayer.Height = Window.Current.Bounds.Height - 50;
            }
        }


        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            //code on how keep the screen active and how to release when not needed
            //Windows.System.Display.DisplayRequest displayRequest = new Windows.System.Display.DisplayRequest();
            //displayRequest.RequestActive();
            //displayRequest.RequestRelease();
        }


        private async void MediaControl_StopPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => myMediaPlayer.Stop());
        }

        private async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    if (myMediaPlayer.CurrentState == MediaElementState.Paused)
                        myMediaPlayer.Play();
                    else
                        myMediaPlayer.Pause();
                }
                catch
                {
                }
            });
        }

        private async void MediaControl_PausePressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => myMediaPlayer.Pause());
        }

        private async void MediaControl_PlayPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => myMediaPlayer.Play());
        }

        //private void myMediaPlayer_Tapped_1(object sender, TappedRoutedEventArgs e)
        //{
        //    if (GoBackPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
        //    {
        //        GoBackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        GoBackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //    }
        //}

        private void myMediaPlayer_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("myMediaPlayer_PointerEntered_1");
        }
    }
}
