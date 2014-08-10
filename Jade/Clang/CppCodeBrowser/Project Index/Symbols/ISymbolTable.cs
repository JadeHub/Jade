using LibClang;

namespace CppCodeBrowser.Symbols
{
    public interface ISymbolTable
    {
        ISymbolSet<NamespaceDecl> Namespaces { get; }
        ISymbolSet<ClassDecl> Classes { get; }
        ISymbolSet<MethodDecl> Methods { get; }
        ISymbolSet<EnumDecl> Enums { get; }
        ISymbolSet<EnumConstantDecl> EnumConstants { get; }
        ISymbolSet<ConstructorDecl> Constructors { get; }
        ISymbolSet<DestructorDecl> Destructors { get; }
        ISymbolSet<FieldDecl> Fields { get; }
        ISymbolSet<FunctionDecl> Functions { get; }
        ISymbolSet<TypedefDecl> Typedefs { get; }
        ISymbolSet<VariableDecl> Variables { get; }

        NamespaceDecl FindNamespaceDeclaration(string usr);
        ClassDecl FindClassDeclaration(string usr);
        MethodDecl FindMethodDeclaration(string usr);
        EnumConstantDecl FindEnumConstantDeclaration(string usr);
        EnumDecl FindEnumDeclaration(string usr);
        ConstructorDecl FindConstructorDeclaration(string usr);
        DestructorDecl FindDestructorDeclaration(string usr);
        FieldDecl FindFieldDeclaration(string usr);
        FunctionDecl FindFunctionDeclaration(string usr);
        TypedefDecl FindTypedefDeclaration(string usr);
        VariableDecl FindVariableDeclaration(string usr);

        void UpdateDefinition(Cursor c);
        void UpdateReference(Cursor c);
    }
}
