using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.DiagnosticsControl
{
    public class DiagsControlViewModel
    {
        public DiagsControlViewModel()
        {
            JadeCore.Services.Provider.EditorController.ActiveDocumentChanged += EditorControllerActiveDocumentChanged;
        }

        private void EditorControllerActiveDocumentChanged(JadeCore.IEditorDoc newValue, JadeCore.IEditorDoc oldValue)
        {
            ClearDiags();
//            if(newValue is JadeCore.Editor.ISourceDocument)
            {

            }
        }

        private void ClearDiags()
        {

        }

        public ObservableCollection<DiagViewMode> Diagnostics
        {
            get { return null; }
        }
    }
}
