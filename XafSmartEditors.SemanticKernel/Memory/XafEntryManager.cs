﻿using DevExpress.ExpressApp;
using Microsoft.SemanticKernel.Connectors.Xpo;
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

        public override IQueryable<XpoDatabaseEntry> GetQuery(bool inTransaction = true)
        {
            return objectSpace.GetObjectsQuery<XpoDatabaseEntry>(inTransaction);
        }
        public override XpoDatabaseEntry CreateObject()
        {
            var Instance = objectSpace.CreateObject<XpoDatabaseEntry>();


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
