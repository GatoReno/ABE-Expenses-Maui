using AbeXP.Abstractions.Interfaces;
using AbeXP.Common.Constants;
using AbeXP.Models;
using AbeXP.UseCases.Plugins;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Services
{
    internal class UserSession : IUserSession
    {
        public UserModel User { get; set; }

        public UserSession()
        {
            CreateUser();
        }


        private void CreateUser()
        {
            User = new UserModel
            {
                UserId = Preferences.Get(PreferencesConstants.UserId, string.Empty),
                DisplayName = Preferences.Get(PreferencesConstants.DisplayName, string.Empty),
                FirstName = Preferences.Get(PreferencesConstants.FirstName, string.Empty),
                LastName = Preferences.Get(PreferencesConstants.LastName, string.Empty),
                Email = Preferences.Get(PreferencesConstants.Email, string.Empty)
            };
        }

       
        public async Task<bool> IsLoggedInAsync()
        {
            // Do we have a refresh token saved?
            var refreshToken = await GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            //// Try to get a valid ID token
            //var token = await fibAuthLog.GetValidTokenAsync();
            //return !string.IsNullOrEmpty(token);

            return true;
        }


        /// <summary>
        /// Check if session is valid. After a X days from loggedin
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsSessionValid()
        {
            var loginDateString = await SecureStorage.GetAsync(PreferencesConstants.LogingDate);
            if (DateTime.TryParse(loginDateString, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var loginDate))
            {
                return DateTime.UtcNow - loginDate < TimeSpan.FromDays(15);
            }

            return true;
        }


        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(PreferencesConstants.Token);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            return await SecureStorage.GetAsync(PreferencesConstants.RefreshToken);
        }

        public async Task<string?> GetTokenExpirationAsync()
        {
            return await SecureStorage.GetAsync(PreferencesConstants.TokenExpirationDate);
        }

        public void SignOut()
        {
            SecureStorage.Remove(PreferencesConstants.Token);
            SecureStorage.Remove(PreferencesConstants.RefreshToken);
            SecureStorage.Remove(PreferencesConstants.TokenExpirationDate);
            Preferences.Remove(PreferencesConstants.UserId);
            Preferences.Remove(PreferencesConstants.DisplayName);
            Preferences.Remove(PreferencesConstants.FirstName);
            Preferences.Remove(PreferencesConstants.LastName);
            Preferences.Remove(PreferencesConstants.Email);
        }

        public async Task NewSession(FirebaseAuthResponse authResponse)
        {
            await NewTokens(authResponse);
            await SecureStorage.SetAsync(PreferencesConstants.LogingDate, DateTime.UtcNow.ToString("o"));

            Preferences.Set(PreferencesConstants.UserId, authResponse.UserId);
            Preferences.Set(PreferencesConstants.DisplayName, authResponse.DisplayName);
            Preferences.Set(PreferencesConstants.FirstName, authResponse.FirstName);
            Preferences.Set(PreferencesConstants.LastName, authResponse.LastName);
            Preferences.Set(PreferencesConstants.Email, authResponse.Email);

            CreateUser();
        }

        public async Task NewTokens(FirebaseAuthResponse authResponse)
        {
            await SecureStorage.SetAsync(PreferencesConstants.Token, authResponse.Token);
            await SecureStorage.SetAsync(PreferencesConstants.RefreshToken, authResponse.RefreshToken);
            await SecureStorage.SetAsync(PreferencesConstants.TokenExpirationDate, authResponse.ExpiresIn.ToString("O"));

        }
    }
}
