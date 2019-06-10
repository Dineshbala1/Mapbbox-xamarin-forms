using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MapboxTest.Common;
using MapboxTest.Common.GeoCalculator;
using MapboxTest.Forms.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MapboxTest.Forms.ViewModel
{
    class MainPageViewModel : BaseViewModel
    {
        
        private int _downloadedPercentage;

        public MainPageViewModel()
        {
            PageTitle = "Download Maps";
            MapBoxOfflineService.OfflineDownloadProgress += MapBoxOfflineServiceOfflineDownloadProgress;
            MapBoxOfflineService.OfflineDownloadError += MapBoxOfflineServiceOfflineDownloadError;

            DownloadCommand = new Command(async () => { await ExecuteDownloadCommand(); }, CanExecuteDownloadCommand);
            NavigateToMapPageCommand = new Command(ExecuteNavigateToMapPageCommand);
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            (DownloadCommand as Command)?.ChangeCanExecute();
        }

        private bool CanExecuteDownloadCommand()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }

        public ICommand DownloadCommand { get; }
        public ICommand NavigateToMapPageCommand { get; }

        public int DownloadedPercentage
        {
            get => _downloadedPercentage;
            set
            {
                _downloadedPercentage = value;
                OnPropertyChanged();
            }
        }

        private void MapBoxOfflineServiceOfflineDownloadError(object sender, EventArgs<string> e)
        {
            PopupService.SendPopupAlertToMessagingCenter(new MessageBlock
                {Title = "Error", Message = e.Data, OkButton = "Ok"});
            IsBusy = false;
            DownloadedPercentage = 0;
        }

        private void MapBoxOfflineServiceOfflineDownloadProgress(object sender, DownloadStatusEventArgs e)
        {
            var progress = e.OfflineRegion.DownloadStatus;
            if (progress.CountOfResourcesExpected > 0)
            {
                DownloadedPercentage =
                    (int) (progress.CountOfResourcesCompleted / progress.CountOfResourcesExpected * 100);
            }

            if (e.OfflineRegion.DownloadState == DownloadState.Completed)
            {
                DownloadedPercentage = 0;
                IsBusy = false;
                PopupService.SendPopupAlertToMessagingCenter(new MessageBlock
                    {Title = "Alert", Message = "Download Successful", OkButton = "Ok"});
            }
        }

        private async void ExecuteNavigateToMapPageCommand()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                var regions = await MapBoxOfflineService.GetOfflineRegions();
                if (regions != null && regions.Length > 0 && await CheckAnyRegionIsFullyDownloaded(regions))
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new MapPage());
                }
                else
                {
                    PopupService.SendPopupAlertToMessagingCenter(new MessageBlock
                    {
                        Title = "Warning", Message = "Please download a region before trying to view a offline map",
                        OkButton = "Ok"
                    });
                }
            }
            else
            {
                await Application.Current.MainPage.Navigation.PushAsync(new MapPage());
            }
        }

        private async Task<bool> CheckAnyRegionIsFullyDownloaded(OfflineRegionDto[] regionsList)
        {
            if (regionsList != null && regionsList.Any())
            {
                foreach (var region in regionsList)
                {
                    if (await MapBoxOfflineService.IsRegionDownloadCompleted(region))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task ExecuteDownloadCommand()
        {
            IsBusy = true;
            var offlineRegion = await MapBoxOfflineService.CreateOfflineRegion(new LatLngBoundsDto
            {
                Ne = new Coordinate {Latitude = 13.418633, Longitude = 80.314072},
                Sw = new Coordinate {Latitude = 12.6299041905939, Longitude = 79.8215715105946}
            });

            if (offlineRegion != null)
            {
                MapBoxOfflineService.DownloadOfflineRegion(offlineRegion);
            }
        }
    }
}
