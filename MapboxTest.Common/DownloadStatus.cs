using System.Collections.Generic;
using System.Text;

namespace MapboxTest.Common
{
    public class DownloadStatus 
    {
        public double CountOfResourcesCompleted { get; set; }

        public double CountOfBytesCompleted { get; set; }

        public double CountOfTilesCompleted { get; set; }

        public double CountOfResourcesExpected { get; set; }

        public double MaximumResourcesExpected { get; set; }

        public double CountOfTileBytesCompleted { get; set; }

        public DownloadState DownloadState { get; set; }
    }
}
