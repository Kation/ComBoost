using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    public interface IEntityUpdateModel
    {
        bool IsSuccess { get; }

        Dictionary<IPropertyMetadata, string> ErrorMessage { get; }

        object Result { get; set; }
    }
}
