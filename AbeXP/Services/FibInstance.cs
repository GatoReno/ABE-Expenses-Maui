using AbeXP.Interfaces;
using AbeXP.UseCases.Plugins;
using Firebase.Database;

namespace AbeXP.Services
{
    public sealed class FibInstance(ISettingsService settingsService, IUserSession userSession) : IFibInstance
    {
        private readonly ISettingsService _settingsService = settingsService;
        private readonly IUserSession _userSession = userSession;
        private FirebaseClient? _instance;

        public FirebaseClient GetInstance()
        {
            if (_instance is null)
                _instance = new FirebaseClient(_settingsService.FireBaseRef, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = async () => await _userSession.GetTokenAsync()
                });

            return _instance;
        }
    }
}

