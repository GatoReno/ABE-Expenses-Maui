using AbeXP.Common.Result;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases
{
    public class CreateLoanUseCase : ICreateLoanUseCase
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserSession _userSession;

        public CreateLoanUseCase(ILoanRepository loanRepository, IUserSession userSession)
        {
            _loanRepository = loanRepository;
            _userSession = userSession;
        }
        public async Task<Result> ExecuteAsync(Loan request)
        {
            request.UserId = _userSession.User.UserId;
            await _loanRepository.AddAsync(new LoanIndexed(request));

            return Result.Ok();
        }
    }
}
