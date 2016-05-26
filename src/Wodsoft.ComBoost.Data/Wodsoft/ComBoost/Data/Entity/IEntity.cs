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

        void OnCreating();

        void OnCreateCompleted();

        void OnEditing();

        void OnEditCompleted();

        bool IsRemoveAllowed { get; }

        bool IsEditAllowed { get; }
    }
}
