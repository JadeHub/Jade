using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace JadeUtils
{
    public static class ArgChecking
    {
        public static void ThrowIfNull(object obj, string paramName, [CallerMemberName] string memberName = "")
        {
            if (obj == null)
                throw new ArgumentNullException(paramName, "in " + memberName);
        }
    }
}
