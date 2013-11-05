using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Project
{
    public enum ItemType
    {
        Unknown,
        CppSourceFile,
        CppHeaderFile
    }

    public interface IItem
    {
        string Name { get; }
        ItemType Type { get; }
    }
}
