using JadeCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace JadeControls.EditorControl.ViewModel
{
    /// <summary>
    /// DocumentViewModel implementation for source code documents.
    /// </summary>
    public class SourceDocumentViewModel : DocumentViewModel
    {
        private CppCodeBrowser.ICodeBrowser _jumpToBrowser;
        private DiagnosticHighlighter _diagnosticHighlighter;
        private SearchHighlighter _searchHighlighter;
        private CppCodeBrowser.IProjectItem _sourceFileProjectItem;
        private CppCodeBrowser.IProjectIndex _projectIndex;

        public SourceDocumentViewModel(IEditorDoc doc, CppCodeBrowser.IProjectIndex index) 
            : base(doc)
        {
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(index);
            _sourceFileProjectItem = index.FindProjectItem(doc.Path);
            _projectIndex = index;
        }

        public override void RegisterCommands(CommandBindingCollection commandBindings)
        {
            //JumpTp
            commandBindings.Add(new CommandBinding(EditorCommands.JumpTo,
                                        delegate(object target, ExecutedRoutedEventArgs a)
                                        {
                                            OnJumpTo(this.CaretOffset);
                                            a.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs a)
                                        {
                                            a.CanExecute = CanJumpTo(this.CaretOffset);
                                            a.Handled = true;
                                        }));

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

        private void OnJumpTo(int offset)
        {
            CppCodeBrowser.ICodeLocation location = new CppCodeBrowser.CodeLocation(Document.File.Path.Str, offset);
            List<LibClang.Cursor> cursors = new List<LibClang.Cursor>(CppCodeBrowser.ProjectIndex.GetCursors(_sourceFileProjectItem.TranslationUnits, location));

            HashSet<CppCodeBrowser.ICodeLocation> results = new HashSet<CppCodeBrowser.ICodeLocation>();

            _jumpToBrowser.BrowseFrom(cursors,
                delegate(CppCodeBrowser.ICodeLocation result)
                                        {
                                            results.Add(result);
                                            return true;
                                        });

            if (results.Count() > 0)
            {
                JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
                cmdHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationParams(results.First(), true));
            }
        }

        private bool CanJumpTo(int offset)
        {
            return true;
        }

        private void OnFindAllReferences(int offset)
        {
            CppCodeBrowser.CodeLocation location = new CppCodeBrowser.CodeLocation(Document.File.Path.Str, offset);

            List<LibClang.Cursor> cursors = new List<LibClang.Cursor>(CppCodeBrowser.ProjectIndex.GetCursors(_projectIndex.TranslationUnits, location));

            JadeCore.Search.ISearch search = new JadeCore.Search.FindAllReferencesSearch(_projectIndex, location);
            Services.Provider.SearchController.RegisterSearch(search);
            search.Start();
        }

        private bool CanFindAllReferences(int offset)
        {
            return true;
        }

        protected override void OnSetView(CodeEditor view)
        {
            Highlighting.Underliner underliner = new Highlighting.Underliner(TextDocument);
            view.TextArea.TextView.BackgroundRenderers.Add(underliner);

            if (_sourceFileProjectItem != null)            
            {
             //   ASTHighlighter _astHighlighter = new ASTHighlighter(_fileBrowser.TranslationUnits.First().Cursor, underliner, _fileBrowser.Path.Str);
                _diagnosticHighlighter = new DiagnosticHighlighter(_sourceFileProjectItem, underliner);
                _searchHighlighter = new SearchHighlighter(_sourceFileProjectItem, underliner);
            }            
        }
    }
}
