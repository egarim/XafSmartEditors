using System;
using System.Linq;

namespace XafSmartEditors.SemanticKernel.Memory
{
    public class EntryCreatedArgs : EventArgs
    {

        public object instance;
        public EntryCreatedArgs(object instance)
        {

            this.instance = instance;
        }
    }
}
