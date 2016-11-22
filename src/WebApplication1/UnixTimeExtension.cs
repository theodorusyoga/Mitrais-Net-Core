using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public static class UnixTimeExtension
    {
        public static string ToUnixTimeString(this DateTime dt)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = Convert.ToInt32((dt.ToUniversalTime() - epoch).TotalSeconds);
            return unixDateTime.ToString();
        }
    }
}
