using System;
using Android.Content;
using Com.Mapbox.Mapboxsdk.Offline;

namespace MapboxTest.Forms.Droid.Callbacks
{
    public class OfflineRegionDownloadObserver : Java.Lang.Object, OfflineRegion.IOfflineRegionObserver
    {
        private readonly Context _context;
        private int errorThrownIndex = 0;

        public OfflineRegionDownloadObserver(Context context)
        {
            _context = context;
        }

        internal Action<long> OnMapBoxTileCountLimitExceededCallback { get; set; }
        internal Action<OfflineRegionError> OnErrorCallback { get; set; }
        internal Action<OfflineRegionStatus> OnStatusChangedCallback { get; set; }

        public void MapboxTileCountLimitExceeded(long limit)
        {
            //Toast.MakeText(_context, $"Limit exceeded {limit}", ToastLength.Short).Show();
            OnMapBoxTileCountLimitExceededCallback?.Invoke(limit);
        }

        public void OnError(OfflineRegionError error)
        {
            //Toast.MakeText(_context, $"{error.Message} - {error.Reason}", ToastLength.Short).Show();
            if (errorThrownIndex == 0)
            {
                OnErrorCallback?.Invoke(error);
            }

            errorThrownIndex = 1;
        }

        public void OnStatusChanged(OfflineRegionStatus status)
        {
            OnStatusChangedCallback?.Invoke(status);
        }
    }
}