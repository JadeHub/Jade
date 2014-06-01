using ICSharpCode.AvalonEdit;
using JadeControls.EditorControl.ViewModel;
using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace JadeControls.EditorControl
{
    using ICSharpCode.AvalonEdit.Highlighting;
    /// <summary>
    /// Wrapper around the Avalon TextEditor
    /// </summary>
    public class CodeEditor : TextEditor
    {
        static private IHighlightingDefinition highlighDefinition;

        static CodeEditor()
        {
            using (Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("JadeControls.EditorControl.cpp_rules.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    highlighDefinition = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
        
        public CodeEditor()
        {
            ShowLineNumbers = true;            
            TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
            this.DataContextChanged += CodeEditor_DataContextChanged;
            this.SyntaxHighlighting = highlighDefinition;
            this.Options.CutCopyWholeLine = false;
        }

        private void CodeEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null)
            {
                Debug.Assert(DataContext is DocumentViewModel);
                DocumentViewModel vm = DataContext as DocumentViewModel;
                vm.SetView(this);
            }
        }

        private void TextArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = GetPositionFromPoint(e.GetPosition(this));
            if (position.HasValue)
            {
                TextArea.Caret.Position = position.Value;
            }
        }

    }
}
