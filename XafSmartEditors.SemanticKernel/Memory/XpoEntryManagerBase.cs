using System;
using System.Linq;

namespace XafSmartEditors.SemanticKernel.Memory
{
    public class XpoEntryManagerBase : IXpoEntryManager
    {

        protected virtual void OnObjectCreated(EventArgs e)
        {
            ObjectCreatedEvent?.Invoke(this, e);

        }
        public event EventHandler ObjectCreatedEvent;

        public virtual T CreateObject<T>()
        {
            throw new NotImplementedException();
        }
        public virtual IQueryable<T> GetQuery<T>(bool inTransaction = true)
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
