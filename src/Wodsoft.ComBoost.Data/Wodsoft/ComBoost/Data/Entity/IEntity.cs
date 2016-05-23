using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntity
    {
        object Index { get; set; }

        DateTime CreateDate { get; set; }

        void OnPreCreate();

        void OnCreateCompleted();

        void OnPreEdit();

        void OnEditCompleted();

        bool IsRemoveAllowed { get; }

        bool IsEditAllowed { get; }
    }
}
