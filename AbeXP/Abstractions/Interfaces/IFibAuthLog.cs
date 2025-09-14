using System;
namespace AbeXP.Abstractions.Interfaces
{
	public interface IFibAuthLog
	{
		Task<string> SignInWithEmailAndPass(string email, string pass);
		Task<string> CreateUserWithEmailAndPass(string email, string pass);
        Task Logout();

    }
}

