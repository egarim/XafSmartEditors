using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XafSmartEditors.SemanticKernel.Memory;

namespace XafSmartEditors.Module.BusinessObjects
{
    public class XafSmartEditorsEntryManager : XpoEntryManagerBase, IXpoEntryManager
    {
        IObjectSpace objectSpace;
        public XafSmartEditorsEntryManager(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
           
        }

        public override IQueryable<MemoryEntry> GetQuery(bool inTransaction = true)
        {
            return objectSpace.GetObjectsQuery<MemoryEntry>(inTransaction);
        }
        public override MemoryEntry CreateObject()
        {
            var Instance = objectSpace.CreateObject<MemoryEntry>();


            OnObjectCreated(new EntryCreatedArgs(Instance));
            return Instance;
        }
        public override void Commit()
        {
            objectSpace.CommitChanges();
        }
        public override void Delete(object Instance)
        {
            objectSpace.Delete(Instance);

        }
        public override void Dispose()
        {
            objectSpace.Dispose();
        }

    }
}
