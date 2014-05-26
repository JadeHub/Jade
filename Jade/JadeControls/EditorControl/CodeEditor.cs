using ICSharpCode.AvalonEdit;
using JadeControls.EditorControl.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace JadeControls.EditorControl
{
    /// <summary>
    /// Wrapper around the Avalon TextEditor
    /// </summary>
    public class CodeEditor : TextEditor
    {
        public CodeEditor()
        {
            ShowLineNumbers = true;            
            TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
            this.DataContextChanged += CodeEditor_DataContextChanged;
            SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C++");
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
