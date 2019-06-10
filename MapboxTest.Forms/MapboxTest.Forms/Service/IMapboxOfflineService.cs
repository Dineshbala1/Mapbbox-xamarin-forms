using System;
using System.Threading.Tasks;
using MapboxTest.Common;

namespace MapboxTest.Forms.Service
{
    public interface IMapboxOfflineService
    {
        Task<OfflineRegionDto[]> GetOfflineRegions();

        Task<OfflineRegionDto> CreateOfflineRegion(LatLngBoundsDto bounds, int zoomLevel = 8);

        void DownloadOfflineRegion(OfflineRegionDto region);

        void Resume(OfflineRegionDto region);

        void Pause(OfflineRegionDto region);

        event EventHandler<DownloadStatusEventArgs> OfflineDownloadProgress;

        event EventHandler<EventArgs<string>> OfflineDownloadError;

        Task UpdateExistingDownloadedDefinition();

        Task<bool> IsRegionDownloadCompleted(OfflineRegionDto regionDto);
    }
}
