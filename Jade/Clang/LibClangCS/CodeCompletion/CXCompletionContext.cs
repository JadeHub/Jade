using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.CodeCompletion
{
    /**
     * \brief Bits that represent the context under which completion is occurring.
     *
     * The enumerators in this enumeration may be bitwise-OR'd together if multiple
     * contexts are occurring simultaneously.
     */
    internal enum CompletionContext
    {
        /**
         * \brief The context for completions is unexposed, as only Clang results
         * should be included. (This is equivalent to having no context bits set.)
         */
        Unexposed = 0,

        /**
         * \brief Completions for any possible type should be included in the results.
         */
        AnyType = 1 << 0,

        /**
         * \brief Completions for any possible value (variables, function calls, etc.)
         * should be included in the results.
         */
        AnyValue = 1 << 1,
        /**
         * \brief Completions for values that resolve to an Objective-C object should
         * be included in the results.
         */
        ObjCObjectValue = 1 << 2,
        /**
         * \brief Completions for values that resolve to an Objective-C selector
         * should be included in the results.
         */
        ObjCSelectorValue = 1 << 3,
        /**
         * \brief Completions for values that resolve to a C++ class type should be
         * included in the results.
         */
        CXXClassTypeValue = 1 << 4,

        /**
         * \brief Completions for fields of the member being accessed using the dot
         * operator should be included in the results.
         */
        DotMemberAccess = 1 << 5,
        /**
         * \brief Completions for fields of the member being accessed using the arrow
         * operator should be included in the results.
         */
        ArrowMemberAccess = 1 << 6,
        /**
         * \brief Completions for properties of the Objective-C object being accessed
         * using the dot operator should be included in the results.
         */
        ObjCPropertyAccess = 1 << 7,

        /**
         * \brief Completions for enum tags should be included in the results.
         */
        EnumTag = 1 << 8,
        /**
         * \brief Completions for union tags should be included in the results.
         */
        UnionTag = 1 << 9,
        /**
         * \brief Completions for struct tags should be included in the results.
         */
        StructTag = 1 << 10,

        /**
         * \brief Completions for C++ class names should be included in the results.
         */
        ClassTag = 1 << 11,
        /**
         * \brief Completions for C++ namespaces and namespace aliases should be
         * included in the results.
         */
        Namespace = 1 << 12,
        /**
         * \brief Completions for C++ nested name specifiers should be included in
         * the results.
         */
        NestedNameSpecifier = 1 << 13,

        /**
         * \brief Completions for Objective-C interfaces (classes) should be included
         * in the results.
         */
        ObjCInterface = 1 << 14,
        /**
         * \brief Completions for Objective-C protocols should be included in
         * the results.
         */
        ObjCProtocol = 1 << 15,
        /**
         * \brief Completions for Objective-C categories should be included in
         * the results.
         */
        ObjCCategory = 1 << 16,
        /**
         * \brief Completions for Objective-C instance messages should be included
         * in the results.
         */
        ObjCInstanceMessage = 1 << 17,
        /**
         * \brief Completions for Objective-C class messages should be included in
         * the results.
         */
        ObjCClassMessage = 1 << 18,
        /**
         * \brief Completions for Objective-C selector names should be included in
         * the results.
         */
        ObjCSelectorName = 1 << 19,

        /**
         * \brief Completions for preprocessor macro names should be included in
         * the results.
         */
        MacroName = 1 << 20,

        /**
         * \brief Natural language completions should be included in the results.
         */
        NaturalLanguage = 1 << 21,

        /**
         * \brief The current context is unknown, so set all contexts.
         */
        Unknown = ((1 << 22) - 1)
    }
}
