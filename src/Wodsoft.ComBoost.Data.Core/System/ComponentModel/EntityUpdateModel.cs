using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    public class EntityUpdateModel<T> : IEntityUpdateModel<T>
        where T : IEntity
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

        public T Result { get; set; }

        object IEntityUpdateModel.Result { get { return Result; } }
    }
}
