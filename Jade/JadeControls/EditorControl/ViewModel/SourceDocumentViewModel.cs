using System;
using System.Collections.Generic;
using System.Windows.Input;
using JadeCore;
using JadeUtils.IO;
using CppCodeBrowser;

namespace JadeControls.EditorControl.ViewModel
{
    public class SourceDocumentViewModel : DocumentViewModel
    {
        public ICodeBrowser BrowseStrategy { get; private set; }

        public SourceDocumentViewModel(IEditorDoc doc, ICodeBrowser browseStrategy) 
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
            ICodeLocation location = new CodeLocation(Document.File.Path.Str, loc.Offset);

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
