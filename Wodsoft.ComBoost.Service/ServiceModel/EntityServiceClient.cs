using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class EntityServiceClient<TEntity> : ClientBase<IEntityService<TEntity>>, IEntityQueryable<TEntity> where TEntity : class, IEntity, new()
    {
        protected EntityServiceClient()
        {
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
        }

        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        protected EntityMetadata Metadata { get; private set; }

        public virtual bool Add(TEntity entity)
        {
            return Channel.Add(entity);
        }

        public virtual bool AddRange(IEnumerable<TEntity> entities)
        {
            return Channel.AddRange(entities.ToArray());
        }

        public virtual TEntity Create()
        {
            return new TEntity();
        }

        public virtual bool Remove(Guid id)
        {
            return Channel.Remove(id);
        }

        public virtual bool RemoveRange(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
                Channel.Remove(id);
            return true;
        }

        public virtual bool Edit(TEntity entity)
        {
            return Channel.Edit(entity);
        }

        public virtual TEntity GetEntity(Guid id)
        {
            return Channel.GetEntity(id);
        }

        public virtual IQueryable<TEntity> Query()
        {
            EntityServiceQueryableProvider<TEntity> provider = new EntityServiceQueryableProvider<TEntity>(Channel);
            return provider.CreateQuery<TEntity>(Expression.Empty());
        }

        public virtual IEnumerable<TEntity> Query(string sql, params object[] parameters)
        {
            SqlParameter[] p = new SqlParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                p[i] = new SqlParameter("@" + i, parameters[i]);
            return Channel.QuerySql(sql, p);
        }

        public virtual IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, Guid[] parents)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (parents == null || parents.Length == 0)
                throw new ArgumentNullException("parents");
            if (Metadata.ParentProperty == null)
                throw new NotSupportedException("Entity doesn't contains parent property.");
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            Expression equal = null;
            foreach (object parent in parents)
            {
                var item = Expression.Equal(Expression.Property(Expression.Property(parameter, Metadata.ParentProperty.Property), typeof(TEntity).GetProperty("Index")), Expression.Constant(parent));
                if (equal == null)
                    equal = item;
                else
                    equal = Expression.Or(equal, item);
            }
            var express = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
            return queryable.Where(express);
        }

        public virtual IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, string path, Guid id)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (path == null)
                throw new ArgumentNullException("path");
            if (Metadata.ParentProperty == null)
                throw new NotSupportedException("Entity doesn't contains parent property.");
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            MemberExpression member = null;
            string[] properties = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Type type = Metadata.Type;
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = type.GetProperty(properties[i]);
                if (property == null)
                    throw new ArgumentException("Parent path invalid.");
                if (member == null)
                    member = Expression.Property(parameter, property);
                else
                    member = Expression.Property(member, property);
                type = property.PropertyType;
            }
            Expression equal = Expression.Equal(Expression.Property(member, type.GetProperty("Index")), Expression.Constant(id));
            var express = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
            return queryable.Where(express);
        }

        public virtual bool Editable() { return true; }

        public virtual bool Addable() { return true; }

        public virtual bool Removeable() { return true; }

        public virtual int Count()
        {
            return Channel.Count();
        }

        public virtual bool Contains(TEntity entity)
        {
            return Channel.Contains(entity.Index);
        }

        public virtual bool Contains(Guid id)
        {
            return Channel.Contains(id);
        }

        public virtual IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> queryable)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (Metadata.SortProperty == null)
                return queryable.OrderByDescending(t => t.CreateDate);
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            var express = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TEntity), Metadata.SortProperty.Property.PropertyType), Expression.Property(parameter, Metadata.SortProperty.Property), parameter);
            if (Metadata.SortDescending)
            {
                var method = typeof(Queryable).GetMethods().Where(t => t.Name == "OrderByDescending").ElementAt(0).MakeGenericMethod(typeof(TEntity), Metadata.SortProperty.Property.PropertyType);
                return (IOrderedQueryable<TEntity>)method.Invoke(null, new object[] { queryable, express });
            }
            else
            {
                var method = typeof(Queryable).GetMethods().Where(t => t.Name == "OrderBy").ElementAt(0).MakeGenericMethod(typeof(TEntity), Metadata.SortProperty.Property.PropertyType);
                return (IOrderedQueryable<TEntity>)method.Invoke(null, new object[] { queryable, express });
            }
        }

        public virtual IOrderedQueryable<TEntity> OrderBy()
        {
            return OrderBy(Query());
        }
    }
}
