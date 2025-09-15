using AbeXP.Attributes;
using Microsoft.Maui.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AbeXP.Util
{
    public static class FirestoreMapper
    {
        public static T? MapToEntity<T>(JsonElement doc) where T : new()
        {
            if (!doc.TryGetProperty("fields", out var fields))
                return default;

            return MapFields<T>(fields);
        }

        public static IEnumerable<T> MapToEntities<T>(JsonElement root) where T : new()
        {
            if (root.TryGetProperty("documents", out var docs))
            {
                foreach (var d in docs.EnumerateArray())
                {
                    if (d.TryGetProperty("fields", out var fields))
                        yield return MapFields<T>(fields);
                }
            }
        }

        private static T MapFields<T>(JsonElement fieldsElement) where T : new()
        {
            var entity = new T();
            var type = typeof(T);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = prop.GetCustomAttribute<FirestoreFieldAttribute>()?.FieldName ?? prop.Name.ToLower();

                var path = attr.Split('.');
                if (TryGetNestedField(fieldsElement, path, out var valueElement))
                {
                    var value = ConvertFirestoreValue(valueElement, prop.PropertyType);
                    if (value is not null)
                        prop.SetValue(entity, value);
                }
            }

            return entity;
        }

        private static bool TryGetNestedField(JsonElement current, string[] path, out JsonElement value)
        {
            value = current;
            foreach (var part in path)
            {
                if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty(part, out value))
                    return false;
            }
            return true;
        }

        private static object? ConvertFirestoreValue(JsonElement element, Type targetType)
        {
            if (element.ValueKind != JsonValueKind.Object) return null;

            foreach (var prop in element.EnumerateObject())
            {
                switch (prop.Name)
                {
                    case "stringValue": return prop.Value.GetString();
                    case "integerValue": return Convert.ChangeType(prop.Value.GetString(), targetType);
                    case "doubleValue": return prop.Value.GetDouble();
                    case "booleanValue": return prop.Value.GetBoolean();
                    case "timestampValue": return DateTime.TryParse(prop.Value.GetString(), out var dt) ? dt : null;
                    case "referenceValue": return prop.Value.GetString();

                    case "arrayValue" when prop.Value.TryGetProperty("values", out var arr):
                        return ConvertArray(arr, targetType);

                    case "mapValue" when prop.Value.TryGetProperty("fields", out var mapFields):
                        var subObj = Activator.CreateInstance(targetType);
                        return subObj is not null ? MapNestedObject(subObj, mapFields) : null;
                }
            }

            return null;
        }

        private static object? ConvertArray(JsonElement arr, Type targetType)
        {
            var elemType = targetType.IsArray
                ? targetType.GetElementType()!
                : targetType.IsGenericType
                    ? targetType.GetGenericArguments()[0]
                    : typeof(object);

            var listType = typeof(List<>).MakeGenericType(elemType);
            var list = (IList)Activator.CreateInstance(listType)!;

            foreach (var item in arr.EnumerateArray())
            {
                var elem = ConvertFirestoreValue(item, elemType);
                if (elem != null) list.Add(elem);
            }

            if (targetType.IsArray)
            {
                var array = Array.CreateInstance(elemType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }

            return list;
        }

        private static object? MapNestedObject(object target, JsonElement fieldsElement)
        {
            var type = target.GetType();

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = prop.GetCustomAttribute<FirestoreFieldAttribute>();
                if (attr is null) continue;

                var path = attr.FieldName.Split('.');
                if (TryGetNestedField(fieldsElement, path, out var node))
                {
                    var value = ConvertFirestoreValue(node, prop.PropertyType);
                    if (value != null)
                        prop.SetValue(target, value);
                }
            }

            return target;

        }
    }
}
