using AbeXP.Common.Constants;
using AbeXP.Common.DTO;
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
        private readonly FirestoreKraken _client;
        private readonly string _collection;
        private readonly string _apiKey;
        private readonly string _collectionApi;

        public FibRepository(string collection)
        {
            _client = new FirestoreKraken(collection);
            _collection = collection;
            _apiKey = FirebaseConstants.KEY;
            _collectionApi = FirebaseConstants.CollectionStoreApi(collection);
        }


        public async Task<string> AddAsync(T entity)
        {
            //var firestoreDoc = ToFirestoreDocument(entity);

            //var response = await _client.PostAsync($"{_collectionApi}/?key={_apiKey}", firestoreDoc);
            //response.EnsureSuccessStatusCode();

            //var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            //return json.GetProperty("name").GetString()?.Split('/').Last();
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string id)
        {
            //var response = await _client.DeleteAsync($"{_collectionApi}/{id}?key={_apiKey}");
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync(SearchParameters request)
        {
            var query = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = _collection } },
                    where = (request.StartDate != null && request.EndDate != null) ? new
                    {
                        compositeFilter = new
                        {
                            op = "AND",
                            filters = new object[]
                            {
                            new {
                                fieldFilter = new {
                                    field = new { fieldPath = "date" },
                                    op = "GREATER_THAN_OR_EQUAL",
                                    value = new { timestampValue = request.StartDate.Value.ToUniversalTime().ToString("O") }
                                }
                            },
                            new {
                                fieldFilter = new {
                                    field = new { fieldPath = "date" },
                                    op = "LESS_THAN_OR_EQUAL",
                                    value = new { timestampValue = request.EndDate.Value.ToUniversalTime().ToString("O") }
                                }
                            }
                            }
                        }
                    } : null,
                    limit = request.Pagination?.PageSize,
                    offset = request.Pagination?.Skip
                }
            };

            var response = await _client.RunQueryAsync<T>(query);

            return response.Value;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            //var response = await _client.GetAsync<JsonElement>($"{_collectionApi}/{id}?key={_apiKey}");

            //return FirestoreMapper.MapToEntity<T>(response.Value);

            throw new NotImplementedException();
        }

        public async Task UpdateAsync(string id, T entity)
        {
            //var firestoreDoc = ToFirestoreDocument(entity);

            //var jsonStr = JsonSerializer.Serialize(firestoreDoc);
            //var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

            //var response = await _client.PatchAsync<JsonElement>($"{_collectionApi}/{id}?key={_apiKey}", content);
            throw new NotImplementedException();
        }



    }
}
