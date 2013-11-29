using System;
using System.Collections.Generic;
using System.Windows.Input;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{    
    public class SourceDocumentViewModel : DocumentViewModel
    {
        public SourceDocumentViewModel(IEditorDoc doc, CodeEditor view) : base(doc, view)
        {
            RegisterCommands(view.CommandBindings);
            
        }

        private void RegisterCommands(CommandBindingCollection commandBindings)
        {
            commandBindings.Add(new CommandBinding(EditorCommands.GoToDefinition,
                                        delegate(object target, ExecutedRoutedEventArgs a)
                                        {
                                            OnGoToDefinition(this.CaretLocation);
                                            a.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs a)
                                        {
                                            a.CanExecute = true;
                                            a.Handled = true;
                                        }));
        }

        private void OnGoToDefinition(JadeCore.Editor.CodeLocation loc)
        {
            CppView.IProjectSourceIndex index = GetProjectSourceIndex();
            if(index == null) return;

            LibClang.Cursor c = index.GetCursorAt(Document.File.Path, loc.Offset);

            //CppView.ICodeElement codeElem = index.GetElementAt(Document.File.Path, loc.Offset);

            /*
            CppView.ISourceFile file = index.FileStore.FindSourceFile(Document.File.Path);
            if(file == null) return;

            CppView.ICodeElement codeElem = index.SymbolTable.GetElementAt(file, loc.Offset);
            if (codeElem == null)
                return;

            if (codeElem is CppView.IReference)
            {
                codeElem = (codeElem as CppView.IReference).ReferencedDecl;
            }

            JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
            cmdHandler.OnDisplayCodeLocation(codeElem.Location);
            * */
        }

        private CppView.IProjectSourceIndex GetProjectSourceIndex()
        {
            if (Services.Provider.WorkspaceController.CurrentWorkspace != null &&
                Services.Provider.WorkspaceController.CurrentWorkspace.ActiveProject != null)
            {
                return Services.Provider.WorkspaceController.CurrentWorkspace.ActiveProject.SourceIndex;
            }
            return null;
        }
    }
}
