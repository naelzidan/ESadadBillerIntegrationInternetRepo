using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esadad.Infrastructure.Helpers
{
    public static class ObjectCasterHelper
    {
        public static T? CastTo<T>(object obj) where T : class
        {
            return obj as T;
        }
    }
}
