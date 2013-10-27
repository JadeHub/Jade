using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.Indexer
{
    public enum EntityKind
    {
        Unexposed = 0,
        Typedef = 1,
        Function = 2,
        Variable = 3,
        Field = 4,
        EnumConstant = 5,

        ObjCClass = 6,
        ObjCProtocol = 7,
        ObjCCategory = 8,

        ObjCInstanceMethod = 9,
        ObjCClassMethod = 10,
        ObjCProperty = 11,
        ObjCIvar = 12,

        Enum = 13,
        Struct = 14,
        Union = 15,

        CXXClass = 16,
        CXXNamespace = 17,
        CXXNamespaceAlias = 18,
        CXXStaticVariable = 19,
        CXXStaticMethod = 20,
        CXXInstanceMethod = 21,
        CXXConstructor = 22,
        CXXDestructor = 23,
        CXXConversionFunction = 24,
        CXXTypeAlias = 25,
        CXXInterface = 26
    }

    public enum EntityCXXTemplateKind
    {
        NonTemplate = 0,
        Template = 1,
        TemplatePartialSpecialization = 2,
        TemplateSpecialization = 3
    }

    public enum EntityLanguage
    {
        None = 0,
        C = 1,
        ObjC = 2,
        CXX = 3
    }

    public enum AttributeKind
    {
        Unexposed = 0,
        IBAction = 1,
        IBOutlet = 2,
        IBOutletCollection = 3
    }

    public enum EntityReferenceKind
    {
        Direct = 1,
        Implicit = 2
    };
}
