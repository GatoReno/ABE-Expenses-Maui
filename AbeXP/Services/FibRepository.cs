using AbeXP.Interfaces;
using AbeXP.Models;
using Firebase.Database;
using Firebase.Database.Query;
using AbeXP.Extensions;

namespace AbeXP.Services
{
    public class FibRepository<T> where T : BaseEntity, new()
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly string _collection;

        public FibRepository(IFibInstance fibInstance, string collection)
        {
            _firebaseClient = fibInstance.GetInstance();
            _collection = collection;
        }

        public async Task AddAsync(T entity)
        {
            await _firebaseClient
                .Child(_collection)
                .Child(entity.Id)
                .PutAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await _firebaseClient
                .Child(_collection)
                .Child(entity.Id)
                .PutAsync(entity);
        }

        public async Task DeleteAsync(string id)
        {
            await _firebaseClient
                .Child(_collection)
                .Child(id)
                .DeleteAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _firebaseClient
                .Child(_collection)
                .Child(id)
                .OnceSingleAsync<T>();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            var items = await _firebaseClient
                .Child(_collection)
                .OnceAsync<T>()
                .UnwrapItems<T>();

            return items;
        }

    }


}

