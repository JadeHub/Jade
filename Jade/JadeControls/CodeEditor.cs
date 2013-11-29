using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;

namespace JadeControls
{
    /// <summary>
    /// Wrapper around the Avalon TextEditor
    /// </summary>
    public class CodeEditor : TextEditor//, INotifyPropertyChanged
    {
        public CodeEditor()
        {
            ShowLineNumbers = true;            
            TextArea.IsVisibleChanged += CodeEditor_IsVisibleChanged;
            TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
            this.Loaded += CodeEditor_Loaded;

        }

        void TextArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = GetPositionFromPoint(e.GetPosition(this));
            if (position.HasValue)
            {
                TextArea.Caret.Position = position.Value;
            }
        }

        void CodeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(TextArea);
        }

        void CodeEditor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)(e.NewValue))
                return;
            
        }
    
        void OnDocChange(object sender, EventArgs e)
        {
            
        }
    }
}
