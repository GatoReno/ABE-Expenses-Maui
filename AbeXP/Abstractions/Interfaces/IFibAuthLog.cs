using AbeXP.Models;
using System;
namespace AbeXP.Abstractions.Interfaces
{
	public interface IFibAuthLog
	{
		Task<UserModel> SignInWithEmailAndPass(string email, string pass);
		Task<UserModel> CreateUserWithEmailAndPass(string email, string pass);

		Task<string?> GetValidTokenAsync();
        Task Logout();

    }
}

