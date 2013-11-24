using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Notifications;

namespace PeriodicTask
{
    public sealed class PeriodicTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Request a deferral since we're making async calls
            var deferral = taskInstance.GetDeferral();
            
            try
            {
                List<string> podcast = await getFavorites();
                List<Episode> episodes = new List<Episode>();
                foreach (var purl in podcast)
                {
                    Debug.WriteLine("url for episodes" + purl);
                    EpisodesXMLParser eparser = new EpisodesXMLParser();
                    await eparser.parse(purl);
                    foreach (var episo in eparser.Episodes)
                    {
                        episodes.Add(episo);
                    }
                    Debug.WriteLine("counter: "+ eparser.Episodes.Count + " " + episodes.Count);
                }


                foreach (Episode ep in episodes)
                {
                    System.Runtime.Serialization.Json.DataContractJsonSerializer g;
                    //await downloadEpisode(ep.url);
                }
                //var xml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                //((XmlElement)xml.GetElementsByTagName("badge")[0]).SetAttribute("value", episodes.Count.ToString());
                //var bu = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
                //bu.Update(new BadgeNotification(xml));
            }
            catch (Exception e)
            {
                // If anything goes wrong, show an error badge on the primary tile
                Debug.WriteLine("Run Exception: "+ e);
                var xml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);
                ((XmlElement)xml.GetElementsByTagName("badge")[0]).SetAttribute("value", "error");
                var bu = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
                bu.Update(new BadgeNotification(xml));
            }

            // Indicate that our work is complete
            deferral.Complete();
        }

        async Task<List<string>> getFavorites()
        {
            StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\data\Favorites.xml");
            string xmlString = await FileIO.ReadTextAsync(file);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            List<string> _podcasts = new List<string>();
            XmlNodeList channels = doc.GetElementsByTagName("url");
            int itemCount = channels.Count;

            foreach (XmlElement elem in channels)
            {
                if (!String.IsNullOrWhiteSpace(elem.InnerText))
                    _podcasts.Add(elem.InnerText);
            }
            return _podcasts;
        }

        async Task downloadEpisode(string url)
        {
            string fileName = url.Substring(url.LastIndexOf("/")+1);
            Debug.WriteLine(fileName);
            IStorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            Debug.WriteLine("file path " +file.Path);
            Debug.WriteLine("url: " + url);
            BackgroundDownloader bd = new BackgroundDownloader();
            DownloadOperation download = bd.CreateDownload(new Uri(url), file);


            //await download.StartAsync();
      
            // Attach progress and completion handlers.
            await HandleDownloadAsync(download, true);

            //var xml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
            //((XmlElement)xml.GetElementsByTagName("badge")[0]).SetAttribute("value", "alert");
            //var bu = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            //bu.Update(new BadgeNotification(xml));
        }




        private async Task HandleDownloadAsync(DownloadOperation download, bool start)
        {
            // Create progress callback
            Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
            // Create cancellation token
            CancellationTokenSource cts = new CancellationTokenSource();

            if (start)
            {
                // Start the download and attach a progress handler.
                await download.StartAsync().AsTask(cts.Token, progressCallback);
            }
            else
            {
                // The download was scheduled in a previous session, re-attach the progress handler.
                await download.AttachAsync().AsTask(cts.Token, progressCallback);
            }
        }

        private void DownloadProgress(DownloadOperation obj)
        {
            Debug.WriteLine("Download progress ..." + obj.Progress.BytesReceived);
        }

        // Enumerate downloads that were running in the background before the app was closed.
        private async Task DiscoverActiveDownloadsAsync()
        {
            var activeDownloads = new List<DownloadOperation>();

            // Get all current download operations
            IReadOnlyList<DownloadOperation> downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();

            if (downloads.Count > 0)
            {
                List<Task> tasks = new List<Task>();
                foreach (DownloadOperation download in downloads)
                {
                    // Attach progress and completion handlers.
                    tasks.Add(HandleDownloadAsync(download, false));
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}
