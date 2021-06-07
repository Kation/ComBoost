using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainFilterOptions
    {
        private List<IDomainServiceFilter> _filters = new List<IDomainServiceFilter>();

        public DomainFilterOptions()
        {
            Filters = new ReadOnlyCollection<IDomainServiceFilter>(_filters);
        }

        public IReadOnlyList<IDomainServiceFilter> Filters { get; } 

        public void Add(IDomainServiceFilter filter)
        {
            _filters.Add(filter);
        }
    }

    public class DomainFilterOptions<T> : DomainFilterOptions
        where T : class, IDomainService
    {

    }
}
