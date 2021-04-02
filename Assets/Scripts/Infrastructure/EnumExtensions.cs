using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static string GetLongString<T>(this T en) where T : Enum
        {
            var type = en.GetType();
            var memberInfo = type.GetMember(en.ToString());
            var attribute = memberInfo[0].GetCustomAttribute(typeof(LongNameAttribute));

            return ((LongNameAttribute) attribute).LongName;
        }
    }


    public class LongNameAttribute : Attribute
    {
        public readonly string LongName;

        public LongNameAttribute(string longName)
        {
            this.LongName = longName;
        }
    }
}
