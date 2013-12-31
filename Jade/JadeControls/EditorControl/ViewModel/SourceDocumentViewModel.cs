using System;
using System.Collections.Generic;
using System.Windows.Input;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class SourceDocumentViewModel : DocumentViewModel
    {
        public ISourceBrowserStrategy BrowseStrategy { get; private set; }

        public SourceDocumentViewModel(IEditorDoc doc, ISourceBrowserStrategy browseStrategy) 
            : base(doc)
        {
            BrowseStrategy = browseStrategy;
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
            CppView.ICodeLocation location = new CppView.CodeLocation(loc.Line, loc.Column, loc.Offset, Document.File.Path);

            location = BrowseStrategy.JumpTo(location);
            if (location != null)
            {
                JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
                cmdHandler.OnDisplayCodeLocation(location);
            }
        }

        private bool CanJumpTo(JadeCore.Editor.CodeLocation loc)
        {
            return true;
        }
    }
}
