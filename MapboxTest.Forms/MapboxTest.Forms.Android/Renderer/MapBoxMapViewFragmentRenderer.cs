using Android.Content;
using Android.Support.V7.App;
using Android.Widget;
using MapboxTest.Forms.Controls;
using MapboxTest.Forms.Droid.Callbacks;
using MapboxTest.Forms.Droid.Controls;
using MapboxTest.Forms.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AndroidView = Android.Views.View;

/*This implementation of the Mapview is more stable as per the comments from the Mapbox SDK i.e. considering the version of the mapbox sdk.
 Using the mapview is usable but it's not reliable, need more time to investigate with an additional log creation to initiate a mapchanged sequence and identify the 
 failure in OnMapReady callback. If that is stable and fixed we can use it all the way in the client.*/
[assembly: ExportRenderer(typeof(MapBoxMapView), typeof(MapBoxMapViewFragmentRenderer))]
namespace MapboxTest.Forms.Droid.Renderer
{
    class MapBoxMapViewFragmentRenderer : ViewRenderer<MapBoxMapView, AndroidView>
    {
        MapViewFragment _mapViewFragment;

        public MapBoxMapViewFragmentRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<MapBoxMapView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var formsAppCompatActivity = (AppCompatActivity) Context;
                var fragmentContainer = new FrameLayout(Context)
                {
                    Id = GenerateViewId()
                };

                SetNativeControl(fragmentContainer);

                _mapViewFragment = new MapViewFragment();

                formsAppCompatActivity.SupportFragmentManager.BeginTransaction().Replace(fragmentContainer.Id, _mapViewFragment)
                    .CommitAllowingStateLoss();
                _mapViewFragment.GetMapAsync(new MapReadyCallback(Context));
            }
        }
    }
}