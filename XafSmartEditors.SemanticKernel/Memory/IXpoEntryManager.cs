using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XafSmartEditors.SemanticKernel.Memory
{
    public interface IXpoEntryManager
    {
        T CreateObject<T>();
        public event EventHandler ObjectCreatedEvent;
        void Commit();
        IQueryable<T> GetQuery<T>(bool inTransaction = true);
        void Delete(object Instance);
        void Dispose();

    }
}
