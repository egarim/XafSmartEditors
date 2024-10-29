using System;
using System.Linq;

namespace NqlDotNet
{
    public interface INqlService
    {
        Task<string> CriteriaToNl(string Criteria, string Schema, string Doc);
        Task<CriteriaResult> NlToCriteria(string Nlq, string Schema, string Doc);
    }
}
