using AbeXP.Interfaces;
using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Services
{
    public class TagsRepository : FibRepository<TagModel>, ITagsRepository
    {
        public IFibInstance _db { get; set; }
        public TagsRepository(IFibInstance fibInstance, string collection)
            : base(fibInstance, collection)
        {
            _db = fibInstance;
        }
    }
}
