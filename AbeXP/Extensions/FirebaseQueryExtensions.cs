using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Extensions
{
    public static class FirebaseQueryExtensions
    {
        /// <summary>
        /// Executes a Firebase query and maps the results to a list of type T.
        /// </summary>
        public static async Task<IReadOnlyCollection<T>> UnwrapItems<T>(this Task<IReadOnlyCollection<FirebaseObject<T>>> task)
        {
            var itemsResult = await task;

            return await Task.FromResult(itemsResult.Select(d => d.Object).ToList());
        }
    }
}
