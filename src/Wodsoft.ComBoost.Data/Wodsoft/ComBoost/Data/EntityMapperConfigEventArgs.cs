using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityMapperConfigEventArgs : DomainServiceEventArgs
    {
        public EntityMapperConfigEventArgs(IMapper mapper)
        {
            Mapper = mapper;
        }

        public IMapper Mapper { get; set; }
    }
}
