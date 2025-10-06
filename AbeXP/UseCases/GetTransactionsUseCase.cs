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
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IGetPaymentMethodsUseCase _getPaymentMethodsUseCase;
        private readonly IGetTagsUseCase _getTagsUseCase;
        private readonly IUserSession _userSession;

        public GetTransactionsUseCase(ITransactionsRepository transactionsRepository, IGetPaymentMethodsUseCase getPaymentMethodsUseCase, IGetTagsUseCase getTagsUseCase, IUserSession userSession)
        {
            _transactionsRepository = transactionsRepository;
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

            var transactionsRaw = await _transactionsRepository.GetAllAsync(new IndexItemRequest
            {
                OrderBy = nameof(TransactionModel.UserId_Date),
                StartAt = $"{_userSession.User.UserId}_{request.StartAt.ToString(DateConstants.IndexDateFormat)}",
                EndAt = $"{_userSession.User.UserId}_{request.EndAt.ToString(DateConstants.IndexDateFormat)}",
                LimitTo = request.LimitTo
            });


            List<TagModel> tagsCatalog = new List<TagModel>();
            if (request.MapTags)
            {
                var tagsResult = await _getTagsUseCase.ExecuteAsync();
                tagsCatalog = tagsResult.Payload.ToList();
            }

            List<TransactionItem> transactions = new List<TransactionItem>();

            foreach (var t in transactionsRaw)
            {
                switch (t.Type)
                {
                    case Common.Enum.TransactionType.Expense:
                        transactions.Add(new TransactionItem
                        {
                            Amount = t.Amount,
                            Date = t.Date,
                            Description = t.Description,
                            PaymentMethod = paymentMethodsCatalog.FirstOrDefault(pm => pm.Id == t.PaymentTypeId)?.Name ?? t.PaymentTypeId,
                            Tags = request.MapTags ? t.TagIds?.Select(tagId => tagsCatalog.FirstOrDefault(tagCatalogItem => tagCatalogItem.Id == tagId) ?? new TagModel(tagId)).ToLocalizeStringList() : [],
                            TypeDescription = AppResources.Expense,
                            Type = Common.Enum.TransactionType.Expense,
                            Icon = MaterialIconsRegular.Attach_money
                        });
                        break;
                    case Common.Enum.TransactionType.Income:
                        transactions.Add(new TransactionItem
                        {
                            Amount = t.Amount,
                            Date = t.Date,
                            Description = t.Description,
                            PaymentMethod = paymentMethodsCatalog.FirstOrDefault(pm => pm.Id == t.PaymentTypeId)?.Name ?? t.PaymentTypeId,
                            Tags = request.MapTags ? t.TagIds?.Select(tagId => tagsCatalog.FirstOrDefault(tagCatalogItem => tagCatalogItem.Id == tagId) ?? new TagModel(tagId)).ToLocalizeStringList() : [],
                            TypeDescription = AppResources.Income,
                            Type = Common.Enum.TransactionType.Income,
                            Icon = MaterialIconsRegular.Attach_money
                        });
                        break;
                    case Common.Enum.TransactionType.Loan:
                        transactions.Add(new TransactionItem
                        {
                            Amount = t.Amount,
                            Date = t.DateGiven ?? t.Date,
                            DatePayment = t.SuggestedPaybackDate,
                            Description = t.PersonName ?? t.Description,
                            IsPaid = (t.IsPaid ?? false) ? AppResources.Yes : AppResources.No,
                            TypeDescription = AppResources.Loan,
                            Type = Common.Enum.TransactionType.Loan,
                            Icon = MaterialIconsRegular.Person
                        });
                        break;
                }
            }

            return transactions.OrderByDescending(exp => exp.Date).ToList();
        }
    }
}
