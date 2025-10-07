using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class FirebaseAuthResponse : UserModel
    {
        public FirebaseAuthResponse(FirebaseAuthLink result)
        {
            UserId = result.User?.LocalId;
            FirstName = result.User?.FirstName;
            LastName = result.User?.LastName;
            DisplayName = result.User?.DisplayName;
            Email = result.User?.Email;
            Token = result.FirebaseToken;
            RefreshToken = result.RefreshToken;
            ExpiresIn = DateTime.UtcNow.AddSeconds(result.ExpiresIn);
        }

        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
    }
}
