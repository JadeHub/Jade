using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeUtils.IO
{
    public static class PathUtils
    {
        public static string CombinePaths(string path1, string path2)
        {
            
            return System.IO.Path.Combine(path1, path2);
        }

        public static bool AreSamePath(string path1, string path2)
        {
            try
            {
                return System.IO.Path.GetFullPath(path1) == System.IO.Path.GetFullPath(path2);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool IsPathDirectory(string path)
        {
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(path);
            return ((attr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);
        }

        public static string CalculateRelativePath(string path1, string path2)
        {
            int c = 0;  //index up to which the paths are the same
            int d = -1; //index of trailing slash for the portion where the paths are the same

            while (c < path1.Length && c < path2.Length)
            {
                if (char.ToLowerInvariant(path1[c]) != char.ToLowerInvariant(path2[c]))
                {
                    break;
                }

                if (path1[c] == '\\')
                {
                    d = c;
                }

                c++;
            }

            if (c == 0)
            {
                return path2;
            }

            if (c == path1.Length && c == path2.Length)
            {
                return string.Empty;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            while (c < path1.Length)
            {
                if (path1[c] == '\\')
                {
                    builder.Append(@"..\");
                }
                c++;
            }

            if (builder.Length == 0 && path2.Length - 1 == d)
            {
                return @".\";
            }

            return builder.ToString() + path2.Substring(d + 1);                        
        }

        public static void Test()
        {
            string r = CalculateRelativePath(@"C:\Code\GitHub\Jade\TestData\JadeData\JadeData.jpj", @"c:\Code\GitHub\Jade\TestData");
            string r2 = CalculateRelativePath(@"C:\Code\GitHub\Jade\TestData", @"C:\Code\GitHub\Jade\TestData\Proj3.jpj");
            string r3 = System.IO.Path.Combine(@"c:\Code\GitHub\Jade\TestData", r2);
        }
    }
}
