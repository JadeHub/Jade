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
        private DiagnosticHighlighter _diagnosticHighlighter;
        private SearchHighlighter _searchHighlighter;
        private CppCodeBrowser.IProjectFile _sourceFileProjectItem;
        private Highlighting.Underliner _underliner;
        private JumpToHelper _jumpToHelper;
        
        public SourceDocumentViewModel(IEditorDoc doc) 
            : base(doc)
        {
            Debug.Assert(doc is JadeCore.Editor.SourceDocument);

            if(doc.Project != null)
            {
                _projectIndex = doc.Project.Index;
            }                        
            if (_projectIndex != null)
            {
                _sourceFileProjectItem = _projectIndex.FindProjectItem(doc.File.Path);
                _projectIndex.ItemUpdated += ProjectIndexItemUpdated;
            }            

            _jumpToHelper = new JumpToHelper(_projectIndex, doc);
            _underliner = new Highlighting.Underliner(TextDocument);
            _diagnosticHighlighter = new DiagnosticHighlighter(_underliner);
        }

        private void ProjectIndexItemUpdated(JadeUtils.IO.FilePath path)
        {
            if (path != Document.File.Path) return;

            _sourceFileProjectItem = _projectIndex.FindProjectItem(Document.File.Path);
            if (_sourceFileProjectItem != null)
            {
                _jumpToHelper.ProjectItemIndex = _sourceFileProjectItem;
                _diagnosticHighlighter.ProjectItem = _sourceFileProjectItem;
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
        
        private void OnJumpTo(int offset)
        {
            CppCodeBrowser.ICodeLocation result = _jumpToHelper.JumpTo(offset);

            if (result != null)
            {                
                JadeCore.Services.Provider.CommandHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(result, true, true));
            }
        }

        private bool CanJumpTo(int offset)
        {
            return _jumpToHelper.CanJumpTo(offset);
        }

        private void OnFindAllReferences(int offset)
        {
            Debug.Assert(_projectIndex != null);

            CppCodeBrowser.CodeLocation location = new CppCodeBrowser.CodeLocation(Document.File.Path.Str, offset);
            JadeCore.Search.ISearch search = new JadeCore.Search.FindAllReferencesSearch(_projectIndex, location);
            Services.Provider.SearchController.RegisterSearch(search);
            search.Start();
        }

        private bool CanFindAllReferences(int offset)
        {
            return _projectIndex != null;
        }

        protected override void OnSetView(CodeEditor view)
        {
            view.TextArea.TextView.BackgroundRenderers.Add(_underliner);

            if (_sourceFileProjectItem != null)            
            {
             //   ASTHighlighter _astHighlighter = new ASTHighlighter(_fileBrowser.TranslationUnits.First().Cursor, underliner, _fileBrowser.Path.Str);
                _diagnosticHighlighter.ProjectItem = _sourceFileProjectItem;
                _searchHighlighter = new SearchHighlighter(_sourceFileProjectItem, _underliner);
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

        public string Path
        {
            get { return Document.File.Path.Str; }
        }
    }
}
