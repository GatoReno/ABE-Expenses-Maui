using System;
using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using Firebase.Database;
using Google.Cloud.Firestore;

namespace AbeXP.Services
{
	public sealed class FibInstance(ISettingsService settingsService) : IFibInstance
	{
        private readonly ISettingsService _settingsService = settingsService;
        private FirestoreDb _instance;

        public FirestoreDb GetInstance()
        {
            if (_instance == null)
            {
                _instance = FirestoreDb.Create(FirebaseConstants.PROJECTID);
            }

            return _instance;
        }
    }
}

