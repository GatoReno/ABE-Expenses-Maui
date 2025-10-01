using AbeXP.Common.Constants;
using AbeXP.Common.Result;
using AbeXP.Extensions;
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
        private readonly IGetPaymentMethodsUseCase _getPaymentMethodsUseCase;
        private readonly IGetTagsUseCase _getTagsUseCase;
        private readonly IUserSession _userSession;

        public GetTransactionsUseCase(IExpenseRepository expenseRepository, ILoanRepository loanRepository, IGetPaymentMethodsUseCase getPaymentMethodsUseCase, IGetTagsUseCase getTagsUseCase, IUserSession userSession)
        {
            _expenseRepository = expenseRepository;
            _loanRepository = loanRepository;
            _getPaymentMethodsUseCase = getPaymentMethodsUseCase;
            _getTagsUseCase = getTagsUseCase;
            _userSession = userSession;
        }


        public async Task<Result<IEnumerable<TransactionItem>>> ExecuteAsync(TransactionRequest request)
        {
            var paymentMethodsResult = await _getPaymentMethodsUseCase.ExecuteAsync();

            if (paymentMethodsResult.IsFailed)
                return paymentMethodsResult.Errors;

            var paymentMethodsCatalog = paymentMethodsResult.Payload.ToList();

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


            List<TagModel> tagsCatalog = new List<TagModel>();
            if (request.MapTags)
            {
                var tagsResult = await _getTagsUseCase.ExecuteAsync();
                tagsCatalog = tagsResult.Payload.ToList();
            }

            List<TransactionItem> transactions = new List<TransactionItem>();
            transactions.AddRange(expenses.Select(ex => new TransactionItem
            {
                Amount = ex.Amount,
                Date = ex.Date,
                Description = ex.Description,
                PaymentMethod = paymentMethodsCatalog.FirstOrDefault(pm => pm.Id == ex.PaymentTypeId)?.Name ?? ex.PaymentTypeId,
                Tags = request.MapTags ? ex.TagIds?.Select(tagId => tagsCatalog.FirstOrDefault(tagCatalogItem => tagCatalogItem.Id == tagId) ?? new TagModel(tagId)).ToLocalizeStringList() : [],
                TypeDescription = AppResources.Expense,
                Type = Common.Enum.TransactionType.Expense,
                Icon = MaterialIconsRegular.Attach_money
            }));

            transactions.AddRange(loans.Select(ex => new TransactionItem
            {
                Amount = ex.Amount,
                Date = ex.DateGiven,
                DatePayment = ex.SuggestedPaybackDate,
                Description = ex.PersonName,
                IsPaid = ex.IsPaid ? AppResources.Yes : AppResources.No,
                TypeDescription = AppResources.Loan,
                Type = Common.Enum.TransactionType.Loan,
                Icon = MaterialIconsRegular.Person
            }));

            return transactions.OrderByDescending(exp => exp.Date).ToList();
        }
    }
}
