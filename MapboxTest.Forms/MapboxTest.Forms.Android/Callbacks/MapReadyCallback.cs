using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Widget;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Camera;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Maps;
using Com.Mapbox.Mapboxsdk.Offline;
using Xamarin.Essentials;
using Style = Com.Mapbox.Mapboxsdk.Constants.Style;

namespace MapboxTest.Forms.Droid.Callbacks
{
    public class MapReadyCallback : Java.Lang.Object, IOnMapReadyCallback, MapView.IOnMapChangedListener
    {
        private MapboxMap _mapBoxMap;
        readonly Context _context;

        public MapReadyCallback(Context context)
        {
            _context = context;
        }

        public new void Dispose()
        {
            //Dispose the unwanted or custom loaded elements on the map
        }

        public void OnMapChanged(int p0)
        {
            switch (p0)
            {
                // Can load styles and make sure the map is not doing heavy lifting the Ui thread when the retrofit is trying to resolve the mutex internal error with the running version.
            }
        }

        public async void OnMapReady(MapboxMap mapBoxMap)
        {
            Toast.MakeText(_context,"Mapready invoked",ToastLength.Short).Show();
            _mapBoxMap = mapBoxMap;
            _mapBoxMap.SetStyle(Style.MapboxStreets);
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                //Getting the last downloaded definition to move the camera so that we see something when the map is loaded.
                var definition = await GetDefinition();
                _mapBoxMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition.Builder()
                    .Target(definition.Bounds.Center)
                    .Zoom(definition.MaxZoom).Build()));
            }
            else
            {
                var position = new CameraPosition.Builder()
                    .Target(new LatLng(13.0664, 80.2844))
                    .Zoom(15)
                    .Build();
                _mapBoxMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(position));

                Toast.MakeText(_context,
                        $"Camera location pointed to Anna square, Chennai Lat : 13.0664 / Long: 80.2844",
                        ToastLength.Short)
                    .Show();

                var markerOptions = new MarkerOptions();
                markerOptions.SetPosition(new LatLng(13.0664, 80.2844));
                markerOptions.SetTitle("Anna square");
                markerOptions.SetIcon(IconFactory.GetInstance(_context)
                    .FromResource(Resource.Drawable.mapbox_marker_icon_default));
                
                _mapBoxMap.AddMarker(markerOptions);
            }
        }

        private Task<OfflineTilePyramidRegionDefinition> GetDefinition()
        {
            var tcs = new TaskCompletionSource<OfflineTilePyramidRegionDefinition>();
            OfflineManager.GetInstance(_context).ListOfflineRegions(new OfflineRegionCallback()
            {
                OnListCallback = regions =>
                {
                    tcs.SetResult(regions.Last().Definition as OfflineTilePyramidRegionDefinition);
                }
            });

            return tcs.Task;
        }
    }
}