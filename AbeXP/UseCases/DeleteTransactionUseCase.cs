using AbeXP.Common.Result;
using AbeXP.Interfaces;
using AbeXP.UseCases.Interfaces;

namespace AbeXP.UseCases
{
    public class DeleteTransactionUseCase : IDeleteTransactionUseCase
    {
        private readonly ITransactionsRepository _transactionsRepository;

        public DeleteTransactionUseCase(ITransactionsRepository transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }

        public async Task<Result> ExecuteAsync(string id)
        {
            await _transactionsRepository.DeleteAsync(id);
            return Result.Ok();
        }
    }
}
