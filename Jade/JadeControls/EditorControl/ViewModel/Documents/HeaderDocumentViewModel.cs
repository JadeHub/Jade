using JadeCore;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    public class HeaderDocumentViewModel : CodeDocumentViewModelBase
    {
        public HeaderDocumentViewModel(IEditorDoc doc) 
            : base(doc)
        {
           // Debug.Assert(doc is JadeCore.Editor.SourceDocument);
        }

        public override void RegisterCommands(CommandBindingCollection commandBindings)
        {
        }
    }
}
