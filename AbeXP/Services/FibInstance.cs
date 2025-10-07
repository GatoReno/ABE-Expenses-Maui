using AbeXP.Abstractions.Interfaces;
using AbeXP.Interfaces;
using AbeXP.UseCases.Plugins;
using Firebase.Database;

namespace AbeXP.Services
{
    public sealed class FibInstance(ISettingsService settingsService, IFibAuthLog fibAuthLog) : IFibInstance
    {
        private readonly ISettingsService _settingsService = settingsService;
        private readonly IFibAuthLog _fibAuthLog = fibAuthLog;
        private FirebaseClient? _instance;

        public FirebaseClient GetInstance()
        {
            if (_instance is null)
                _instance = new FirebaseClient(_settingsService.FireBaseRef, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = async () => await _fibAuthLog.GetValidTokenAsync()
                });

            return _instance;
        }
    }
}

