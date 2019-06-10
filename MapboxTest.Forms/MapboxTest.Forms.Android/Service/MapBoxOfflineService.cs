using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Com.Mapbox.Mapboxsdk.Constants;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Offline;
using MapboxTest.Common;
using MapboxTest.Common.GeoCalculator;
using MapboxTest.Forms.Droid.Callbacks;
using MapboxTest.Forms.Droid.Service;
using MapboxTest.Forms.Service;
using Org.Json;
using Polly;
using Xamarin.Essentials;
using DownloadStatus = MapboxTest.Common.DownloadStatus;

[assembly: Xamarin.Forms.Dependency(typeof(MapBoxOfflineService))]
namespace MapboxTest.Forms.Droid.Service
{
    class MapBoxOfflineService : IMapboxOfflineService
    {
        private OfflineManager _offlineManager;

        public MapBoxOfflineService()
        {
            _offlineManager = OfflineManager.GetInstance(Application.Context);
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, Xamarin.Essentials.ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                _offlineManager = null;
                _offlineManager = OfflineManager.GetInstance(Application.Context);
            }
        }

        public Task<OfflineRegionDto[]> GetOfflineRegions()
        {
            var tcs = new TaskCompletionSource<OfflineRegionDto[]>();
            _offlineManager.ListOfflineRegions(new OfflineRegionCallback()
            {
                OnListCallback = regions => { tcs.SetResult(regions.Select(Transform).ToArray()); },
                OnErrorCallback = offlineRegionError => { tcs.SetResult(null); }
            });
            return tcs.Task;
        }

        public Task<OfflineRegionDto> CreateOfflineRegion(LatLngBoundsDto bounds, int zoomLevel = 8)
        {
            var tcs = new TaskCompletionSource<OfflineRegionDto>();

            LatLngBounds latLngBounds = new LatLngBounds.Builder()
                .Include(new LatLng(bounds.Ne.Latitude, bounds.Ne.Longitude))
                .Include(new LatLng(bounds.Sw.Latitude, bounds.Sw.Longitude))
                .Build();

            var offlineDefinition = new OfflineTilePyramidRegionDefinition(Style.MapboxStreets, latLngBounds, 1,
                zoomLevel,
                Application.Context.Resources.DisplayMetrics.Density);

            var jsonObject = new JSONObject();
            jsonObject.Put("REGION_NAME", "Chennai");
            string json = jsonObject.ToString();
            byte[] metadata = Encoding.UTF8.GetBytes(json);

            _offlineManager.CreateOfflineRegion(offlineDefinition, metadata,
                new CreateOfflineRegionCallback(Application.Context)
                {
                    OnListCallback = region =>
                    {
                        region.SetDownloadState(OfflineRegion.StateActive);
                        tcs.SetResult(Transform(region));
                    },
                    OnErrorCallback = error => { tcs.SetResult(null); }
                });

            return tcs.Task;
        }

        /*
         * If the device has network connectivity, the Maps SDK for Android or iOS will make periodic network requests to revalidate cached tiles
         * and other resources if the Cache-Control or Expires HTTP response headers have expired.
         * If an updated resource is available, it will replace the older version in the offline database.
         * When the SDK automatically updates offline map tiles, the offline region is not re-download from scratch.
         * The offline tile update process is the same process as with regular map tiles:
         * The map tile's only downloaded if there's a new version of that tile.
         */

        public async void DownloadOfflineRegion(OfflineRegionDto regionDto)
        {
            var regions = await GetMapBoxOfflineRegions();
            var region = regions.FirstOrDefault(d => d.ID == regionDto.Id);
            if (region == null)
            {
                return;
            }

            region.SetDownloadState(OfflineRegion.StateActive);
            region.SetObserver(new OfflineRegionDownloadObserver(Application.Context)
            {
                OnStatusChangedCallback = status =>
                {
                    regionDto.DownloadStatus = new DownloadStatus
                    {
                        CountOfResourcesExpected = (ulong) status.RequiredResourceCount,
                        CountOfResourcesCompleted = (ulong) status.CompletedResourceCount,
                        CountOfTilesCompleted = (ulong) status.CompletedTileCount,
                        CountOfTileBytesCompleted = (ulong) status.CompletedTileSize,
                        CountOfBytesCompleted = (ulong) status.CompletedResourceSize,
                        MaximumResourcesExpected = (ulong) status.RequiredResourceCount
                    };

                    if (status.IsComplete)
                    {
                        regionDto.DownloadState = DownloadState.Completed;
                    }
                    else if (status.DownloadState == OfflineRegion.StateActive)
                    {
                        regionDto.DownloadState = DownloadState.Active;
                    }
                    else
                    {
                        regionDto.DownloadState = DownloadState.Inactive;
                    }

                    OfflineDownloadProgress?.Invoke(this, new DownloadStatusEventArgs(regionDto));
                },

                OnErrorCallback = error =>
                {
                    OfflineDownloadError?.Invoke(this, new EventArgs<string>($"{error.Reason} - {error.Message}"));
                    region.SetDownloadState(OfflineRegion.StateInactive);
                },

                OnMapBoxTileCountLimitExceededCallback = l =>
                {
                    Toast.MakeText(Application.Context, "Limit exceeded", ToastLength.Long).Show();
                }
            });
        }

