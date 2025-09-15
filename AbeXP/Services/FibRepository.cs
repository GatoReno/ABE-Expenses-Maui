using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Util;
using CommunityToolkit.Mvvm.DependencyInjection;
using Firebase.Database;
using Firebase.Database.Query;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AbeXP.Services
{
    public class FibRepository<T> where T : new()
    {
        private readonly HttpClient _client;
        private readonly string _collection;
        private readonly string _apiKey;
        private readonly string _collectionApi;

        public FibRepository(string collection)
        {
            _client = new HttpClient() 
            {
                BaseAddress = new Uri(FirebaseConstants.FIREBASE_API_URL)
            };
            _collection = collection;
            _apiKey = FirebaseConstants.KEY;
            _collectionApi = FirebaseConstants.GetCollectionStoreURL(collection);
        }


        public async Task<string> AddAsync(T entity)
        {
            var firestoreDoc = ToFirestoreDocument(entity);

            var response = await _client.PostAsJsonAsync($"{_collectionApi}/?key={_apiKey}", firestoreDoc);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("name").GetString()?.Split('/').Last();
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync($"{_collectionApi}/{id}?key={_apiKey}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var response = await _client.GetAsync($"{_collectionApi}/?key={_apiKey}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return FirestoreMapper.MapToEntities<T>(json);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync($"{_collectionApi}/{id}?key={_apiKey}");
            if (!response.IsSuccessStatusCode) return default;

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return FirestoreMapper.MapToEntity<T>(json);
        }

        public async Task UpdateAsync(string id, T entity)
        {
            var firestoreDoc = ToFirestoreDocument(entity);

            var jsonStr = JsonSerializer.Serialize(firestoreDoc);
            var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync($"{_collectionApi}/{id}?key={_apiKey}", content);
            response.EnsureSuccessStatusCode();
        }

        // Helper: convert T to Firestore JSON format
        private object ToFirestoreDocument(T entity)
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in typeof(T).GetProperties())
            {
                var value = prop.GetValue(entity);
                if (value == null) continue;

                dict[prop.Name] = new Dictionary<string, object>
            {
                { GetFirestoreType(value), value }
            };
            }
            return new { fields = dict };
        }

        private string GetFirestoreType(object value)
        {
            return value switch
            {
                string => "stringValue",
                int => "integerValue",
                long => "integerValue",
                decimal => "doubleValue",
                double => "doubleValue",
                float => "doubleValue",
                DateTime => "timestampValue",
                bool => "booleanValue",
                _ => "stringValue"
            };
        }

    }
}
