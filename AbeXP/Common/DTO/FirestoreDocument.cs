using AbeXP.Attributes;
using AbeXP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AbeXP.Common.DTO
{
    public class FirestoreValue
    {
        public string? StringValue { get; set; }
        public string? IntegerValue { get; set; }
        public string? DoubleValue { get; set; }
        public string? BooleanValue { get; set; }
        public Dictionary<string, FirestoreValue>? MapValue { get; set; }
        public ArrayValue ArrayValue { get; set; }
        public string? TimestampValue { get; set; }
    }

    public class ArrayValue
    {
        public List<FirestoreValue> Values { get; set; }
    }

    public class FirestoreDocument<T> where T : new()
    {
        public string Name { get; set; }
        public Dictionary<string, FirestoreValue> Fields { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }

        // Convenience: map fields to T
        public T ToEntity() => FirestoreMapper.Map<T>(Fields);
    }

    public class FirestoreListResponse<T> where T : new()
    {
        public List<FirestoreDocument<T>> Documents { get; set; } = new();
        public string NextPageToken { get; set; }
    }

    public class FirestoreQueryResponse<T> where T : new()
    {
        public string Transaction { get; set; }
        public FirestoreDocument<T> Document { get; set; }
        public string ReadTime { get; set; }
        public int SkippedResults { get; set; }
        public object ExplainMetrics { get; set; }
        public bool? Done { get; set; }
    }


}
