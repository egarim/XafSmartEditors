using Microsoft.SemanticKernel.Connectors.Xpo;
using System;
using System.Linq;

namespace XafSmartEditors.SemanticKernel.Memory
{
    public class XpoEntryManagerBase : IXpoEntryManager
    {

        protected virtual void OnObjectCreated(EntryCreatedArgs e)
        {
            ObjectCreatedEvent?.Invoke(this, e);

        }
        public event EventHandler<EntryCreatedArgs> ObjectCreatedEvent;

        public virtual IXpoMemoryEntry CreateObject()
        {
            throw new NotImplementedException();
        }
        public virtual IQueryable<IXpoMemoryEntry> GetQuery(bool inTransaction = true)
        {
            throw new NotImplementedException();
        }

        public virtual void Commit()
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(object Instance)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
