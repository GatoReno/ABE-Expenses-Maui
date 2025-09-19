using AbeXP.Common.Constants;
using AbeXP.Interfaces;

namespace AbeXP.Services
{
    class SettingsService : ISettingsService
    {
        // Firebase ref
        private const string _fireBaseRef = "FireBaseRef";
        private const string _fireBaseRefDefault = FirebaseConstants.REF;

        public string FireBaseRef
        {
            get => Preferences.Get(_fireBaseRef, _fireBaseRefDefault);
            set => Preferences.Set(_fireBaseRef, value);
        }


        // Culture
        private const string _culture = "culture";
        private const string _cultureDefault = "Es-MX";

        public string Culture
        {
            get => Preferences.Get(_culture, _cultureDefault);
            set => Preferences.Set(_culture, value);
        }


        // Authtoken
        private const string _authAccessToken = "AuthAccessToken";
        private const string _authAccessTokenDefault = default;

        public string AuthAccessToken
        {
            get => Preferences.Get(_authAccessToken, _authAccessTokenDefault);
            set => Preferences.Set(_authAccessToken, value);
        }
    }
}
