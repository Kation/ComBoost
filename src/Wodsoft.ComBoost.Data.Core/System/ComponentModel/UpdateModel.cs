using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    public class UpdateModel : IUpdateModel
    {
        public UpdateModel()
        {
            _ErrorMessage = new Dictionary<string, string>();
        }

        public bool IsSuccess { get; set; }

        private Dictionary<string, string> _ErrorMessage;
        public IDictionary<string, string> ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
        }
    }

    public class UpdateModel<T> : UpdateModel, IUpdateModel<T>
    {
        public T Result { get; set; }
    }
}
