using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class SourceDocumentViewModel : DocumentViewModel
    {
        private CppCodeBrowser.ICodeBrowser _jumpToBrowser;
        
        public SourceDocumentViewModel(IEditorDoc doc, CppCodeBrowser.IProjectIndex index) 
            : base(doc)
        {
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(index);
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
    }
}
