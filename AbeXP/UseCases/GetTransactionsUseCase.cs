using AbeXP.Common.Constants;
using FluentResults;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Localizers;
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
            // catalogs
            // TODO: better for performance to have one centralized storage and load once instead of each request (singleton for catalogs)
            var paymentMethodsResult = await _getPaymentMethodsUseCase.ExecuteAsync();

            if (paymentMethodsResult.IsFailed)
                return Result.Fail<IEnumerable<TransactionItem>>(paymentMethodsResult.Errors);

            var paymentMethodsCatalog = paymentMethodsResult.Value.ToList();

            List<TagModel> tagsCatalog = new List<TagModel>();
            if (request.MapTags)
            {
                var tagsResult = await _getTagsUseCase.ExecuteAsync();
                if (tagsResult.IsFailed)
                    return Result.Fail<IEnumerable<TransactionItem>>(tagsResult.Errors);
                tagsCatalog = tagsResult.Value.ToList();
            }

            // transactions
            var transactionsRaw = await _transactionsRepository.GetAllAsync(new IndexItemRequest
            {
                OrderBy = nameof(TransactionModel.UserId_Date),
                StartAt = $"{_userSession.User.UserId}_{request.StartAt.ToString(DateConstants.IndexDateFormat)}",
                EndAt = $"{_userSession.User.UserId}_{request.EndAt.ToString(DateConstants.IndexDateFormat)}",
                LimitTo = request.LimitTo
            });

            List<TransactionItem> transactions = transactionsRaw.Select(t => new TransactionItem
            {
                Id = t.Id,
                Amount = t.Amount,
                Date = t.Date,
                Description = t.Description,
                PaymentMethod = paymentMethodsCatalog.FirstOrDefault(pm => pm.Id == t.PaymentTypeId)?.Name ?? t.PaymentTypeId,
                Tags = request.MapTags ? t.TagIds?.Select(tagId => tagsCatalog.FirstOrDefault(tagCatalogItem => tagCatalogItem.Id == tagId) ?? new TagModel(tagId)).ToLocalizeStringList() : [],
                TypeDescription = TransactionModelLocalizer.GetTypeName(t.Type.ToString()),
                Type = t.Type,
                Icon = MaterialIconsRegular.Attach_money
            }).ToList();

            return transactions.OrderByDescending(exp => exp.Date).ToList();
        }
    }
}
