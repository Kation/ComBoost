using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Metadata;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity base object.
    /// </summary>
    public abstract class EntityBase : IEntity
    {
        /// <summary>
        /// Get or set the id of entity.
        /// </summary>
        [Key]
        [Required]
        [Hide]
        public virtual Guid Index { get; set; }

        /// <summary>
        /// Get or set the create date of entity.
        /// </summary>
        [Required]
        [Hide]
        [Column(TypeName = "Datetime2")]
        public virtual DateTime CreateDate { get; set; }

        /// <summary>
        /// Call when entity created.
        /// </summary>
        public virtual void OnCreateCompleted()
        {

        }

        /// <summary>
        /// Call when entity edited.
        /// </summary>
        public virtual void OnEditCompleted()
        {

        }

        /// <summary>
        /// Get is the entity can remove.
        /// </summary>
        /// <returns></returns>
        [Hide]
        [NotMapped]
        public virtual bool IsRemoveAllowed
        {
            get { return true; }
        }

        /// <summary>
        /// Get is the entity can edit.
        /// </summary>
        /// <returns></returns>
        [Hide]
        [NotMapped]
        public virtual bool IsEditAllowed
        {
            get { return true; }
        }

        /// <summary>
        /// Return a string that entity display.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var metadata = EntityAnalyzer.GetMetadata(GetType());
            if (metadata.DisplayProperty == null)
                return base.ToString();
            object value = metadata.DisplayProperty.GetValue(this);
            if (value == null)
                return "";
            else
                return value.ToString();
        }

        //private ReadOnlyCollection<ValidationResult> _NoError = new ReadOnlyCollection<ValidationResult>(new List<ValidationResult>());
        /// <summary>
        /// Ensure that entity is valid.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>Collection that include error messages.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var metadata = EntityAnalyzer.GetMetadata(GetType());
            var result = new List<ValidationResult>();
            foreach (var property in metadata.Properties)
            {
                validationContext.MemberName = property.ClrName;
                validationContext.DisplayName = property.Name;
                var list = property.GetAttributes<ValidationAttribute>();
                foreach (var item in list)
                {
                    var r = item.GetValidationResult(property.GetValue(this), validationContext);
                    if (r != null && r != ValidationResult.Success)
                        result.Add(r);
                }
            }
            return result;
        }
    }
}
