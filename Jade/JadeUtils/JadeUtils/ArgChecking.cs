using System;
using System.Runtime.CompilerServices;

namespace JadeUtils
{
    public static class ArgChecking
    {
        public static void ThrowIfTrue(bool value, string paramName, [CallerMemberName] string memberName = "")
        {
            if (value == true)
                throw new ArgumentException(paramName + " is true in " + memberName);
        }

        public static void ThrowIfFalse(bool value, string paramName, [CallerMemberName] string memberName = "")
        {
            if(value == false)
                throw new ArgumentException(paramName + " is false in " + memberName);
        }

        public static void ThrowIfNull(object obj, string paramName, [CallerMemberName] string memberName = "")
        {
            if (obj == null)
                throw new ArgumentNullException(paramName, "in " + memberName);
        }
    }
}
