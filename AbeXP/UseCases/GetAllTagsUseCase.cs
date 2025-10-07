using FluentResults;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;

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
            var tags = await _tagsRepository.GetAllAsync();

            return tags.ToLocalizeList();
        }
    }
}
