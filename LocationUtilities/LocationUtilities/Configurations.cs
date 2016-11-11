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
    class Configurations
    {
        public static readonly string LocationUpdated = "LOCATION_UPDATED";

        public static readonly string AddressArray = "Address_";

        public static readonly string CoordinatesArray = "Coordinates_";
        public static readonly string CoordinatesMessageArray = "Coordinates_Message_";

        public static readonly string AddressRunningCount = "AddressCount";

        public static readonly string CoordinatesRunnungCount = "CoordinatesCount";



    }
}