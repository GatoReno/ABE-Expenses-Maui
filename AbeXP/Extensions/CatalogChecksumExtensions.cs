using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AbeXP.Models;

namespace AbeXP.Extensions
{
    internal static class CatalogChecksumExtensions
    {
        private static string ComputeHash(IEnumerable<string> parts)
        {
            using var sha256 = SHA256.Create();
            var payload = string.Join('|', parts);
            var bytes = Encoding.UTF8.GetBytes(payload);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        public static string ComputeChecksum(this IEnumerable<TagModel> tags)
        {
            return ComputeHash(tags
                .OrderBy(tag => tag.Id)
                .Select(tag => $"{tag.Id}|{tag.Name}|{tag.ColorHex}"));
        }

        public static string ComputeChecksum(this IEnumerable<PaymentMethod> paymentMethods)
        {
            return ComputeHash(paymentMethods
                .OrderBy(pm => pm.Id)
                .Select(pm => $"{pm.Id}|{pm.Name}|{pm.Details}"));
        }
    }
}
