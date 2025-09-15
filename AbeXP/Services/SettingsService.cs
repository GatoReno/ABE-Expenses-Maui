using AbeXP.Common.Constants;
using AbeXP.Interfaces;

namespace AbeXP.Services
{
    class SettingsService : ISettingsService
    {
        private const string _fireBaseRef = "FireBaseRef";
        private const string _fireBaseRefDefault = FirebaseConstants.DOMAIN;

        public string FireBaseRef
        {
            get => Preferences.Get(_fireBaseRef, _fireBaseRefDefault);
            set => Preferences.Set(_fireBaseRef, value);
        }
    }
}
