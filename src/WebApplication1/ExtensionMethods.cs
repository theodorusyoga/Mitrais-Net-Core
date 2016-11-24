using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1
{
    public static class ExtensionMethods
    {
        public static string ToUnixTimeString(this DateTime dt)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = Convert.ToInt32((dt.ToUniversalTime() - epoch).TotalSeconds);
            return unixDateTime.ToString();
        }

        public static string Sha256Encrypt(this string phrase)
        {
            using(var alg = SHA256.Create())
            {
                var hash = alg.ComputeHash(Encoding.ASCII.GetBytes(phrase));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
