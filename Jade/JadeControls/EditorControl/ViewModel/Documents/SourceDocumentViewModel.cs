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
        private SourceFileJumpToCommand _jumpToCommand;

        public SourceDocumentViewModel(IEditorDoc doc) 
            : base(doc)
        {
            Debug.Assert(doc is JadeCore.Editor.SourceDocument);

            if (HasIndex)
            {
                _jumpToCommand = new SourceFileJumpToCommand(this, doc.File.Path, doc.Project.Index);
                SetIndexItem(Index.FindProjectItem(Document.File.Path));
                Index.ItemUpdated += ProjectIndexItemUpdated;
            }
        }

        private void SetIndexItem(CppCodeBrowser.IProjectFile indexItem)
        {
            DiagnosticHighlighter.ProjectItem = indexItem;
        }

        private void ProjectIndexItemUpdated(JadeUtils.IO.FilePath path)
        {
            if (path != Document.File.Path) return;

            SetIndexItem(Index.FindProjectItem(Document.File.Path));
        }

        public override void RegisterCommands(CommandBindingCollection commandBindings)
        {
            //FindAllReferences
            commandBindings.Add(new CommandBinding(EditorCommands.FindAllReferences,
                                        delegate(object target, ExecutedRoutedEventArgs a)
                                        {
                                            OnFindAllReferences(this.CaretOffset);
                                            a.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs a)
                                        {
                                            a.CanExecute = CanFindAllReferences(this.CaretOffset);
                                            a.Handled = true;
                                        }));
        }

        private void OnFindAllReferences(int offset)
        {
            Debug.Assert(HasIndex);

            CppCodeBrowser.CodeLocation location = new CppCodeBrowser.CodeLocation(Document.File.Path.Str, offset);
            JadeCore.Search.ISearch search = new JadeCore.Search.FindAllReferencesSearch(Index, location);
            Services.Provider.SearchController.RegisterSearch(search);
            search.Start();
        }

        private bool CanFindAllReferences(int offset)
        {
            return HasIndex;
        }

        public ICommand JumpToCommand
        {
            get { return _jumpToCommand; }
        }
    }
}
