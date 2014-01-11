namespace LibClang
{

/// <summary>
/// Describes the type of entity refered to by a Cursor.
/// The comments below come directly from the libclang source
/// </summary>
public enum CursorKind
{
    //Declarations 
    
    //A declaration whose specific kind is not exposed via this
    //interface.
        
    //Unexposed declarations have the same operations as any other kind
    //of declaration; one can extract their location information,
    //spelling, find their definitions, etc. However, the specific kind
    //of the declaration is not reported.
        
    UnexposedDecl = 1,
    // A C or C++ struct. 
    StructDecl = 2,
    // A C or C++ union. 
    UnionDecl = 3,
    // A C++ class. 
    ClassDecl = 4,
    // An enumeration. 
    EnumDecl = 5,
    
    //A field (in C) or non-static data member (in C++) in a
    //struct, union, or C++ class.
        
    FieldDecl = 6,
    // An enumerator constant. 
    EnumConstantDecl = 7,
    // A function. 
    FunctionDecl = 8,
    // A variable. 
    VarDecl = 9,
    // A function or method parameter. 
    ParmDecl = 10,
    // An Objective-C \@interface. 
    ObjCInterfaceDecl = 11,
    // An Objective-C \@interface for a category. 
    ObjCCategoryDecl = 12,
    // An Objective-C \@protocol declaration. 
    ObjCProtocolDecl = 13,
    // An Objective-C \@property declaration. 
    ObjCPropertyDecl = 14,
    // An Objective-C instance variable. 
    ObjCIvarDecl = 15,
    // An Objective-C instance method. 
    ObjCInstanceMethodDecl = 16,
    // An Objective-C class method. 
    ObjCClassMethodDecl = 17,
    // An Objective-C \@implementation. 
    ObjCImplementationDecl = 18,
    // An Objective-C \@implementation for a category. 
    ObjCCategoryImplDecl = 19,
    // A typedef 
    TypedefDecl = 20,
    // A C++ class method. 
    CXXMethod = 21,
    // A C++ namespace. 
    Namespace = 22,
    // A linkage specification, e.g. 'extern "C"'. 
    LinkageSpec = 23,
    // A C++ constructor. 
    Constructor = 24,
    // A C++ destructor. 
    Destructor = 25,
    // A C++ conversion function. 
    ConversionFunction = 26,
    // A C++ template type parameter. 
    TemplateTypeParameter = 27,
    // A C++ non-type template parameter. 
    NonTypeTemplateParameter = 28,
    // A C++ template template parameter. 
    TemplateTemplateParameter = 29,
    // A C++ function template. 
    FunctionTemplate = 30,
    // A C++ class template. 
    ClassTemplate = 31,
    // A C++ class template partial specialization. 
    ClassTemplatePartialSpecialization = 32,
    // A C++ namespace alias declaration. 
    NamespaceAlias = 33,
    // A C++ using directive. 
    UsingDirective = 34,
    // A C++ using declaration. 
    UsingDeclaration = 35,
    // A C++ alias declaration 
    TypeAliasDecl = 36,
    // An Objective-C \@synthesize definition. 
    ObjCSynthesizeDecl = 37,
    // An Objective-C \@dynamic definition. 
    ObjCDynamicDecl = 38,
    // An access specifier. 
    CXXAccessSpecifier = 39,

    FirstDecl = UnexposedDecl,
    LastDecl = CXXAccessSpecifier,

    //References 
    FirstRef = 40, //Decl references 
    ObjCSuperClassRef = 40,
    ObjCProtocolRef = 41,
    ObjCClassRef = 42,
    
    //A reference to a type declaration.
        
    //A type reference occurs anywhere where a type is named but not
    //declared. For example, given:
        
    //\code
    //typedef unsigned size_type;
    //size_type size;
    //\endcode
        
    //The typedef is a declaration of size_type (TypedefDecl),
    //while the type of the variable "size" is referenced. The cursor
    //referenced by the type of size is the typedef for size_type.
        
    TypeRef = 43,
    CXXBaseSpecifier = 44,
     
    //A reference to a class template, function template, template
    //template parameter, or class template partial specialization.
        
    TemplateRef = 45,
    
    //A reference to a namespace or namespace alias.
        
    NamespaceRef = 46,
    
    //A reference to a member of a struct, union, or class that occurs in 
    //some non-expression context, e.g., a designated initializer.
        
    MemberRef = 47,
    
    //A reference to a labeled statement.
    
    //This cursor kind is used to describe the jump to "start_over" in the 
    //goto statement in the following example:
    
    //\code
    //  start_over:
    //    ++counter;
    
    //    goto start_over;
    //\endcode
    
    //A label reference cursor refers to a label statement.
        
    LabelRef = 48,

    
    //A reference to a set of overloaded functions or function templates
    //that has not yet been resolved to a specific function or function template.
        
    //An overloaded declaration reference cursor occurs in C++ templates where
    //a dependent name refers to a function. For example:
        
    //\code
    //template<typename T> void swap(T&, T&);
        
    //struct X { ... };
    //void swap(X&, X&);
        
    //template<typename T>
    //void reverse(T//first, T//last) {
    //  while (first < last - 1) {
    //    swap(first, --last);
    //    ++first;
    //  }
    //}
        
    //struct Y { };
    //void swap(Y&, Y&);
    //\endcode
        
    //Here, the identifier "swap" is associated with an overloaded declaration
    //reference. In the template definition, "swap" refers to either of the two
    //"swap" functions declared above, so both results will be available. At
    //instantiation time, "swap" may also refer to other functions found via
    //argument-dependent lookup (e.g., the "swap" function at the end of the
    //example).
        
    //The functions \c clang_getNumOverloadedDecls() and 
    //\c clang_getOverloadedDecl() can be used to retrieve the definitions
    //referenced by this cursor.
        
    OverloadedDeclRef = 49,

    
    //A reference to a variable that occurs in some non-expression 
    //context, e.g., a C++ lambda capture list.
        
    VariableRef = 50,

    LastRef = VariableRef,

    //Error conditions 
    FirstInvalid = 70,
    InvalidFile = 70,
    NoDeclFound = 71,
    NotImplemented = 72,
    InvalidCode = 73,
    LastInvalid = InvalidCode,

    //Expressions 
    FirstExpr = 100,

    
    //An expression whose specific kind is not exposed via this
    //interface.
        
    //Unexposed expressions have the same operations as any other kind
    //of expression; one can extract their location information,
    //spelling, children, etc. However, the specific kind of the
    //expression is not reported.
        
    UnexposedExpr = 100,

    
    //An expression that refers to some value declaration, such
    //as a function, varible, or enumerator.
        
    DeclRefExpr = 101,

    
    //An expression that refers to a member of a struct, union,
    //class, Objective-C class, etc.
        
    MemberRefExpr = 102,

    // An expression that calls a function. 
    CallExpr = 103,

    // An expression that sends a message to an Objective-C object or class. 
    ObjCMessageExpr = 104,

    // An expression that represents a block literal. 
    BlockExpr = 105,

    // An integer literal.
        
    IntegerLiteral = 106,

    // A floating point number literal.
        
    FloatingLiteral = 107,

    // An imaginary number literal.
        
    ImaginaryLiteral = 108,

    // A string literal.
        
    StringLiteral = 109,

    // A character literal.
        
    CharacterLiteral = 110,

    // A parenthesized expression, e.g. "(1)".
        
    //This AST node is only formed if full location information is requested.
        
    ParenExpr = 111,

    // This represents the unary-expression's (except sizeof and
    //alignof).
        
    UnaryOperator = 112,

    // [C99 6.5.2.1] Array Subscripting.
        
    ArraySubscriptExpr = 113,

    // A builtin binary operation expression such as "x + y" or
    //"x <= y".
        
    BinaryOperator = 114,

    // Compound assignment such as "+=".
        
    CompoundAssignOperator = 115,

    // The ?: ternary operator.
        
    ConditionalOperator = 116,

    // An explicit cast in C (C99 6.5.4) or a C-style cast in C++
    //(C++ [expr.cast]), which uses the syntax (Type)expr.
        
    //For example: (int)f.
        
    CStyleCastExpr = 117,

    // [C99 6.5.2.5]
        
    CompoundLiteralExpr = 118,

    // Describes an C or C++ initializer list.
        
    InitListExpr = 119,

    // The GNU address of label extension, representing &&label.
        
    AddrLabelExpr = 120,

    // This is the GNU Statement Expression extension: ({int X=4; X;})
        
    StmtExpr = 121,

    // Represents a C11 generic selection.
        
    GenericSelectionExpr = 122,

    // Implements the GNU __null extension, which is a name for a null
    //pointer constant that has integral type (e.g., int or long) and is the same
    //size and alignment as a pointer.
        
    //The __null extension is typically only used by system headers, which define
    //NULL as __null in C++ rather than using 0 (which is an integer that may not
    //match the size of a pointer).
        
    GNUNullExpr = 123,

    // C++'s static_cast<> expression.
        
    CXXStaticCastExpr = 124,

    // C++'s dynamic_cast<> expression.
        
    CXXDynamicCastExpr = 125,

    // C++'s reinterpret_cast<> expression.
        
    CXXReinterpretCastExpr = 126,

    // C++'s const_cast<> expression.
        
    CXXConstCastExpr = 127,

    // Represents an explicit C++ type conversion that uses "functional"
    //notion (C++ [expr.type.conv]).
        
    //Example:
    //\code
    //  x = int(0.5);
    //\endcode
        
    CXXFunctionalCastExpr = 128,

    // A C++ typeid expression (C++ [expr.typeid]).
        
    CXXTypeidExpr = 129,

    // [C++ 2.13.5] C++ Boolean Literal.
        
    CXXBoolLiteralExpr = 130,

    // [C++0x 2.14.7] C++ Pointer Literal.
        
    CXXNullPtrLiteralExpr = 131,

    // Represents the "this" expression in C++
        
    CXXThisExpr = 132,

    // [C++ 15] C++ Throw Expression.
        
    //This handles 'throw' and 'throw' assignment-expression. When
    //assignment-expression isn't present, Op will be null.
        
    CXXThrowExpr = 133,

    // A new expression for memory allocation and constructor calls, e.g:
    //"new CXXNewExpr(foo)".
        
    CXXNewExpr = 134,

    // A delete expression for memory deallocation and destructor calls,
    //e.g. "delete[] pArray".
        
    CXXDeleteExpr = 135,

    // A unary expression.
        
    UnaryExpr = 136,

    // An Objective-C string literal i.e. @"foo".
        
    ObjCStringLiteral = 137,

    // An Objective-C \@encode expression.
        
    ObjCEncodeExpr = 138,

    // An Objective-C \@selector expression.
        
    ObjCSelectorExpr = 139,

    // An Objective-C \@protocol expression.
        
    ObjCProtocolExpr = 140,

    // An Objective-C "bridged" cast expression, which casts between
    //Objective-C pointers and C pointers, transferring ownership in the process.
        
    //\code
    //  NSString str = (__bridge_transfer NSString )CFCreateString();
    //\endcode
        
    ObjCBridgedCastExpr = 141,

    // Represents a C++0x pack expansion that produces a sequence of
    //expressions.
        
    //A pack expansion expression contains a pattern (which itself is an
    //expression) followed by an ellipsis. For example:
        
    //\code
    //template<typename F, typename ...Types>
    //void forward(F f, Types &&...args) {
    // f(static_cast<Types&&>(args)...);
    //}
    //\endcode
        
    PackExpansionExpr = 142,

    // Represents an expression that computes the length of a parameter
    //pack.
        
    //\code
    //template<typename ...Types>
    //struct count {
    //  static const unsigned value = sizeof...(Types);
    //};
    //\endcode
        
    SizeOfPackExpr = 143,

    //Represents a C++ lambda expression that produces a local function
    //object.
        
    //\code
    //void abssort(float x, unsigned N) {
    //  std::sort(x, x + N,
    //            [](float a, float b) {
    //              return std::abs(a) < std::abs(b);
    //            });
    //}
    //\endcode
        
    LambdaExpr = 144,

    // Objective-c Boolean Literal.
        
    ObjCBoolLiteralExpr = 145,

    // Represents the "self" expression in a ObjC method.
        
    ObjCSelfExpr = 146,

    LastExpr = ObjCSelfExpr,

    //Statements 
    FirstStmt = 200,
    
    //A statement whose specific kind is not exposed via this
    //interface.
        
    //Unexposed statements have the same operations as any other kind of
    //statement; one can extract their location information, spelling,
    //children, etc. However, the specific kind of the statement is not
    //reported.
        
    UnexposedStmt = 200,

    // A labelled statement in a function. 
        
    //This cursor kind is used to describe the "start_over:" label statement in 
    //the following example:
        
    //\code
    //  start_over:
    //    ++counter;
    //\endcode
        
        
    LabelStmt = 201,

    // A group of statements like { stmt stmt }.
        
    //This cursor kind is used to describe compound statements, e.g. function
    //bodies.
        
    CompoundStmt = 202,

    // A case statment.
        
    CaseStmt = 203,

    // A default statement.
        
    DefaultStmt = 204,

    // An if statement
        
    IfStmt = 205,

    // A switch statement.
        
    SwitchStmt = 206,

    // A while statement.
        
    WhileStmt = 207,

    // A do statement.
        
    DoStmt = 208,

    // A for statement.
        
    ForStmt = 209,

    // A goto statement.
        
    GotoStmt = 210,

    // An indirect goto statement.
        
    IndirectGotoStmt = 211,

    // A continue statement.
        
    ContinueStmt = 212,

    // A break statement.
        
    BreakStmt = 213,

    // A return statement.
        
    ReturnStmt = 214,

    // A GCC inline assembly statement extension.
        
    GCCAsmStmt = 215,
    AsmStmt = GCCAsmStmt,

    // Objective-C's overall \@try-\@catch-\@finally statement.
        
    ObjCAtTryStmt = 216,

    // Objective-C's \@catch statement.
        
    ObjCAtCatchStmt = 217,

    // Objective-C's \@finally statement.
        
    ObjCAtFinallyStmt = 218,

    // Objective-C's \@throw statement.
        
    ObjCAtThrowStmt = 219,

    // Objective-C's \@synchronized statement.
        
    ObjCAtSynchronizedStmt = 220,

    // Objective-C's autorelease pool statement.
        
    ObjCAutoreleasePoolStmt = 221,

    // Objective-C's collection statement.
        
    ObjCForCollectionStmt = 222,

    // C++'s catch statement.
        
    CXXCatchStmt = 223,

    // C++'s try statement.
        
    CXXTryStmt = 224,

    // C++'s for (//: ) statement.
        
    CXXForRangeStmt = 225,

    // Windows Structured Exception Handling's try statement.
        
    SEHTryStmt = 226,

    // Windows Structured Exception Handling's except statement.
        
    SEHExceptStmt = 227,

    // Windows Structured Exception Handling's finally statement.
        
    SEHFinallyStmt = 228,

    // A MS inline assembly statement extension.
        
    MSAsmStmt = 229,

    // The null satement ";": C99 6.8.3p3.
        
    //This cursor kind is used to describe the null statement.
        
    NullStmt = 230,

    // Adaptor class for mixing declarations with statements and
    //expressions.
        
    DeclStmt = 231,

    // OpenMP parallel directive.
        
    OMPParallelDirective = 232,

    LastStmt = OMPParallelDirective,

    
    //Cursor that represents the translation unit itself.
        
    //The translation unit cursor exists primarily to act as the root
    //cursor for traversing the contents of a translation unit.
        
    TranslationUnit = 300,

    //Attributes 
    FirstAttr = 400,
    
    //An attribute whose specific kind is not exposed via this
    //interface.
        
    UnexposedAttr = 400,

    IBActionAttr = 401,
    IBOutletAttr = 402,
    IBOutletCollectionAttr = 403,
    CXXFinalAttr = 404,
    CXXOverrideAttr = 405,
    AnnotateAttr = 406,
    AsmLabelAttr = 407,
    PackedAttr = 408,
    LastAttr = PackedAttr,

    //Preprocessing 
    PreprocessingDirective = 500,
    MacroDefinition = 501,
    MacroExpansion = 502,
    MacroInstantiation = MacroExpansion,
    InclusionDirective = 503,
    FirstPreprocessing = PreprocessingDirective,
    LastPreprocessing = InclusionDirective,

    //Extra Declarations 
    
    //A module import declaration.
        
    ModuleImportDecl = 600,
    FirstExtraDecl = ModuleImportDecl,
    LastExtraDecl = ModuleImportDecl
};

}
