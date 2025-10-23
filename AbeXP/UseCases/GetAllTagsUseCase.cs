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

        public GetAllTagsUseCase(ITagsRepository tagsRepository)
        {
            _tagsRepository = tagsRepository;
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
                return Result.Fail<IEnumerable<TagModel>>(new ExceptionalError(ex));
            }
        }
    }
}
