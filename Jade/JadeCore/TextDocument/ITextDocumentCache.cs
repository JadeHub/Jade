using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore
{
    public interface ITextDocumentCache
    {
        ITextDocument FindOrAdd(IFileHandle file);
        ITextDocument Find(IFileHandle file);
        void Reset();
    }
}
