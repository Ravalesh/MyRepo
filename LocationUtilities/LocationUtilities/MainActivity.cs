using Android.App;
using Android.Widget;
using Android.OS;

namespace LocationUtilities
{
    [Activity(Label = "LocationUtilities", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button addAddress = FindViewById<Button>(Resource.Id.btnAddAddress);
            Button addCoodinates = FindViewById<Button>(Resource.Id.btnAddCoordinates);
            Button nearbyLocations = FindViewById<Button>(Resource.Id.btnNearbyCasinos);

            addAddress.Click += (object sender, System.EventArgs e) =>
            {
                StartActivity(typeof(AddAddressActivity));
            };

            addCoodinates.Click += (object sender, System.EventArgs e) =>
            {
                StartActivity(typeof(AddCoordinatesActivity));
            };

            nearbyLocations.Click += (object sender, System.EventArgs e) =>
            {
                StartActivity(typeof(NearbyCasinosActivity));
            };


        }
    }
}

