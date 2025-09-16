using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AbeXP.Common.DTO
{
    //public class FirestoreValue
    //{
    //    public static object? GetValue(JsonElement element)
    //    {
    //        if (element.ValueKind != JsonValueKind.Object) return null;

    //        if (element.TryGetProperty("stringValue", out var stringValue))
    //            return stringValue.GetString();

    //        if (element.TryGetProperty("integerValue", out var intValue) && long.TryParse(intValue.GetString(), out var i))
    //            return i;

    //        if (element.TryGetProperty("doubleValue", out var doubleValue))
    //            return doubleValue.GetDouble();

    //        if (element.TryGetProperty("booleanValue", out var boolValue))
    //            return boolValue.GetBoolean();

    //        if (element.TryGetProperty("timestampValue", out var timestampValue) && DateTime.TryParse(timestampValue.GetString(), out var dt))
    //            return dt;

    //        if (element.TryGetProperty("mapValue", out var mapValue) && mapValue.TryGetProperty("fields", out var fields))
    //            return fields;

    //        if (element.TryGetProperty("arrayValue", out var arrayValue) && arrayValue.TryGetProperty("values", out var values))
    //            return values.EnumerateArray();

    //        return null;
    //    }
    //}
}
