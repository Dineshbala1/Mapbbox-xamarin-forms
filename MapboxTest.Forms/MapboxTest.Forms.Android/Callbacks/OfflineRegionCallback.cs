using System;
using Com.Mapbox.Mapboxsdk.Offline;

namespace MapboxTest.Forms.Droid.Callbacks
{
    class OfflineRegionCallback : Java.Lang.Object, OfflineManager.IListOfflineRegionsCallback
    {
        public Action<OfflineRegion[]> OnListCallback;
        public Action<string> OnErrorCallback;

        public void OnError(string error)
        {
            OnErrorCallback?.Invoke(error);
        }

        public void OnList(OfflineRegion[] offlineRegions)
        {
            OnListCallback(offlineRegions);
        }
    }

    class OfflineRegionStatusCallback : Java.Lang.Object, OfflineRegion.IOfflineRegionStatusCallback
    {

        public Action<OfflineRegionStatus> OnStatusCallback;
        public Action<string> OnErrorCallback;

        public void OnError(string error)
        {
            OnErrorCallback(error);
        }

        public void OnStatus(OfflineRegionStatus status)
        {
            OnStatusCallback(status);
        }
    }
}