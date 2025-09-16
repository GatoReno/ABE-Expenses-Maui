using AbeXP.Common.DTO;
using AbeXP.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AbeXP.Util
{
    //public class HttpKraken
    //{
    //    private readonly HttpClient _client;
    //    private readonly JsonSerializerOptions _jsonOptions;
    //    public HttpKraken(HttpClient client)
    //    {
    //        _client = client;
    //        _jsonOptions = new JsonSerializerOptions
    //        {
    //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    //            PropertyNameCaseInsensitive = true
    //        };
    //    }

    //    private async Task<JsonElement> GetJsonElementAsync(HttpResponseMessage response)
    //    {
    //        var json = await response.Content.ReadAsStringAsync();
    //        return JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
    //    }


    //    private Result<T> MapResponse<T>(JsonElement element)
    //    {
    //        var targetType = typeof(T);

    //        // Check if T is List<X>
    //        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
    //        {
    //            // Get the element type of the list
    //            var itemType = targetType.GenericTypeArguments[0];

    //            // Call FirestoreMapper.MapList<itemType> dynamically
    //            var mapListMethod = typeof(FirestoreMapper)
    //                .GetMethod("MapList")
    //                ?.MakeGenericMethod(itemType);

    //            if (mapListMethod == null)
    //                return new Error("Mapping failed");

    //            var mappedList = mapListMethod.Invoke(null, new object[] { element });

    //            // Return as Result<T> by casting
    //            return (Result<T>)(object)Result<T>.Success(mappedList);
    //        }
    //        else
    //        {
    //            // Single object mapping
    //            var mapMethod = typeof(FirestoreMapper)
    //                .GetMethod("Map")
    //                ?.MakeGenericMethod(targetType);

    //            if (mapMethod == null)
    //                return new Error("Mapping failed");

    //            var mappedObj = mapMethod.Invoke(null, new object[] { element });

    //            if (mappedObj == null)
    //                return new Error { Message = "Mapping failed" };

    //            return (Result<T>)mappedObj; // uses implicit operator
    //        }
    //    }

    //    public async Task<Result<T>> GetAsync<T>(string url) where T : new()
    //    {
    //        try
    //        {
    //            var response = await _client.GetAsync(url);
    //            if (!response.IsSuccessStatusCode)
    //                return new Error ($"HTTP GET failed: {response.StatusCode}", (int)response.StatusCode);

    //            var json = await response.Content.ReadAsStringAsync();
    //            var element = JsonSerializer.Deserialize<JsonElement>(json);

    //            if (typeof(T) == typeof(List<>).MakeGenericType(typeof(T).GenericTypeArguments[0]))
    //            {
    //                var list = FirestoreMapper.MapList<dynamic>(element);
    //                return (Result<T>)(object)list;
    //            }

    //            var mapped = FirestoreMapper.Map<T>(element!);
    //            return mapped;
    //        }
    //        catch (Exception ex)
    //        {
    //            return new Error (ex.Message);
    //        }
    //    }

    //    public async Task<Result<T>> PostAsync<T>(string url, object data) where T : new()
    //    {
    //        try
    //        {
    //            var jsonContent = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
    //            var response = await _client.PostAsync(url, jsonContent);

    //            if (!response.IsSuccessStatusCode)
    //                return new Error ($"HTTP POST failed: {response.StatusCode}");


    //            var element = await GetJsonElementAsync(response);
    //            return MapResponse<T>(element);
    //        }
    //        catch (Exception ex)
    //        {
    //            return new Error (ex.Message);
    //        }
    //    }

    //    public async Task<Result> DeleteAsync(string url)
    //    {
    //        try
    //        {
    //            var response = await _client.DeleteAsync(url);
    //            if (!response.IsSuccessStatusCode)
    //                return new Error($"HTTP DELETE failed: {response.StatusCode}", (int)response.StatusCode);

    //            return Result.Success();
    //        }
    //        catch (Exception ex)
    //        {
    //            return new Error(ex.Message);
    //        }
    //    }
    //}
}