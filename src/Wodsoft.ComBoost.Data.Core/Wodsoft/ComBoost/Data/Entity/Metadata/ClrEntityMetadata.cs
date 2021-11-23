﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// Entity metadata.
    /// </summary>
    public class ClrEntityMetadata : EntityMetadataBase
    {
        /// <summary>
        /// Initialize entity metadata.
        /// </summary>
        /// <param name="type"></param>
        public ClrEntityMetadata(Type type)
            : base(type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            SetProperties(type.GetProperties().Select(t => new ClrPropertyMetadata(t)).OrderBy(t => t.Order));
            SetMetadata();
        }
    }
}
