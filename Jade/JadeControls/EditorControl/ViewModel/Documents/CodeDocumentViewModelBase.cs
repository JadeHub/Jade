using System;
using System.Diagnostics;
using System.Windows.Input;
using JadeCore;
using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    public abstract class CodeDocumentViewModelBase : DocumentViewModel
    {
        internal CodeDocumentViewModelBase(IEditorDoc doc)
            : base(doc)
        {
            DiagnosticHighlighter = new DiagnosticHighlighter(new Highlighting.Highlighter(TextDocument));
            SearchHighlighter = new SearchHighlighter(Document.File.Path, new Highlighting.Highlighter(TextDocument));

            if(HasIndex)
            {
                DiagnosticHighlighter.ProjectItem = Index.FindProjectItem(Document.File.Path);
                Index.ItemUpdated += ProjectIndexItemUpdated;
            }
        }

        private void ProjectIndexItemUpdated(JadeUtils.IO.FilePath path)
        {
            if (path != Document.File.Path) return;
            DiagnosticHighlighter.ProjectItem = Index.FindProjectItem(Document.File.Path);
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
    }
}
