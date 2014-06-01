using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class InspectSymbolCommand : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.IProjectIndex _index;
    
        public InspectSymbolCommand(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
            : base(vm)
        {
            _path = path;
            _index = index;
        }

        protected override bool CanExecute()
        {
            return true;
        }

        protected override void Execute()
        {
        }
    }
}
