using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public class EntityAPIController<TEntity> : EntityAPIController where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Metadata of entity.
        /// </summary>
        public EntityMetadata Metadata { get; private set; }

        /// <summary>
        /// Get the queryable of entity.
        /// </summary>
        public IEntityQueryable<TEntity> EntityQueryable { get; private set; }

        /// <summary>
        /// Initialize entity controller.
        /// </summary>
        /// <param name="builder">Context builder of entity.</param>
        public EntityAPIController(IEntityContextBuilder builder)
            : base(builder)
        {
            EntityQueryable = builder.GetContext<TEntity>();
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
        }

        public virtual async Task<bool> Authenticate(string data)
        {
            return false;
        }

        public virtual async Task<bool> IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        public virtual async Task GetEntity(Guid id)
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.ViewRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                Response.End();
                return;
            }
            TEntity entity = await EntityQueryable.GetEntityAsync(id);
            if (entity == null)
            {
                Response.StatusCode = 404;
                Response.End();
                return;
            }

            var serializer = EntitySerializer.GetFormatter(EntityBuilder.DescriptorContext);
            serializer.Serialize(Response.OutputStream, entity);
        }

        public virtual async Task<bool> Add()
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.AddRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                return false;
            }

            var serializer = EntitySerializer.GetFormatter(EntityBuilder.DescriptorContext);
            TEntity entity = (TEntity)serializer.Deserialize(Request.InputStream);
            return await EntityQueryable.AddAsync(entity);
        }

        public virtual async Task<bool> Edit()
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.EditRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                return false;
            }

            //Todo
            //Deserialize entity and edit it.
            var serializer = EntitySerializer.GetFormatter(EntityBuilder.DescriptorContext);
            TEntity entity = (TEntity)serializer.Deserialize(Request.InputStream);
            return await EntityQueryable.EditAsync(entity);
        }

        public virtual async Task<bool> Remove(Guid id)
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.RemoveRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                return false;
            }
            if (!await EntityQueryable.RemoveAsync(id))
            {
                Response.StatusCode = 404;
                return false;
            }
            return true;
        }

        public virtual async Task Query()
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.ViewRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                Response.End();
                return;
            }

            ExpressionSerializer serializer = new ExpressionSerializer();
            dynamic queryable = serializer.Deserialize(Request.InputStream, ((IQueryable)EntityQueryable.Query()).Provider);
            dynamic result = Enumerable.ToArray(queryable);// queryable.Select(t => t.Index).ToArray();
            EntitySerializer.GetFormatter(EntityBuilder.DescriptorContext).Serialize(Response.OutputStream, result);
        }

        public virtual async Task QuerySingle(HttpPostedFileBase expression)
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.ViewRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                Response.End();
                return;
            }
            ExpressionSerializer serializer = new ExpressionSerializer();
            object result = serializer.DeserializeSingle(Request.InputStream, ((IQueryable)EntityQueryable.Query()).Provider);
            EntitySerializer.GetFormatter(EntityBuilder.DescriptorContext).Serialize(Response.OutputStream, result);
        }

        public virtual async Task QuerySql(string sql)
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.ViewRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                Response.End();
                return;
            }
            //EntityBuilder.Query<TEntity>(sql,)
            throw new NotImplementedException();
        }

        public virtual async Task<int> Count()
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.ViewRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                return -1;
            }
            return await EntityQueryable.CountAsync();
        }

        public virtual async Task<bool> Contains(Guid id)
        {
            if ((!Metadata.AllowAnonymous && !await IsAuthenticated()) || !Metadata.ViewRoles.All(t => User.IsInRole(t)))
            {
                Response.StatusCode = 401;
                return false;
            }
            return await EntityQueryable.ContainsAsync(id);
        }

        public virtual async Task<bool> Editable()
        {
            if (!Metadata.AllowAnonymous && !await IsAuthenticated())
                return false;
            return Metadata.EditRoles.All(t => User.IsInRole(t));
        }

        public virtual async Task<bool> Addable()
        {
            if (!Metadata.AllowAnonymous && !await IsAuthenticated())
                return false;
            return Metadata.AddRoles.All(t => User.IsInRole(t));
        }

        public virtual async Task<bool> Removeable()
        {
            if (!Metadata.AllowAnonymous && !await IsAuthenticated())
                return false;
            return Metadata.RemoveRoles.All(t => User.IsInRole(t));
        }

        public virtual async Task<bool> Readable()
        {
            if (!Metadata.AllowAnonymous && !await IsAuthenticated())
                return false;
            return Metadata.ViewRoles.All(t => User.IsInRole(t));
        }
    }
}
