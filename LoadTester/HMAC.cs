using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NBomber.Http.CSharp;

namespace LoadTester
{
    public static class HMAC
    {
        public static HttpRequestMessage WithHMACSignature(this HttpRequestMessage request, string keyId, string secret)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            
            var signature = CalculateHash("SECRET", $"GET{request.RequestUri}{timestamp}");

            return request
                .WithHeader("X-Request-Timestamp", timestamp)
                .WithHeader("Authorization", "HMAC-SHA256 id=Test;signature=" + signature);
        }

        private static string CalculateHash(string key, string input)
        {
            var textBytes = Encoding.UTF8.GetBytes(input);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            using var hash = new HMACSHA256(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
