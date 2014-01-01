using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using JadeControls.EditorControl.ViewModel;

namespace JadeControls.EditorControl
{
    /// <summary>
    /// Wrapper around the Avalon TextEditor
    /// </summary>
    public class CodeEditor : TextEditor
    {
        private bool _firstLoad = true;

        public CodeEditor()
        {
            ShowLineNumbers = true;            
            TextArea.IsVisibleChanged += CodeEditor_IsVisibleChanged;
            TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
            this.Loaded += CodeEditor_Loaded;

            this.DataContextChanged += CodeEditor_DataContextChanged;
        }

        void CodeEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && DataContext is DocumentViewModel)
            {
                DocumentViewModel vm = DataContext as DocumentViewModel;
                vm.SetView(this);
                vm.RegisterCommands(this.CommandBindings);
            }
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
            if (_firstLoad)
            {
                Keyboard.Focus(TextArea);
                _firstLoad = false;
            }
        }

        void CodeEditor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)(e.NewValue))
                return;
            
        }
    }
}
