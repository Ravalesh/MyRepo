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
    [Activity(Label = "AddCoordinatesActivity")]
    public class AddCoordinatesActivity : Activity, IUpdateUi
    {
        EditText _longitudesEditText;
        EditText _latitudesEditText;
        EditText _messageEditText;

        Button _saveButton;
        Button _currentLocationButton;


        GpsServiceConnection _gpsServiceConnection;
        Intent _gpsServiceIntent;

        private BroadCastHandler.MyBroadcastReceiver _receiver;

        public void UpdateUi(Intent intent)
        {
            _longitudesEditText.Text = intent.GetDoubleExtra("Longitude",0).ToString();
            _latitudesEditText.Text = intent.GetDoubleExtra("Latitude",0).ToString();
            UnregisterService();
            UnRegisterBroadcastReceiver();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddCoordinates);
            BroadCastHandler.MyBroadcastReceiver.Instance = this;

            _longitudesEditText = FindViewById<EditText>(Resource.Id.etLongitude);
            _latitudesEditText = FindViewById<EditText>(Resource.Id.etLatitude);
            _messageEditText = FindViewById<EditText>(Resource.Id.etMessage);

            _saveButton = FindViewById<Button>(Resource.Id.btnSave);
            _currentLocationButton = FindViewById<Button>(Resource.Id.btnCurrentLocation);


            _currentLocationButton.Click += (object sender, EventArgs e) =>
            {
                RegisterService();
                RegisterBroadcastReceiver();
            };

            _saveButton.Click += (object sender, EventArgs e) =>
            {
                Preferences.SaveCoodinates(this, _latitudesEditText.Text.Trim(),_longitudesEditText.Text.Trim(), _messageEditText.Text.Trim());
            };

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