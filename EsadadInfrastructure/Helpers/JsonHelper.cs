using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Esadad.Infrastructure.Helpers
{
    public static class JsonHelper
    {
        public static string ToJsonString(object dto, bool IsCamelCasePropNaming= false )
        {
            if (IsCamelCasePropNaming)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false, // or true if you want pretty-printing
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // optional: matches typical JSON naming
                };

                return JsonSerializer.Serialize(dto, options);

            }

            return JsonSerializer.Serialize(dto);
        }

    }
}
