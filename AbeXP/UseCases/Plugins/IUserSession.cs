using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases.Plugins
{
    public interface IUserSession
    {
        UserModel User {  get; }
        Task<string?> GetTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task<string?> GetTokenExpirationAsync();
        Task<bool> IsLoggedInAsync();
        Task NewSession(FirebaseAuthResponse authResponse);
        void SignOut();
    }
}
