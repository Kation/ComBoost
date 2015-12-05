using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf
{
    public interface IEntityController<TEntity>
        where TEntity : class, IEntity, new()
    {
        Task<EntityViewer> GetViewer();
        Task<EntityEditor> GetEditor(Guid id);
        Task<EntityEditor> GetEditor(TEntity entity);
    }
}
