using System.ComponentModel;
using Android.Content;
using Com.Mapbox.Mapboxsdk.Maps;
using MapboxTest.Forms.Controls;
using MapboxTest.Forms.Droid.Callbacks;
using MapboxTest.Forms.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

/*[assembly: ExportRenderer(typeof(MapBoxMapView), typeof(MapBoxMapViewRenderer))]*/
namespace MapboxTest.Forms.Droid.Renderer
{
    class MapBoxMapViewRenderer : ViewRenderer<MapBoxMapView, MapView>
    {

        private MapView _mapView;

        public MapBoxMapViewRenderer(Context context) : base(context)
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _mapView.Dispose();
                _mapView = null;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MapBoxMapView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                _mapView = new MapView(Context);
                _mapView.GetMapAsync(new MapReadyCallback(Context));
                SetNativeControl(_mapView);
            }
        }

        //TODO: Future changes if any required.
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}