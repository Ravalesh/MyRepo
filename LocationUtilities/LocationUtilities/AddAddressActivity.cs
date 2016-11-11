using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LocationUtilities
{
    [Activity(Label = "AddAddressActivity")]
    public class AddAddressActivity : Activity, IUpdateUi
    {
        TextView _addressTextView;
        Button _getCurrentAddress;
        Button _addNewAddress;

        GpsServiceConnection _gpsServiceConnection;
        Intent _gpsServiceIntent;

        private BroadCastHandler.MyBroadcastReceiver _receiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddAddresses);
            BroadCastHandler.MyBroadcastReceiver.Instance = this;
            _addressTextView = FindViewById<TextView>(Resource.Id.tvAddress);
            _getCurrentAddress = FindViewById<Button>(Resource.Id.btnCurrentAddress);

            _getCurrentAddress.Click += (object sender, EventArgs e) =>
                {
                    RegisterService();
                    RegisterBroadcastReceiver();
                };

            _addNewAddress = FindViewById<Button>(Resource.Id.btnSave);

            _addNewAddress.Click += (object sender, EventArgs e) =>
            {
                Preferences.SaveAddress(this, _addressTextView.Text);
            };
        }

        public void UpdateUi(Intent intent)
        {
            _addressTextView.Text = intent.GetStringExtra("Address");
            UnregisterService();
            UnRegisterBroadcastReceiver();
        }

        private void RegisterService()
        {
            _gpsServiceConnection = new GpsServiceConnection();
            _gpsServiceIntent = new Intent(Application.Context, typeof(GpsService));
            BindService(_gpsServiceIntent, _gpsServiceConnection, Bind.AutoCreate);
        }

        private void RegisterBroadcastReceiver()
        {
            IntentFilter filter = new IntentFilter(Configurations.LocationUpdated);
            filter.AddCategory(Intent.CategoryDefault);
            _receiver = new BroadCastHandler.MyBroadcastReceiver();
            RegisterReceiver(_receiver, filter);
        }

        private void UnRegisterBroadcastReceiver()
        {
            UnregisterReceiver(_receiver);
        }

        private void UnregisterService()
        {
            if (_gpsServiceConnection != null)
            {
                _gpsServiceConnection.StopLocationService();
                UnbindService(_gpsServiceConnection);
                _gpsServiceConnection = null;
                _gpsServiceIntent.Dispose();
            }
            
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            BroadCastHandler.MyBroadcastReceiver.Instance = null;
            Finish();
        }
    }
}