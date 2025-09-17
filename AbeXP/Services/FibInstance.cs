using AbeXP.Interfaces;
using Firebase.Database;

namespace AbeXP.Services
{
    public sealed class FibInstance(ISettingsService settingsService) : IFibInstance
    {
        private readonly ISettingsService _settingsService = settingsService;
        private FirebaseClient? _instance;

        public FirebaseClient GetInstance()
        {
            if (_instance is null)
                _instance = new FirebaseClient(_settingsService.FireBaseRef);

            return _instance;
        }
    }
}

