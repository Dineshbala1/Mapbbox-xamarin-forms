using System;
using Android.Content;
using Com.Mapbox.Mapboxsdk.Offline;

namespace MapboxTest.Forms.Droid.Callbacks
{
    public class CreateOfflineRegionCallback : Java.Lang.Object, OfflineManager.ICreateOfflineRegionCallback
    {
        private readonly Context _context;

        public CreateOfflineRegionCallback(Context context)
        {
            _context = context;
        }

        public Action<OfflineRegion> OnListCallback;
        public Action<string> OnErrorCallback;

        public void OnCreate(OfflineRegion region)
        {
            OnListCallback?.Invoke(region);
        }

        public void OnError(string error)
        {
           OnErrorCallback?.Invoke(error);
        }
    }
}