﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Workspace
{
    public interface IWorkspace : IFolder
    {
        string Path { get; set; }
        string Directory { get; }
        JadeCore.Project.IProject ActiveProject { get; }
        ITextDocumentCache DocumentCache { get; }
    }
}
