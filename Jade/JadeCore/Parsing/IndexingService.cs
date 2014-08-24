using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppCodeBrowser;

namespace JadeCore.Parsing
{
    public class IndexingService
    {
        private IUnsavedFileProvider _unsavedFileProvider;

        public IndexingService(IUnsavedFileProvider unsavedProvider)
        {
            _unsavedFileProvider = unsavedProvider;
        }



    }
}
