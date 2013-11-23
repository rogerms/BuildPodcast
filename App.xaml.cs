using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Search;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using DataPodcast;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace BuildPodcast
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        List<string> suggestionList = null;
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            // Register handler for SuggestionsRequested events from the search pane
            suggestionList = await Utils.getSearchSuggestionList();
            SearchPane.GetForCurrentView().SuggestionsRequested += OnSuggestionsRequested;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(FavoritesMainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        void OnSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            string query = args.QueryText.ToLower();
            
            if (suggestionList != null)
            {
                List<string> result = suggestionList.FindAll(a => a.Contains(query));
                args.Request.SearchSuggestionCollection.AppendQuerySuggestions(result);
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running
            // Reinitialize the app if a new instance was launched for search
            if (args.PreviousExecutionState == ApplicationExecutionState.NotRunning ||
                args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser ||
                args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {

                PodcastCollection pc = App.Current.Resources["podcastCollection"] as PodcastCollection;
                if (pc.Items.Count < 1)
                {
                    List<Podcast> podList = await Utils.loadLocalDataAsync();
                    PodcastCollection podcasts = new PodcastCollection();
                    foreach (Podcast pcast in podList)
                    {
                        //CHECK time w/o image 1969 with 2370 TDOD: add a async image download

                        podcasts.addItem(pcast);
                        // getEpisodes(pcast.url);
                        App.Current.Resources["podcastCollection"] = podcasts;
                    }
                    foreach (Podcast pcast in podcasts.Items)
                    {
                        //string imageurl = pcast.imageurl;
                        //if (!imageurl.Equals(String.Empty) && imageurl.StartsWith("http://"))
                        //{
                        //    pcast.thumbnail = new BitmapImage(new Uri(imageurl));
                        //}
                        //if (pcast.thumbnail == null)
                        //{
                        //    pcast.thumbnail = new BitmapImage(new Uri("ms-appx:/Assets/no-image-512-bg.png"));
                        //}
                    }
                }


                // Register handler for SuggestionsRequested events from the search pane
                SearchPane.GetForCurrentView().SuggestionsRequested += OnSuggestionsRequested;


            }
            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                BuildPodcast.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await BuildPodcast.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (BuildPodcast.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(SearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }
    }
}
