using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Infrastructure
{
    public static class EnumExtensions
    {
        public static string GetString<T>(this T type) where T : Enum
        {
            return Enum.GetName(type.GetType(), type);
        }
    }
}
