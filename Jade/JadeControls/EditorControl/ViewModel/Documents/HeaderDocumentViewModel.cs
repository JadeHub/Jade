using JadeCore;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    public class HeaderDocumentViewModel : CodeDocumentViewModelBase
    {
        private Commands.SourceFileJumpToCommand _jumpToCommand;
        private Commands.FindAllReferences _findAllRefsCommand;

        public HeaderDocumentViewModel(IEditorDoc doc) 
            : base(doc)
        {
            Debug.Assert(doc is JadeCore.Editor.SourceDocument);
            if (HasIndex)
            {
                _jumpToCommand = new Commands.SourceFileJumpToCommand(this, doc.File.Path, doc.Project.Index);
                _findAllRefsCommand = new Commands.FindAllReferences(this, doc.File.Path, doc.Project.Index);
            }
        }

        public ICommand FindAllReferencesCommand
        {
            get { return _findAllRefsCommand; }
        }

        public ICommand JumpToCommand
        {
            get { return _jumpToCommand; }
        }
    }
}
