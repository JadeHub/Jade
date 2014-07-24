using System;
using System.Windows.Controls;
using System.Windows;

namespace JadeControls.SymbolInspector
{
    public class GroupItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultnDataTemplate { get; set; }
        public DataTemplate CtorDataTemplate { get; set; }
        public DataTemplate MethodDataTemplate { get; set; }
        public DataTemplate DataMemberDataTemplate { get; set; }
        public DataTemplate BaseTypeDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ConstructorViewModel)
            {
                return CtorDataTemplate;
            }

            if (item is MethodDeclarationViewModel)
            {
                return MethodDataTemplate;
            }

            if (item is DataMemberViewModel)
            {
                return DataMemberDataTemplate;
            }

            if (item is ClassDeclarationViewModel)
            {
                return BaseTypeDataTemplate;
            }

            return DefaultnDataTemplate;
        }
    }
}
