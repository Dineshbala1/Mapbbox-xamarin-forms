using Android.OS;
using Android.Views;
using Com.Mapbox.Mapboxsdk.Maps;

namespace MapboxTest.Forms.Droid.Controls
{
    public class MapViewFragment : SupportMapFragment
    {

        public MapView MapView { get; private set; }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            MapView = view as MapView;
            //MapView?.AddOnMapChangedListener(this);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
    }
}