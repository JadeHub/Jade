using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Workspace.Parser
{
    public interface IParseTaskQueue
    {
        void QueueProject(ProjectTaskQueue proj);
        void Prioritise(ProjectTaskQueue proj);
        void RemoveProject(ProjectTaskQueue proj);
        FileIndexerTask DequeueTask();
        WaitHandle WaitHandle { get; }
    }
}
