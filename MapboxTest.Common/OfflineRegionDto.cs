namespace MapboxTest.Common
{
    public class OfflineRegionDto
    {
        public string Style { get; set; }

        public LatLngBoundsDto Bounds { get; set; }

        public double MaximumZoomLevel { get; set; }

        public double MinimumZoomLevel { get; set; }

        public long Id { get; set; }

        public DownloadState DownloadState { get; set; }

        public DownloadStatus DownloadStatus { get; set; }
    }
}
