using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JadeCore;
using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    internal static class DiagnosticOutputWriter
    {
        static private string FormatMsg(LibClang.Diagnostic diag)
        {
            return diag.DiagnosticSeverity.ToString() + " : " + diag.Location + " : " + diag.Spelling;
        }

        static internal void UpdateOutput(IList<LibClang.Diagnostic> diagnostics)
        {
            JadeCore.Output.IOutputController controller = JadeCore.Services.Provider.OutputController;
            controller.Clear();

            foreach(LibClang.Diagnostic diag in diagnostics)
            {
                
                controller.Create(JadeCore.Output.Source.Compilation, JadeCore.Output.Level.Info, FormatMsg(diag));
            }            
        }
    }

    public abstract class CodeDocumentViewModelBase : DocumentViewModel
    {
        private Commands.InspectSymbolCommand _inspectSymbolCommand;
        private Commands.InspectCursorCommand _inspectCursorCommand;
        private Commands.SourceFileJumpToCommand _jumpToCommand;        
        private Commands.FindAllReferences _findAllRefsCommand;

        private CppCodeBrowser.Symbols.FileMapping.IFileMap _fileMap;

        private IndexHighlighter _indexHighlighter;
        
        internal CodeDocumentViewModelBase(IEditorDoc doc)
            : base(doc)
        {
            DiagnosticHighlighter = new DiagnosticHighlighter(new Highlighting.Highlighter(TextDocument));
            SearchHighlighter = new SearchHighlighter(Document.File.Path, new Highlighting.Highlighter(TextDocument));
            _indexHighlighter = new IndexHighlighter(new Highlighting.Highlighter(TextDocument));

            if(HasIndex)
            {
                JadeCore.Services.Provider.CppParser.TranslationUnitIndexed += OnCppParserTranslationUnitIndexed;

                //DiagnosticHighlighter.ProjectItem = Document.Project.Index.FindProjectItem(Document.File.Path);
               
                _inspectSymbolCommand = new Commands.InspectSymbolCommand(this, doc.File.Path, doc.Project.Index);
                _inspectCursorCommand = new Commands.InspectCursorCommand(this, doc.File.Path, doc.Project.Index);
                _jumpToCommand = new Commands.SourceFileJumpToCommand(this, doc.File.Path, doc.Project.Index);
                _findAllRefsCommand = new Commands.FindAllReferences(this, doc.File.Path, doc.Project.Index);

                _fileMap = Document.Project.Index.FileSymbolMaps.GetMap(Document.File.Path);
                if (_fileMap != null)
                {
                    _indexHighlighter.SetMap(_fileMap);
                }
            }

        }

        private void OnCppParserTranslationUnitIndexed(CppCodeBrowser.ParseResult result)
        {
            if (result.Path != Document.File.Path) return;

            //highlight the symbol mappings
            if (_fileMap == null)
            {
                _fileMap = result.Index.FileSymbolMaps.GetMap(Document.File.Path);
                if (_fileMap != null)
                {
                    _indexHighlighter.SetMap(_fileMap);
                }
            }
            /*

            CppCodeBrowser.IProjectFile fileIndex = result.Index.FindProjectItem(Document.File.Path);

            //highlight diagnostics
            List<LibClang.Diagnostic> diags = new List<Diagnostic>(fileIndex.Diagnostics);
            DiagnosticOutputWriter.UpdateOutput(diags);
             * */
        }

        protected DiagnosticHighlighter DiagnosticHighlighter
        {
            get;
            private set;
        }

        protected SearchHighlighter SearchHighlighter
        {
            get;
            private set;
        }

        protected bool HasIndex
        {
            get
            {
                return Document.Project != null && Document.Project.Index != null;
            }
        }

        protected override void OnSetView(CodeEditor view)
        {
            view.TextArea.TextView.BackgroundRenderers.Add(DiagnosticHighlighter.Renderer);
            view.TextArea.TextView.BackgroundRenderers.Add(SearchHighlighter.Renderer);
            view.TextArea.TextView.BackgroundRenderers.Add(_indexHighlighter.Renderer);
            view.KeyDown += OnViewKeyDown;
            //Underliner.Redraw();
        }

        void OnViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SearchHighlighter.Clear();
            }
        }

        public string Path
        {
            get { return Document.File.Path.Str; }
        }

        public ICommand InspectSymbolCommand
        {
            get { return _inspectSymbolCommand; }
        }

        public ICommand InspectCursorCommand
        {
            get { return _inspectCursorCommand; }
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
