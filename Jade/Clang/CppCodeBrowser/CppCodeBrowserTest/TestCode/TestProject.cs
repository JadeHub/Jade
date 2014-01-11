using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CppCodeBrowser;
using LibClang;

namespace CppCodeBrowserTest
{
    static public class TestProject
    {        
        public static Project MakeProject()
        {
            IIndexBuilder ib = new IndexBuilder();
            Project proj = new Project("TestProject", ib);

            proj.AddSourceFile("TestCode\\main.cpp", null);
            proj.AddSourceFile("TestCode\\class_a.cpp", null);

            return proj;
        }
    }
}
