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
            _errorMessage = new List<KeyValuePair<string, string>>();
        }

        public bool IsSuccess { get; set; }

        private List<KeyValuePair<string, string>> _errorMessage;
        public IList<KeyValuePair<string, string>> ErrorMessage => _errorMessage;
    }

    public class UpdateModel<T> : UpdateModel, IUpdateModel<T>
    {
        public UpdateModel(T value)
        {
            Item = value;
        }

        public UpdateModel() { }

        public T? Item { get; set; }
    }
}
