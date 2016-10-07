using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityBase : IEntity
    {
        [Hide]
        public virtual DateTime CreateDate { get; set; }

        [Hide]
        public virtual DateTime EditDate { get; set; }

        [Hide]
        [Key]
        public virtual Guid Index { get; set; }

        object IEntity.Index { get { return Index; } set { Index = (Guid)value; } }

        [Hide]
        public virtual bool IsEditAllowed { get { return true; } }

        [Hide]
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

        public override string ToString()
        {
            var metadata = EntityDescriptor.GetMetadata(GetType());
            if (metadata.DisplayProperty == null)
                return base.ToString();
            object value = metadata.DisplayProperty.GetValue(this);
            if (value == null)
                return "";
            else
                return value.ToString();
        }
    }
}
