using AbeXP.Abstractions.Interfaces;
using AbeXP.Common.Constants;
using AbeXP.Models;
using AbeXP.UseCases.Plugins;
using Firebase.Auth;

namespace AbeXP.Abstractions.Services
{
    public class FirebaseAuthService : IFibAuthLog
    {
        private readonly FirebaseAuthProvider _authProvider;
        private readonly string ApiKey = FirebaseConstants.KEY;
        private readonly IUserSession _userSession;

        public FirebaseAuthService(IUserSession userSession)
        {
            _authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            _userSession = userSession;
        }

        public async Task<UserModel> SignInWithEmailAndPass(string email, string pass)
        {
            var result = await _authProvider.SignInWithEmailAndPasswordAsync(email, pass);

            var userModel = new FirebaseAuthResponse(result);
            await _userSession.NewSession(userModel);

            return userModel;
        }

        public async Task<UserModel> CreateUserWithEmailAndPass(string email, string pass)
        {
            try
            {
                var auth = await _authProvider.CreateUserWithEmailAndPasswordAsync(email, pass, null, true);

                var userModel = new FirebaseAuthResponse(auth);
                await _userSession.NewSession(userModel);

                return userModel;
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Error al crear usuario: {ex.Reason}");
            }
        }


        public async Task<string?> GetValidTokenAsync()
        {
            var savedToken = await _userSession.GetTokenAsync();
            var tokenExpiration = await _userSession.GetTokenExpirationAsync();

            // if token still valid
            if (!string.IsNullOrEmpty(savedToken) && DateTime.TryParse(tokenExpiration, out var expiryTime) && DateTime.UtcNow < expiryTime)
            {
                return savedToken;
            }

            var refreshToken = await _userSession.GetRefreshTokenAsync();
          
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            // Refresh if expired
            var refreshedAuth = await _authProvider.RefreshAuthAsync(new FirebaseAuth { RefreshToken = refreshToken });

            var userModel = new FirebaseAuthResponse(refreshedAuth);
            await _userSession.NewSession(userModel);
           
            return userModel.Token;
        }

        public async Task Logout()
        {
            try
            {
                _userSession.SignOut();

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cerrar sesión: " + ex.Message);
            }
        }
    }
}

