using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NqlDotNet
{
    public class CriteriaResult
    {
        public string RootEntity { get; set; }
        public string Criteria { get; set; }

        public string CriteriaDescription { get; set; }
    }
}
