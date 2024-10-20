using DevExpress.ExpressApp;
using Microsoft.SemanticKernel.Connectors.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XafSmartEditors.SemanticKernel.Memory
{
    public interface IXpoEntryManager
    {
        IXpoMemoryEntry CreateObject();
        public event EventHandler<EntryCreatedArgs> ObjectCreatedEvent;
        void Commit();
        IQueryable<IXpoMemoryEntry> GetQuery(bool inTransaction = true);
        void Delete(object Instance);
        void Dispose();

    }
}
