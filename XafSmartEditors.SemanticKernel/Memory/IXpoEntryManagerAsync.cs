using System;
using System.Linq;

namespace XafSmartEditors.SemanticKernel.Memory
{
    public interface IXpoEntryManagerAsync : IXpoEntryManager
    {
        Task CommitAsync();
    }
}
