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
using Android.Locations;
using System.Timers;

namespace LocationUtilities
{
    [Service]
    public class GpsService : Service, ILocationListener
    {
        protected LocationManager _locationManager = (LocationManager)Application.Context.GetSystemService(LocationService);

        protected LocationManager _locationManager2 = (LocationManager)Application.Context.GetSystemService(LocationService);

        private Location _currentLocation;

        private string _address;
        Timer _timer;
        private readonly int TwoMinutes = 2;

        public override IBinder OnBind(Intent intent)
        {
            return new GpsServiceBinder(this);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public void StartLocationService()
        {
            Criteria criteriaForGPSService = new Criteria
            {
                //A constant indicating an approximate accuracy  
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };

            var locationProvider = _locationManager.GetBestProvider(criteriaForGPSService, true);
            _locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

            criteriaForGPSService = new Criteria
            {
                //A constant indicating an approximate accuracy  
                Accuracy = Accuracy.Fine,
                PowerRequirement = Power.Medium
            };

            locationProvider = _locationManager.GetBestProvider(criteriaForGPSService, true);
            _locationManager2.RequestLocationUpdates(locationProvider, 0, 0, this);

            _timer = new Timer(5000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public void StopLocationUpdates()
        {
            _locationManager.RemoveUpdates(this);
            _locationManager2.RemoveUpdates(this);
            _timer.Stop();
            _timer.Dispose();
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                Address addressCurrent;
                if (location == null)
                    return;
                else
                {
                    //Start Timer and run for 5 seconds to detemine the optimal location.
                    
                    if (IsBetterLocation(location, _currentLocation))
                    {
                        _currentLocation = location;

                        Geocoder geocoder = new Geocoder(this);

                        //The Geocoder class retrieves a list of address from Google over the internet  
                        IList<Address> addressList = geocoder.GetFromLocation(location.Latitude, location.Longitude, 10);

                        addressCurrent = addressList.FirstOrDefault();
                        if (addressCurrent != null)
                        {
                            StringBuilder deviceAddress = new StringBuilder();

                            for (int i = 0; i < addressCurrent.MaxAddressLineIndex; i++)
                                deviceAddress.Append(addressCurrent.GetAddressLine(i))
                                    .AppendLine(",");

                            _address = deviceAddress.ToString();

                        }
                        else
                            _address = "Unable to determine the address.";
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_currentLocation != null)
            {
                _timer.Stop();

                //Send the broadcast to the receiver

                Intent intent = new Intent(this, typeof(BroadCastHandler.MyBroadcastReceiver));

                intent.SetAction(Configurations.LocationUpdated);
                intent.AddCategory(Intent.CategoryDefault);
                intent.PutExtra("Longitude", _currentLocation.Longitude);
                intent.PutExtra("Latitude", _currentLocation.Latitude);
                intent.PutExtra("Address", _address);

                SendBroadcast(intent);
            }
           
        }

        #region Get the most accurate result

        private bool IsBetterLocation(Location location, Location currentBestLocation)
        {
            if (currentBestLocation == null)
            {
                // A new location is always better than no location
                return true;
            }

            long timeDelta = location.Time - currentBestLocation.Time;

            bool isSignificantlyNewer = timeDelta > TwoMinutes;

            bool isSignificantlyOlder = timeDelta < -TwoMinutes;

            bool isNewer = timeDelta > 0;

            // If it's been more than two minutes since the current location, use the new location
            // because the user has likely moved
            if (isSignificantlyNewer)
            {
                return true;
            }
            else if (isSignificantlyOlder)
            {
                return false;
            }

            // Check whether the new location fix is more or less accurate
            int accuracyDelta = (int)(location.Accuracy - currentBestLocation.Accuracy);
            bool isLessAccurate = accuracyDelta > 0;
            bool isMoreAccurate = accuracyDelta < 0;
            bool isSignificantlyLessAccurate = accuracyDelta > 200;

            // Check if the old and new location are from the same provider
            bool isFromSameProvider = IsSameProvider(location.Provider, currentBestLocation.Provider);

            // Determine location quality using a combination of timeliness and accuracy
            if (isMoreAccurate)
            {
                return true;
            }
            else if (isNewer && !isLessAccurate)
            {
                return true;
            }
            else if (isNewer && !isSignificantlyLessAccurate && isFromSameProvider)
            {
                return true;
            }

            return false;
        }

        private bool IsSameProvider(string provider1, string provider2)
        {
            if (provider1 == null)
            {
                return (provider2 == null);
            }

            return (provider1 == provider2);
        }




        #endregion

        public void OnProviderDisabled(string provider)
        {
           
        }

        public void OnProviderEnabled(string provider)
        {
           
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            
        }
    }

    public class GpsServiceBinder: Binder
    {
        public GpsService Service
        {
            get
            {
                return _gpsService;
            }
        }

        protected GpsService _gpsService;
        public GpsServiceBinder(GpsService service)
        {
            _gpsService = service;
        }
    }

    public class GpsServiceConnection : Java.Lang.Object, IServiceConnection
    {
        GpsServiceBinder _binder;
       
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            _binder = (GpsServiceBinder)service;

            if (_binder != null)
            {
                _binder.Service.StartLocationService();
            }
        }

        public void StopLocationService()
        {
            if (_binder != null)
            {
                _binder.Service.StopLocationUpdates();
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            
        }
    }

}