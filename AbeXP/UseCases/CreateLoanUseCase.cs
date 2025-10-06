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
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IUserSession _userSession;

        public CreateLoanUseCase(ITransactionsRepository transactionsRepository, IUserSession userSession)
        {
            _transactionsRepository = transactionsRepository;
            _userSession = userSession;
        }
        public async Task<Result> ExecuteAsync(Loan request)
        {
            request.UserId = _userSession.User.UserId;
            await _transactionsRepository.AddAsync(request);

            return Result.Ok();
        }
    }
}
