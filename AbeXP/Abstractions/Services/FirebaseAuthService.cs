using AbeXP.Abstractions.Interfaces;
using AbeXP.Common.Constants;
using Firebase.Auth;

namespace AbeXP.Abstractions.Services
{
    public class FirebaseAuthService : IFibAuthLog
    {
        private readonly FirebaseAuthProvider _authProvider;
        private readonly string ApiKey = FirebaseConstants.KEY;

        public FirebaseAuthService()
        {
            // Inicializa con tu API Key de Firebase
            _authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
        }

        public async Task<string> SignInWithEmailAndPass(string email, string pass)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var result = await auth.SignInWithEmailAndPasswordAsync(email, pass);
            return result.FirebaseToken;
        }

        public async Task<string> CreateUserWithEmailAndPass(string email, string pass)
        {
            try
            {
                var auth = await _authProvider.CreateUserWithEmailAndPasswordAsync(email, pass, null, true);
                var token = auth.FirebaseToken;
                return token;
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Error al crear usuario: {ex.Reason}");
            }
        }

        public async Task Logout()
        {
            try
            {
                SecureStorage.Remove("firebase_token");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cerrar sesión: " + ex.Message);
            }
        }
    }
}

