using Microsoft.Maui.LifecycleEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Extensions
{
    internal static class MauiAppBuilderExtensions
    {
        public static MauiAppBuilder RegisterFirebase(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if IOS
		events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) =>
        {							
            Firebase.Core.App.Configure();			
            Firebase.Crashlytics.Crashlytics.SharedInstance.Init();
            Firebase.Crashlytics.Crashlytics.SharedInstance.SetCrashlyticsCollectionEnabled(true);
            Firebase.Crashlytics.Crashlytics.SharedInstance.SendUnsentReports();
            return false; 		
		}));
#else
                events.AddAndroid(android => android.OnCreate((activity, bundle) =>
                {
                    Firebase.FirebaseApp.InitializeApp(activity);
                    Firebase.Crashlytics.FirebaseCrashlytics.Instance.SetCrashlyticsCollectionEnabled(Java.Lang.Boolean.True);
                    Firebase.Crashlytics.FirebaseCrashlytics.Instance.SendUnsentReports();
                }));
#endif
            });

            return builder;
        }
    }
}
