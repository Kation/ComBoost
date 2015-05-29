using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class ServiceEntityQueryable<TEntity> : IEntityQueryable<TEntity> where TEntity : class, IEntity, new()
    {
        private IEntityService<TEntity> _Service;
        private ClrEntityMetadata Metadata;

        public ServiceEntityQueryable(IEntityService<TEntity> service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            _Service = service;
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
        }

        public bool Add(TEntity entity)
        {
            return _Service.Add(entity);
        }

        public Task<bool> AddAsync(TEntity entity)
        {
            return _Service.AddAsync(entity);
        }

        public bool AddRange(IEnumerable<TEntity> entities)
        {
            return _Service.AddRange(entities.ToArray());
        }

        public Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return _Service.AddRangeAsync(entities.ToArray());
        }

        public TEntity Create()
        {
            return _Service.Create();
        }

        public bool Remove(Guid id)
        {
            return _Service.Remove(id);
        }

        public Task<bool> RemoveAsync(Guid id)
        {
            return _Service.RemoveAsync(id);
        }

        public bool RemoveRange(IEnumerable<Guid> ids)
        {
            return _Service.RemoveRange(ids.ToArray());
        }

        public Task<bool> RemoveRangeAsync(IEnumerable<Guid> ids)
        {
            return _Service.RemoveRangeAsync(ids.ToArray());
        }

        public bool Edit(TEntity entity)
        {
            return _Service.Edit(entity);
        }

        public Task<bool> EditAsync(TEntity entity)
        {
            return _Service.EditAsync(entity);
        }

        public TEntity GetEntity(Guid id)
        {
            return _Service.GetEntity(id);
        }

        public Task<TEntity> GetEntityAsync(Guid id)
        {
            return _Service.GetEntityAsync(id);
        }

        public IQueryable<TEntity> Query()
        {
            ServiceEntityQueryProvider<TEntity> provider = new ServiceEntityQueryProvider<TEntity>(_Service);
            return new ServiceEntityQuery<TEntity>(provider, Expression.Empty());
        }

        public IEnumerable<TEntity> Query(string sql, params object[] parameters)
        {
            return _Service.QuerySql(sql, parameters);
        }

        public IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, Guid[] parents)
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

        public IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, string path, Guid id)
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

        public bool Editable()
        {
            return _Service.Editable();
        }

        public bool Addable()
        {
            return _Service.Addable();
        }

        public bool Removeable()
        {
            return _Service.Removeable();
        }

        public int Count()
        {
            return _Service.Count();
        }

        public Task<int> CountAsync()
        {
            return _Service.CountAsync();
        }

        public bool Contains(TEntity entity)
        {
            return _Service.Contains(entity.Index);
        }

        public Task<bool> ContainsAsync(TEntity entity)
        {
            return _Service.ContainsAsync(entity.Index);
        }

        public bool Contains(Guid id)
        {
            return _Service.Contains(id);
        }

        public Task<bool> ContainsAsync(Guid id)
        {
            return _Service.ContainsAsync(id);
        }

        public IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> queryable)
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

        public IOrderedQueryable<TEntity> OrderBy()
        {
            return OrderBy(Query());
        }

        public Task<TEntity[]> ToArrayAsync(IQueryable<TEntity> queryable)
        {
            return new Task<TEntity[]>(() =>
            {
                return queryable.ToArray();
            });
        }

        public Task<List<TEntity>> ToListAsync(IQueryable<TEntity> queryable)
        {
            return new Task<List<TEntity>>(() =>
            {
                return queryable.ToList();
            });
        }
    }
}
