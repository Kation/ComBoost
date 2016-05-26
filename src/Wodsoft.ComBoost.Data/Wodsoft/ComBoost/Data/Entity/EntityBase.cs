using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityBase : IEntity
    {
        public DateTime CreateDate { get; set; }

        [Key]
        public Guid Index { get; set; }

        object IEntity.Index { get { return Index; } set { Index = (Guid)value; } }

        public virtual bool IsEditAllowed { get { return true; } }

        public virtual bool IsRemoveAllowed { get { return true; } }

        public virtual void OnCreateCompleted() { }

        public virtual void OnEditCompleted() { }

        public virtual void OnCreating()
        {
            Index = Guid.NewGuid();
            CreateDate = DateTime.Now;
        }

        public virtual void OnEditing() { }
    }
}
