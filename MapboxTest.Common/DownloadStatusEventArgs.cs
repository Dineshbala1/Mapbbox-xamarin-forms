using System;

namespace MapboxTest.Common
{
    public class DownloadStatusEventArgs : EventArgs
    {
        public OfflineRegionDto OfflineRegion { get;}

        public DownloadStatusEventArgs(OfflineRegionDto downloadStatus)
        {
            OfflineRegion = downloadStatus;
        }
    }
}