﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
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

        internal CodeDocumentViewModelBase(IEditorDoc doc)
            : base(doc)
        {
            DiagnosticHighlighter = new DiagnosticHighlighter(new Highlighting.Highlighter(TextDocument));
            SearchHighlighter = new SearchHighlighter(Document.File.Path, new Highlighting.Highlighter(TextDocument));

            if(HasIndex)
            {
                DiagnosticHighlighter.ProjectItem = Index.FindProjectItem(Document.File.Path);
                Index.ItemUpdated += ProjectIndexItemUpdated;

                _inspectSymbolCommand = new Commands.InspectSymbolCommand(this, doc.File.Path, doc.Project.Index);
            }
        }

        private void ProjectIndexItemUpdated(JadeUtils.IO.FilePath path)
        {
            if (path != Document.File.Path) return;
            CppCodeBrowser.IProjectFile fileIndex = Index.FindProjectItem(Document.File.Path);
            DiagnosticHighlighter.ProjectItem = fileIndex;
            List<LibClang.Diagnostic> diags = new List<Diagnostic>(fileIndex.Diagnostics);
            DiagnosticOutputWriter.UpdateOutput(diags);
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

        protected CppCodeBrowser.IProjectIndex Index
        {
            get
            {
                Debug.Assert(HasIndex);
                return Document.Project.Index;
            }
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
    }
}