        public void Resume(OfflineRegionDto region)
        {
            throw new NotImplementedException();
        }

        public void Pause(OfflineRegionDto region)
        {
            throw new NotImplementedException();
        }

        public Task UpdateExistingDownloadedDefinition()
        {
            var tcs = new TaskCompletionSource<Task>();
            _offlineManager.ListOfflineRegions(new OfflineRegionCallback()
            {
                OnListCallback = regions =>
                {
                    //Resetting this to active state to make sure that the downloaded items are available
                    foreach (var offlineRegion in regions)
                    {
                        offlineRegion.GetStatus(new OfflineRegionStatusCallback()
                        {
                            OnStatusCallback = status =>
                            {
                                if (status.DownloadState == OfflineRegion.StateActive || status.IsComplete)
                                {
                                    offlineRegion.SetDownloadState(OfflineRegion.StateActive);
                                }
                            }
                        });
                    }

                    tcs.SetResult(Task.FromResult(1));
                }
            });

            return tcs.Task;
        }

        public async Task<bool> IsRegionDownloadCompleted(OfflineRegionDto regionDto)
        {
            var regions = await GetMapBoxOfflineRegions();
            var requiredRegion = regions.SingleOrDefault(x => x.ID == regionDto.Id);
            if (requiredRegion != null)
            {
                return await GetStatusRegionDto(requiredRegion) == DownloadState.Completed;
            }

            return false;
        }

        private Task<DownloadState> GetStatusRegionDto(OfflineRegion offlineRegion)
        {
            var tcs = new TaskCompletionSource<DownloadState>();
            offlineRegion.GetStatus(new OfflineRegionStatusCallback
            {
                OnStatusCallback = status =>
                {
                    if (status.IsComplete)
                    {
                        tcs.TrySetResult(DownloadState.Completed);
                    }
                    else if (status.RequiredResourceCount == status.CompletedResourceCount)
                    {
                        tcs.TrySetResult(DownloadState.Inactive);
                    }
                    else
                    {
                        tcs.TrySetResult(DownloadState.Inactive);
                    }
                }
            });

            return tcs.Task;
        }

        public event EventHandler<DownloadStatusEventArgs> OfflineDownloadProgress;
        public event EventHandler<EventArgs<string>> OfflineDownloadError;

        void DeleteMapboxOfflineRegions()
        {
            _offlineManager.ListOfflineRegions(new OfflineRegionCallback()
            {
                OnListCallback = regions =>
                {
                    for (int i = 0; i < regions.Length; i++)
                    {
                        regions[i].Delete(new OfflineRegionDeleteCallback(Application.Context));
                    }
                }
            });
        }

        Task<OfflineRegion[]> GetMapBoxOfflineRegions()
        {
            var tcs = new TaskCompletionSource<OfflineRegion[]>();
            _offlineManager.ListOfflineRegions(new OfflineRegionCallback()
            {
                OnErrorCallback = ((msg) =>
                {
                    tcs.TrySetResult(null);
                }),
                OnListCallback = ((regs) =>
                {
                    tcs.TrySetResult(regs);
                })
            });
            return tcs.Task;
        }

        private OfflineRegionDto Transform(OfflineRegion region)
        {
            var definition = region.Definition as OfflineTilePyramidRegionDefinition;
            var regionDto = new OfflineRegionDto()
            {
                Id = region.ID,
                Bounds = new LatLngBoundsDto()
                {
                    Ne = new Coordinate
                    {
                        Latitude = definition.Bounds.NorthEast.Latitude,
                        Longitude = definition.Bounds.NorthEast.Longitude
                    },
                    Sw = new Coordinate
                    {
                        Latitude = definition.Bounds.SouthWest.Latitude,
                        Longitude = definition.Bounds.SouthWest.Longitude
                    }
                },
                Style = definition.StyleURL,
                MinimumZoomLevel = definition.MinZoom,
                MaximumZoomLevel = definition.MaxZoom
            };
           
            return regionDto;
        }
    }
}