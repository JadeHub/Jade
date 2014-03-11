using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JadeUtils.IO;

namespace JadeCore
{
    public interface ITextDocument
    {
        string Name { get; }
        IFileHandle File { get; }
        string Content { get; }
    }
}
