using AbeXP.Common.Result;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;

namespace AbeXP.UseCases
{
    public class CreateIncomeUseCase : ICreateIncomeUseCase
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IUserSession _userSession;

        public CreateIncomeUseCase(IIncomeRepository incomeRepository, IUserSession userSession)
        {
            _incomeRepository = incomeRepository;
            _userSession = userSession;
        }

        public async Task<Result> ExecuteAsync(Income request)
        {
            request.UserId = _userSession.User.UserId;
            await _incomeRepository.AddAsync(new IncomeIndexed(request));
            return Result.Ok();
        }
    }
}
