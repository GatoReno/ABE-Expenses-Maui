using AbeXP.UseCases.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Services
{
    internal class UserSession : IUserSession
    {
        private string userId = "alex";

        public string UserId
        {
            get
            {
                return userId;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return Preferences.Get("IsLogged", true);
            }
        }

    }
}
