using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace JadeControls.EditorControl.CodeCompletion
{
    public class OverloadProvider : NotifyPropertyChanged, IOverloadProvider
    {
        private LibClang.CodeCompletion.Result _result;
        private CompletionSelection _selection;

        public OverloadProvider(LibClang.CodeCompletion.Result r, CompletionSelection selection)
        {
            _result = r;
            _selection = selection;
        }

        /// <summary>
        /// Gets/Sets the selected index.
        /// </summary>
        public int SelectedIndex 
        { 
            get; set; 
        }

        /// <summary>
        /// Gets the number of overloads.
        /// </summary>
        public int Count
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text 'SelectedIndex of Count'.
        /// </summary>
        public string CurrentIndexText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current header.
        /// </summary>
        public object CurrentHeader
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current content.
        /// </summary>
        public object CurrentContent
        {
            get;
            private set;
        }
    }
}
