namespace AbeXP.Common.Enum
{
    internal static class CatalogTypeExtensions
    {
        public static string ToStorageKey(this CatalogType catalogType)
            => catalogType.ToString();
    }
}
