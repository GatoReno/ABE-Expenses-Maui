using AbeXP.Attributes;
using AbeXP.Common.DTO;
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
        public static T Map<T>(object source) where T : new()
        {
            if (source == null) return default!;

            if (typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = typeof(T).GetGenericArguments()[0];

                if (source is IEnumerable<FirestoreDocument<object>> listDocs)
                {
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
                    foreach (var doc in listDocs)
                    {
                        var entity = MapToEntity(itemType, doc.Fields);
                        list.Add(entity);
                    }
                    return (T)list;
                }

                if (source is IEnumerable<Dictionary<string, FirestoreValue>> listFields)
                {
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
                    foreach (var fields in listFields)
                    {
                        var entity = MapToEntity(itemType, fields);
                        list.Add(entity);
                    }
                    return (T)list;
                }
            }
            else
            {
                // single entity
                if (source is FirestoreDocument<T> doc)
                {
                    return (T)MapToEntity(typeof(T), doc.Fields);
                }
                if (source is Dictionary<string, FirestoreValue> fields)
                {
                    return (T)MapToEntity(typeof(T), fields);
                }
            }

            throw new InvalidOperationException("Unsupported source type for mapping.");
        }

        private static object MapToEntity(Type type, Dictionary<string, FirestoreValue> fields)
        {
            var entity = Activator.CreateInstance(type)!;

            foreach (var prop in type.GetProperties())
            {
                var attr = prop.GetCustomAttribute<FirestoreFieldAttribute>();
                string path = attr?.Path ?? prop.Name.ToLower();

                var value = GetNestedValue(fields, path);
                if (value != null)
                {
                    prop.SetValue(entity, ConvertValue(value, prop.PropertyType));
                }
            }

            return entity;
        }

        private static FirestoreValue GetNestedValue(Dictionary<string, FirestoreValue> fields, string path)
        {
            var parts = path.Split('.');
            FirestoreValue? current = null;
            Dictionary<string, FirestoreValue>? currentDict = fields;

            foreach (var part in parts)
            {
                if (currentDict != null && currentDict.TryGetValue(part, out current))
                {
                    currentDict = current.MapValue;
                }
                else
                {
                    return null;
                }
            }

            return current;
        }

        private static object ConvertValue(FirestoreValue value, Type targetType)
        {
            if (targetType == typeof(string)) return value.StringValue ?? "";
            if (targetType == typeof(int)) return Convert.ToInt32(value.IntegerValue);
            if (targetType == typeof(long)) return Convert.ToInt64(value.IntegerValue);
            if (targetType == typeof(decimal)) return Convert.ToDecimal(value.IntegerValue);
            if (targetType == typeof(double)) return Convert.ToDouble(value.DoubleValue);
            if (targetType == typeof(bool)) return Convert.ToBoolean(value.BooleanValue);
            if (targetType == typeof(DateTime) && value.TimestampValue != null)
                return DateTime.Parse(value.TimestampValue);

            // Handle arrays
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>)
                && value.ArrayValue != null)
            {
                var itemType = targetType.GetGenericArguments()[0];
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
                foreach (var item in value.ArrayValue.Values)
                {
                    list.Add(ConvertValue(item, itemType));
                }
                return list;
            }

            // Nested object mapping
            if (value.MapValue != null)
            {
                var method = typeof(FirestoreMapper).GetMethod(nameof(MapToEntity), BindingFlags.NonPublic | BindingFlags.Static)!;
                return method.Invoke(null, new object[] { targetType, value.MapValue })!;
            }

            return null!;
        }
    }
}
