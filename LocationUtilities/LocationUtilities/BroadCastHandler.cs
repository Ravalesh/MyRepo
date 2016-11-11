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
    class BroadCastHandler
    {
        [BroadcastReceiver]
        internal class MyBroadcastReceiver : BroadcastReceiver
        {
            public static IUpdateUi Instance;
            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action.Equals(Configurations.LocationUpdated))
                {
                    if (Instance != null)
                    {
                        Instance.UpdateUi(intent);
                    }
                }
            }
        }
    }
}