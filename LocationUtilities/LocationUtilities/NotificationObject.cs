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
    public class NotificationObject
    {
        public string Coordinates { get; set; }

        public string Message { get; set; }
    }
}