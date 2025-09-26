using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases
{
    internal class GetExpendituresUseCase : IGetExpendituresUseCase
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IUserSession _userSession;

        public GetExpendituresUseCase(IExpenseRepository expenseRepository, ILoanRepository loanRepository, IUserSession userSession)
        {
            _expenseRepository = expenseRepository;
            _loanRepository = loanRepository;
            _userSession = userSession;
        }


        public async Task<IEnumerable<ExpenditureItem>> ExecuteAsync(ExpendituresRequest request)
        {


            var expenses = await _expenseRepository.GetAllAsync(new IndexItemRequest
            {
                OrderBy = nameof(ExpenseIndexed.UserId_Date),
                StartAt = $"{_userSession.UserId}_{request.StartAt.ToString(DateConstants.IndexDateFormat)}",
                EndAt = $"{_userSession.UserId}_{request.EndAt.ToString(DateConstants.IndexDateFormat)}",
                LimitTo = request.LimitTo
            });


            var loans = await _loanRepository.GetAllAsync(new IndexItemRequest
            {
                OrderBy = nameof(LoanIndexed.UserId_DateGiven),
                StartAt = $"{_userSession.UserId}_{request.StartAt.ToString(DateConstants.IndexDateFormat)}",
                EndAt = $"{_userSession.UserId}_{request.EndAt.ToString(DateConstants.IndexDateFormat)}",
                LimitTo = request.LimitTo
            });


            List<ExpenditureItem> expenditureItems = new List<ExpenditureItem>();
            expenditureItems.AddRange(expenses.Select(ex => new ExpenditureItem
            {
                Amount = ex.Amount,
                Date = ex.Date,
                Description = ex.Description,
                PaymentMethod = ex.PaymentTypeId,
                Type = "Gasto"  //TODO localize
            }));

            expenditureItems.AddRange(loans.Select(ex => new ExpenditureItem
            {
                Amount = ex.Amount,
                Date = ex.DateGiven,
                Description = ex.PersonName,
                Type = "Préstamo"  //TODO localize
            }));

            return expenditureItems.OrderByDescending(exp => exp.Date);
        }
    }
}
