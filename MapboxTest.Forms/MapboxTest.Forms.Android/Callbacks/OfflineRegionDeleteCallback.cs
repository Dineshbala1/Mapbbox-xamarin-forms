using Android.Content;
using Com.Mapbox.Mapboxsdk.Offline;

namespace MapboxTest.Forms.Droid.Callbacks
{
    public class OfflineRegionDeleteCallback : Java.Lang.Object, OfflineRegion.IOfflineRegionDeleteCallback
    {
        private readonly Context _context;

        public OfflineRegionDeleteCallback(Context context)
        {
            _context = context;
        }

        public void OnDelete()
        {
         //   Toast.MakeText(_context, "Region delete completed", ToastLength.Short).Show();
        }

        public void OnError(string error)
        {
         //   Toast.MakeText(_context, error, ToastLength.Short).Show();
        }
    }
}