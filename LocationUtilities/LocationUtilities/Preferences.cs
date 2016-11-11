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
using Android.Preferences;

namespace LocationUtilities
{
    public static class Preferences
    {
        private static readonly object _recordLock = new object();

        public static void SaveAddress(Context c, string address)
        {
            lock(_recordLock)
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(c);
                var edit = prefs.Edit(); 
                edit.PutString(Configurations.AddressArray + prefs.GetInt(Configurations.AddressRunningCount, 0) + 1, address);
                edit.PutInt(Configurations.AddressRunningCount, prefs.GetInt(Configurations.AddressRunningCount, 0) + 1);
                edit.Commit();
            }
        }

        public static string[] GetAddress(Context c)
        {
            string[] result;
            lock (_recordLock)
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(c);
                int count = prefs.GetInt(Configurations.AddressRunningCount, 0);
                result = new string[count];
                for (int i = 1; i < count; i++)
                {
                    result[i-1] = prefs.GetString(Configurations.AddressArray + count, "");
                }
            }
            return result;
        }


        public static void SaveCoodinates(Context c, string latitude, string longitude, string message)
        {
            lock (_recordLock)
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(c);
                var edit = prefs.Edit();
                edit.PutString(Configurations.CoordinatesArray + prefs.GetInt(Configurations.CoordinatesRunnungCount, 0) + 1, latitude + "," + longitude);
                edit.PutString(Configurations.CoordinatesMessageArray + prefs.GetInt(Configurations.CoordinatesRunnungCount, 0) + 1, message);
                edit.PutInt(Configurations.CoordinatesRunnungCount, prefs.GetInt(Configurations.CoordinatesRunnungCount, 0) + 1);
                edit.Commit();
            }
        }

        public static NotificationObject[] GetCoordinates(Context c)
        {
            NotificationObject[] result;
            lock (_recordLock)
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(c);
                int count = prefs.GetInt(Configurations.CoordinatesRunnungCount, 0);
                result = new NotificationObject[count];
                for (int i = 1; i < count; i++)
                {
                    result[i - 1].Coordinates = prefs.GetString(Configurations.CoordinatesArray + count, "");
                    result[i - 1].Message = prefs.GetString(Configurations.CoordinatesMessageArray + count, "");
                }
            }
            return result;
        }

    }
}