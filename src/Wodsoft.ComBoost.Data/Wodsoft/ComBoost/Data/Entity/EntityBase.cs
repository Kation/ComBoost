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

        public DateTime EditDate { get; set; }

        [Key]
        public Guid Index { get; set; }

        object IEntity.Index { get { return Index; } set { Index = (Guid)value; } }

        public virtual bool IsEditAllowed { get { return true; } }

        public virtual bool IsRemoveAllowed { get { return true; } }

        public virtual void OnCreateCompleted()
        {
            Index = Guid.NewGuid();
            CreateDate = DateTime.Now;
            EditDate = CreateDate;
        }

        public virtual void OnEditCompleted()
        {
            EditDate = DateTime.Now;
        }

        public virtual void OnCreating() { }

        public virtual void OnEditing() { }

        IEntityQueryContext<IEntity> IEntity.EntityContext { get; set; }

        bool IEntity.IsNewCreated { get { return Index == Guid.Empty; } }
    }
}
