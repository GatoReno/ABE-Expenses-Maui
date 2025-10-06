using AbeXP.Common.Result;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;

namespace AbeXP.UseCases
{
    public class CreateIncomeUseCase : ICreateIncomeUseCase
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IUserSession _userSession;

        public CreateIncomeUseCase(ITransactionsRepository transactionsRepository, IUserSession userSession)
        {
            _transactionsRepository = transactionsRepository;
            _userSession = userSession;
        }

        public async Task<Result> ExecuteAsync(Income request)
        {
            request.UserId = _userSession.User.UserId;
            await _transactionsRepository.AddAsync(request);
            return Result.Ok();
        }
    }
}
