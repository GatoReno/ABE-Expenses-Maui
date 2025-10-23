using Firebase.Core;
using Foundation;
using UIKit;

namespace AbeXP;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        if (Firebase.Core.App.DefaultInstance == null)
        {
            Firebase.Core.App.Configure();
        }

        return base.FinishedLaunching(application, launchOptions);
    }
}
