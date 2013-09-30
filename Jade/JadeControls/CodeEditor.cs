using System;
using System.Windows;
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
            base.DocumentChanged += OnDocChange;
            
        }

        void OnDocChange(object sender, EventArgs e)
        {
            
        }
    }
}
