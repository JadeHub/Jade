
namespace LibClangTest.TestCode
{
    using LibClang;

    public static class TranslationUnits
    {
        public static Index Index;

        public static TranslationUnit SimpleClassCpp
        {
            get
            {
                return CreateTranslationUnit(SimpleClassCppFile.Path);
            }
        }

        public static TranslationUnit ErrorWarningClassCpp
        {
            get
            {
                return CreateTranslationUnit(ErrorWarningCppFile.Path);
            }
        }

        public static TranslationUnit HeaderTestsCpp
        {
            get
            {
                return CreateTranslationUnit(HeaderTestsCppFile.Path);
            }
        }

        public static TranslationUnit StringIncludeCpp
        {
            get
            {
                return CreateTranslationUnit(StringIncludeCppFile.Path);
            }
        }

        static TranslationUnit CreateTranslationUnit(string path)
        {
            TranslationUnit tu = new TranslationUnit(Index, path);
            tu.Parse(new string[] { "-Weverything" }, null);
            return tu;
        }
        
        static TranslationUnits()
        {
            Index = new LibClang.Index(false, true);
        }
    }

    public static class SimpleClassCppFile
    {
        public static string Path = "TestCode\\simple_class.cpp";

        //namespace TestCode
        //          ^
        public static int NamespaceDefinition = 39;

        //namespace TestCode
        //^
        public static int NamespaceStart = 29;

        //}
        // ^
        public static int NamespaceEnd = 318;

        //void SimpleClass::Method(int param)
        //                  ^
        public static int MethodDefinition = 290;

        //SimpleClass::SimpleClass()
        //             ^
        public static int ConstructorDefinition = 67;
    }

    public static class SimpleClassHeaderFile
    {
        public static string Path = "TestCode\\simple_class.h";

        //	class SimpleClass
        //        ^
        public static int ClassDefinition = 46;

        //void Method(int param);
        //                ^
        public static int MethodParam = 225;
    }

    public static class ErrorWarningCppFile
    {
        public static string Path = "TestCode\\error_warning_class.cpp";
    }

    public static class HeaderTestsCppFile
    {
        public static string Path = "TestCode\\header_tests.cpp";
    }

    public static class StringIncludeCppFile
    {
        public static string Path = "TestCode\\string_include.cpp";
    }
}
