using JadeUtils.IO;
using LibClang;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CppCodeBrowser
{   
    public interface IUnsavedFileProvider
    {
        IEnumerable<ParseFile> UnsavedFiles { get; }
    }
    
    public class ProjectIndexBuilder
    {
        static public void IndexTranslationUnit(ParseResult parseResult)
        {
            parseResult.Index.UpdateSourceFile(parseResult.Path, parseResult.TranslationUnit);
            System.Diagnostics.Debug.WriteLine("**Indexing " + parseResult.Path.FileName + " version:" + parseResult.GetFileVersion(parseResult.Path));
            IndexTranslationUnit(parseResult.Index, parseResult.TranslationUnit);
        }

        static private void IndexTranslationUnit(IProjectIndex index, TranslationUnit tu)        
        {            
            foreach (Cursor c in tu.Cursor.Children)
            {
                IndexCursor(index, c);
            }
        }

        static private bool IsIndexCursorKind(CursorKind k)
        {
            return 
                (
                    CursorKinds.IsClassStructEtc(k) ||
                    k == LibClang.CursorKind.CXXMethod ||
                    k == LibClang.CursorKind.Constructor ||
                    k == LibClang.CursorKind.Destructor ||
                    k == LibClang.CursorKind.FieldDecl ||
                    k == LibClang.CursorKind.ClassTemplate ||
                    k == LibClang.CursorKind.Namespace ||
                    k == LibClang.CursorKind.FunctionDecl ||
                    k == LibClang.CursorKind.VarDecl ||
                    k == LibClang.CursorKind.EnumDecl ||
                    k == LibClang.CursorKind.EnumConstantDecl ||
                    k == LibClang.CursorKind.ParamDecl ||
                    k == CursorKind.ConversionFunction ||
                    k == CursorKind.ParamDecl ||
                    k == CursorKind.TemplateTypeParameter ||
                    k == CursorKind.CallExpr ||
                    k == CursorKind.DeclRefExpr ||
                    k == CursorKind.MemberRefExpr ||
                    k == CursorKind.InclusionDirective ||
                    CursorKinds.IsReference(k) ||
                    CursorKinds.IsStatement(k) ||
                    CursorKinds.IsExpression(k)
                );
        }

        static private bool FilterCursor(Cursor c)
        {
            if (IsIndexCursorKind(c.Kind) == false) return false;
            if (c.Location == null || c.Location.File == null) return false;

            return true;
        }

        static private void IndexCursor(IProjectIndex index, Cursor c)
        {
            if (FilterCursor(c) == false) return;

            if (c.Kind == CursorKind.InclusionDirective)
            {
                int i = 0;
            }

            if (CursorKinds.IsDefinition(c.Kind) || c.Kind == CursorKind.InclusionDirective)
            {
                index.Symbols.UpdateDefinition(c);
            }
            else if (c.CursorReferenced != null)// && c.Kind != CursorKind.CallExpr)
            {
                if (FilterCursor(c.CursorReferenced))
                    index.Symbols.UpdateReference(c);
            } 

            foreach (Cursor child in c.Children)
            {
                IndexCursor(index, child);
            }
        }
    }
}
