using Esadad.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esadad.Infrastructure.Helpers
{
    public static class CurrencyHelper
    {
        public static decimal AdjustDecimal(decimal value, int decimalPlaces, DecimalAdjustment adjustment)
        {
            return adjustment switch
            {
                DecimalAdjustment.Truncate => TruncateDecimal(value, decimalPlaces),
                DecimalAdjustment.Round => Math.Round(value, decimalPlaces),
                _ => throw new ArgumentOutOfRangeException(nameof(adjustment), adjustment, null)
            };
        }

        private static decimal TruncateDecimal(decimal value, int decimalPlaces)
        {
            // Calculate the factor to scale the value
            decimal scale = (decimal)Math.Pow(10, decimalPlaces);
            // Truncate the value by converting it to an integer
            return Math.Truncate(value * scale) / scale;
        }

    }
}
