using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using AbeXP.Util;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.UseCases
{
    internal class GetTransactionsUseCase : IGetTransactionsUseCase
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IUserSession _userSession;

        public GetTransactionsUseCase(IExpenseRepository expenseRepository, ILoanRepository loanRepository, IUserSession userSession)
        {
            _expenseRepository = expenseRepository;
            _loanRepository = loanRepository;
            _userSession = userSession;
        }


        public async Task<IEnumerable<TransactionItem>> ExecuteAsync(TransactionRequest request)
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


            List<TransactionItem> transactions = new List<TransactionItem>();
            transactions.AddRange(expenses.Select(ex => new TransactionItem
            {
                Amount = ex.Amount,
                Date = ex.Date,
                Description = ex.Description,
                PaymentMethod = ex.PaymentTypeId,
                TypeDescription = AppResources.Expense,
                Type = Common.Enum.TransactionType.Expense,
                Icon = MaterialIconsRegular.Attach_money
            }));

            transactions.AddRange(loans.Select(ex => new TransactionItem
            {
                Amount = ex.Amount,
                Date = ex.DateGiven,
                Description = ex.PersonName,
                IsPaid = ex.IsPaid ? AppResources.Yes : AppResources.No,
                TypeDescription = AppResources.Loan,
                Type = Common.Enum.TransactionType.Loan,
                Icon = MaterialIconsRegular.Person
            }));

            return transactions.OrderByDescending(exp => exp.Date);
        }
    }
}
