using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AbeXP.Interfaces;
using Microsoft.Maui.ApplicationModel;

#if ANDROID
using Android.OS;
using Firebase.Analytics;
#endif

#if IOS
using Firebase.Core;
using Foundation;
#endif

namespace AbeXP.Services
{
    internal sealed class FirebaseAnalyticsService : IAnalyticsService
    {
        private const string DefaultEventName = "app_event";

        public async Task LogEventAsync(string eventName, IDictionary<string, string>? properties = null, CancellationToken cancellationToken = default)
        {
#if ANDROID
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var firebaseAnalytics = FirebaseAnalytics.GetInstance(Android.App.Application.Context);
                var bundle = new Bundle();
                if (properties != null)
                {
                    foreach (var kvp in properties.Take(25))
                    {
                        bundle.PutString(NormalizeParameterName(kvp.Key), TruncateValue(kvp.Value));
                    }
                }

                firebaseAnalytics.LogEvent(NormalizeEventName(eventName), bundle);
            });
#elif IOS
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Firebase.Core.App.DefaultInstance == null)
                {
                    Firebase.Core.App.Configure();
                }

                NSDictionary<NSString, NSObject>? parameters = null;
                if (properties != null && properties.Count > 0)
                {
                    var keys = new List<NSString>();
                    var values = new List<NSObject>();

                    foreach (var kvp in properties.Take(25))
                    {
                        keys.Add(new NSString(NormalizeParameterName(kvp.Key)));
                        values.Add(new NSString(TruncateValue(kvp.Value)));
                    }

                    parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values.ToArray(), keys.ToArray(), keys.Count);
                }

                Firebase.Analytics.Analytics.LogEvent(NormalizeEventName(eventName), parameters);
            });
#else
            await Task.CompletedTask;
#endif
        }

        private static string NormalizeEventName(string? eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return DefaultEventName;
            }

            var normalized = eventName.Trim().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"[^a-z0-9_]", "_");

            if (char.IsDigit(normalized[0]))
            {
                normalized = $"e_{normalized}";
            }

            return normalized.Length > 40 ? normalized.Substring(0, 40) : normalized;
        }

        private static string NormalizeParameterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "param";
            }

            var normalized = name.Trim().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"[^a-z0-9_]", "_");
            if (char.IsDigit(normalized[0]))
            {
                normalized = $"p_{normalized}";
            }

            return normalized.Length > 40 ? normalized.Substring(0, 40) : normalized;
        }

        private static string TruncateValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Length > 100 ? value.Substring(0, 100) : value;
        }
    }
}
