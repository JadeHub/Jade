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
        /// <summary>
        /// Item's name. Must be unique across the owning Project.
        /// For files the path is used
        /// </summary>
        string ItemName { get; }
        ItemType Type { get; }
    }
}
