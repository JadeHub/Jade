﻿using ICSharpCode.AvalonEdit;
using JadeControls.EditorControl.ViewModel;
using System.Windows;
using System.Windows.Input;

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
            TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
            this.Loaded += CodeEditor_Loaded;

            this.DataContextChanged += CodeEditor_DataContextChanged;
        }

        private void CodeEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && DataContext is DocumentViewModel)
            {
                DocumentViewModel vm = DataContext as DocumentViewModel;
                vm.SetView(this);
                
                TextArea.TextView.BackgroundRenderers.Add(new Highlighting.Underliner(vm.TextDocument));
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

        private void CodeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstLoad)
            {
                Keyboard.Focus(TextArea);
                _firstLoad = false;
            }
        }
    }
}
