using System;
using System.Collections.Generic;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using FluentResults;

namespace AbeXP.UseCases
{
    public class GetAllTagsUseCase : IGetTagsUseCase
    {
        private readonly ITagsRepository _tagsRepository;
        private readonly IAnalyticsService _analyticsService;

        public GetAllTagsUseCase(ITagsRepository tagsRepository, IAnalyticsService analyticsService)
        {
            _tagsRepository = tagsRepository;
            _analyticsService = analyticsService;
        }

        public async Task<Result<IEnumerable<TagModel>>> ExecuteAsync()
        {
            try
            {
                var tags = await _tagsRepository.GetAllAsync();
                return Result.Ok<IEnumerable<TagModel>>(tags.ToLocalizeList());
            }
            catch (Exception ex)
            {
                await _analyticsService.LogEventAsync("get_tags_failed", new Dictionary<string, string>
                {
                    ["exception"] = ex.Message
                });

                return Result.Fail<IEnumerable<TagModel>>(new ExceptionalError(ex));
            }
        }
    }
}
