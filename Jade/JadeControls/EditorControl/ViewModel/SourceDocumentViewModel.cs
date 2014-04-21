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
    /// DocumentViewModel implementation for source code documents.
    /// </summary>
    public class SourceDocumentViewModel : DocumentViewModel
    {
        private CppCodeBrowser.IProjectIndex _projectIndex;
        private CppCodeBrowser.ICodeBrowser _jumpToBrowser;
        private DiagnosticHighlighter _diagnosticHighlighter;
        private SearchHighlighter _searchHighlighter;
        private CppCodeBrowser.IProjectItem _sourceFileProjectItem;
        
        public SourceDocumentViewModel(IEditorDoc doc) 
            : base(doc)
        {
            Debug.Assert(doc is JadeCore.Editor.EditorSourceDocument);
            _projectIndex = (doc as JadeCore.Editor.EditorSourceDocument).ProjectIndex;
            if (_projectIndex != null)
            {
                _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(_projectIndex);
                _sourceFileProjectItem = _projectIndex.FindProjectItem(doc.File.Path);
            }
        }

        private bool HasProjectIndex { get { return _projectIndex != null && _sourceFileProjectItem != null; } }

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

        private bool CanJumpTo(LibClang.Cursor cursor, int offset)
        {
            if (cursor.Extent == null) 
                return false;

            if (cursor.Extent.Tokens == null)
                return false;

            Token tok = cursor.Extent.Tokens.GetTokenAtOffset(offset);
            return (tok != null && tok.Kind == TokenKind.Identifier);
        }

        private CppCodeBrowser.ICodeLocation JumpTo(int offset)
        {
            CppCodeBrowser.ICodeLocation location = new CppCodeBrowser.CodeLocation(Document.File.Path.Str, offset);
            List<LibClang.Cursor> cursors = new List<LibClang.Cursor>(CppCodeBrowser.ProjectIndex.GetCursors(_sourceFileProjectItem.TranslationUnits, location));

            HashSet<CppCodeBrowser.ICodeLocation> results = new HashSet<CppCodeBrowser.ICodeLocation>();

            _jumpToBrowser.BrowseFrom(from c in cursors where CanJumpTo(c, offset) select c,
                delegate(CppCodeBrowser.ICodeLocation result)
                                        {
                                            results.Add(result);
                                            return true;
                                        });

            return results.Count() > 0 ? results.First() : null;
        }

        private void OnJumpTo(int offset)
        {
            if (!HasProjectIndex) return;

            CppCodeBrowser.ICodeLocation result = JumpTo(offset);

            if (result != null)
            {
                
                JadeCore.Services.Provider.CommandHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(result, true, true));
            }
        }

        private bool CanJumpTo(int offset)
        {
            return JumpTo(offset) != null;
        }

        private void OnFindAllReferences(int offset)
        {
            if (!HasProjectIndex) return;

            CppCodeBrowser.CodeLocation location = new CppCodeBrowser.CodeLocation(Document.File.Path.Str, offset);

            List<LibClang.Cursor> cursors = new List<LibClang.Cursor>(CppCodeBrowser.ProjectIndex.GetCursors(_projectIndex.TranslationUnits, location));

            JadeCore.Search.ISearch search = new JadeCore.Search.FindAllReferencesSearch(_projectIndex, location);
            Services.Provider.SearchController.RegisterSearch(search);
            search.Start();
        }

        private bool CanFindAllReferences(int offset)
        {
            return HasProjectIndex;
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

            view.KeyDown += OnViewKeyDown;
        }

        void OnViewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                _searchHighlighter.Clear();
            }
        }
    }
}
