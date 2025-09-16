using AbeXP.Common.Constants;
using AbeXP.Common.DTO;
using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AbeXP.Util
{
    public class FirestoreKraken
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _collection;

        public FirestoreKraken(string collection)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(FirebaseConstants.FIREBASE_API_URL)
            };
            _collection = collection;
        }

        // -----------------------------
        // Create a document in a collection
        // -----------------------------
        public async Task<T> CreateAsync<T>(string collection, object data) where T : new()
        {
            var url = $"{_baseUrl}/{collection}";
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            // Deserialize FirestoreDocument
            var doc = JsonSerializer.Deserialize<FirestoreDocument<Dictionary<string, FirestoreValue>>>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return FirestoreMapper.Map<T>(doc);
        }

        // -----------------------------
        // RunQuery
        // -----------------------------
        public async Task<Result<List<T>>> RunQueryAsync<T>(object query) where T : new()
        {
            var url = FirebaseConstants.RunQueryApi();
            var json = JsonSerializer.Serialize(query, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            // Firestore RunQuery returns an array of { document: {...} }
            var queryResults = JsonSerializer.Deserialize<List<FirestoreQueryResponse<T>>>(
                responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Map each document to T
            var list = new List<T>();
            if (queryResults != null)
            {
                foreach (var result in queryResults)
                {
                    if (result.Document != null)
                    {
                        list.Add(FirestoreMapper.Map<T>(result.Document));
                    }
                }
            }

            return list;
        }

        // -----------------------------
        // Get a single document by path
        // -----------------------------
        public async Task<T> GetAsync<T>(string documentPath) where T : new()
        {
            var url = $"{_baseUrl}/{documentPath}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            var doc = JsonSerializer.Deserialize<FirestoreDocument<Dictionary<string, FirestoreValue>>>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return FirestoreMapper.Map<T>(doc);
        }
    }
}
