using DevExpress.ExpressApp;
using System;
using System.Linq;

namespace XafSmartEditors.SemanticKernel.Memory
{
    public class XafEntryManager : XpoEntryManagerBase, IXpoEntryManager
    {
        IObjectSpace objectSpace;
        public XafEntryManager(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
        }

        public override IQueryable<T> GetQuery<T>(bool inTransaction = true)
        {
            return objectSpace.GetObjectsQuery<T>(inTransaction);
        }
        public override T CreateObject<T>()
        {
            var Instance = objectSpace.CreateObject<T>();


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
