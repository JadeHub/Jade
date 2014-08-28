using JadeCore;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    /// <summary>
    /// View model for source code documents.
    /// </summary>
    public class SourceDocumentViewModel : CodeDocumentViewModelBase
    {
        private Commands.CodeCompleteCommand _codeCompleteCommand;
        //private CodeCompletion.CompletionEngine _codeCompletion;
        
        public SourceDocumentViewModel(IEditorDoc doc) 
            : base(doc)
        {
            Debug.Assert(doc is JadeCore.Editor.SourceDocument);
            if (HasIndex)
            {
                _codeCompleteCommand = new Commands.CodeCompleteCommand(this, doc.File.Path, doc.Project.Index);                
            }
        }

        public ICommand CodeCompleteCommand
        {
            get { return _codeCompleteCommand; }
        }

        protected override void OnSetView(CodeEditor view)
        {
            base.OnSetView(view);
            //Debug.Assert(_codeCompletion == null);

//            _codeCompletion = new CodeCompletion.CompletionEngine(Document.TextDocument, view.TextArea, new CodeCompletion.ResultProvider(Document.File.Path, Document.Project.Index, JadeCore.Services.Provider.EditorController));
  //          _codeCompleteCommand.CodeCompletion = _codeCompletion;

            view.InputBindings.Add(new InputBinding(CodeCompleteCommand, new KeyGesture(Key.Space, ModifierKeys.Control)));
            view.InputBindings.Add(new InputBinding(JumpToCommand, new KeyGesture(Key.F12)));
        }
    }
}
