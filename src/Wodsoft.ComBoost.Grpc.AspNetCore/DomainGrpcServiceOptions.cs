using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcServiceOptions
    {
        private List<Type> _types;

        public DomainGrpcServiceOptions()
        {
            _types = new List<Type>();
            Types = new ReadOnlyCollection<Type>(_types);
        }

        public IReadOnlyList<Type> Types { get; }

        public void AddService<T>()
            where T : IDomainTemplate
        {
            if (!_types.Contains(typeof(T)))
                _types.Add(typeof(T));
        }
    }
}
