using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    public class EntityUpdateModel : IEntityUpdateModel
    {
        public EntityUpdateModel()
        {
            _ErrorMessage = new Dictionary<IPropertyMetadata, string>();
        }

        private Dictionary<IPropertyMetadata, string> _ErrorMessage;
        public Dictionary<IPropertyMetadata, string> ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
        }

        public bool IsSuccess { get; set; }

        public object Result { get; set; }
    }
}
