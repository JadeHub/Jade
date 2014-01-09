using JadeCore;
using System.Linq;
using System.Windows.Input;

namespace JadeControls.EditorControl.ViewModel
{
    public class SourceDocumentViewModel : DocumentViewModel
    {
        private CppCodeBrowser.ICodeBrowser _jumpToBrowser;
        private DiagnosticHighlighter _diagnosticHighlighter;
    //    private ASTHighlighter _astHighlighter;
        private CppCodeBrowser.IProjectItem _fileBrowser;

        public SourceDocumentViewModel(IEditorDoc doc, CppCodeBrowser.IProjectIndex index) 
            : base(doc)
        {
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(index);
            _fileBrowser = index.FindProjectItem(doc.Path.Str);
            
        }

        public override void RegisterCommands(CommandBindingCollection commandBindings)
        {
            commandBindings.Add(new CommandBinding(EditorCommands.JumpTo,
                                        delegate(object target, ExecutedRoutedEventArgs a)
                                        {
                                            OnJumpTo(this.CaretLocation);
                                            a.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs a)
                                        {
                                            a.CanExecute = CanJumpTo(this.CaretLocation);
                                            a.Handled = true;
                                        }));
        }

        private void OnJumpTo(JadeCore.Editor.CodeLocation loc)
        {
            var results = _jumpToBrowser.BrowseFrom(new CppCodeBrowser.CodeLocation(Document.File.Path.Str, loc.Offset));

            if (results != null && results.Count() > 0)
            {
                JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
                cmdHandler.OnDisplayCodeLocation(results.First());
            }
        }

        private bool CanJumpTo(JadeCore.Editor.CodeLocation loc)
        {
            return true;
        }

        protected override void OnSetView(CodeEditor view)
        {
            Highlighting.Underliner underliner = new Highlighting.Underliner(TextDocument);
            view.TextArea.TextView.BackgroundRenderers.Add(underliner);

            if (_fileBrowser != null)            
            {
                //_astHighlighter = new ASTHighlighter(_fileBrowser.TranslationUnits.First().Cursor, underliner, _fileBrowser.Path);
                _diagnosticHighlighter = new DiagnosticHighlighter(_fileBrowser, underliner);
            }            
        }
    }
}
