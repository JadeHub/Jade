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
        private Commands.SourceFileJumpToCommand _jumpToCommand;
        private Commands.FindAllReferences _findAllRefsCommand;

        public SourceDocumentViewModel(IEditorDoc doc) 
            : base(doc)
        {
            Debug.Assert(doc is JadeCore.Editor.SourceDocument);
            if (HasIndex)
            {
                _jumpToCommand = new Commands.SourceFileJumpToCommand(this, doc.File.Path, doc.Project.Index);
                _findAllRefsCommand = new Commands.FindAllReferences(this, doc.File.Path, doc.Project.Index);
            }
        }

        public ICommand JumpToCommand
        {
            get { return _jumpToCommand; }
        }

        public ICommand FindAllReferencesCommand
        {
            get { return _findAllRefsCommand; }
        }

        public LibClang.Cursor CurrentCppCursor
        {
            get
            {
                if (HasIndex)
                {
                    CppCodeBrowser.ISourceFile fileIndex = Index.FindSourceFile(Document.File.Path);
                    if (fileIndex != null)
                        return fileIndex.GetCursorAt(Document.File.Path, CaretOffset);
                }
                return null;
            }
        }

        protected override void OnSetView(CodeEditor view)
        {
            base.OnSetView(view);
            /*
            view.CommandBindings.Add(new CommandBinding(JadeCore.Commands.DisplaySymbolInspector,
                                        delegate(object target, ExecutedRoutedEventArgs args)
                                        {
                                 //           args.Parameter = 5;
                                            if (HasIndex)
                                            {
                                            }
                                            //args.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs args)
                                        {
                                            args.CanExecute = true;
                                            args.Handled = true;
                                        }));*/
        }
    }
}
