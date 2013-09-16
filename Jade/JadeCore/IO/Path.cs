using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.IO
{
    public static class Path
    {
        public static string NormalisePath(string path)
        {
            return System.IO.Path.GetFullPath(path).TrimEnd(new char[] { '\\' });
        }

        public static string CombinePaths(string path1, string path2)
        {
            return NormalisePath(System.IO.Path.Combine(path1, path2));
        }

        public static bool IsPathDirectory(string path)
        {
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(path);
            return ((attr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);
        }

        public static string CalculateRelativePath(string from, string to)
        {
            from = NormalisePath(from);
            to = NormalisePath(to);
            
            if(IsPathDirectory(from))
                from += '\\';
            if (IsPathDirectory(to))
                to += '\\';

            var uri = new Uri(from);
            string result = uri.MakeRelativeUri(new Uri(to)).ToString();
            return result.Replace('/', '\\');            
        }

        public static void Test()
        {
            string r = CalculateRelativePath(@"C:\Code\GitHub\Jade\TestData\JadeData\JadeData.jpj", @"c:\Code\GitHub\Jade\TestData");
            string r2 = CalculateRelativePath(@"C:\Code\GitHub\Jade\TestData", @"C:\Code\GitHub\Jade\TestData\Proj3.jpj");
            string r3 = System.IO.Path.Combine(@"c:\Code\GitHub\Jade\TestData", r2);
        }
    }
}
