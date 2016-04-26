using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityViewer<TEntity> : EntityViewer
        where TEntity : class, IEntity, new()
    {

    }
}
