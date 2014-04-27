using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace System.Data.Entity
{
    public class CacheLocalEntityQueryable<TEntity> : ICacheManager<TEntity> where TEntity : CacheEntityBase, new()
    {
        private string TableName;
        private ICacheEntityQueryable<TEntity> EntityContext;
        private DbSet<TEntity> DbSet;
        private DbContext DbContext;

        public CacheLocalEntityQueryable(ICacheEntityQueryable<TEntity> remoteEntityContext, DbContext localDbContext)
        {
            EntityContext = remoteEntityContext;
            DbContext = localDbContext;
            DbSet = localDbContext.Set<TEntity>();
            var tableAtt = (System.ComponentModel.DataAnnotations.Schema.TableAttribute)typeof(TEntity).GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), true).LastOrDefault();
            if (tableAtt != null && tableAtt.Name != null)
            {
                TableName = tableAtt.Name;
            }
            else
            {
                TableName = typeof(TEntity).Name;
                Regex plural1 = new Regex("(?<keep>[^aeiou])y$");
                Regex plural2 = new Regex("(?<keep>[aeiou]y)$");
                Regex plural3 = new Regex("(?<keep>[sxzh])$");
                Regex plural4 = new Regex("(?<keep>[^sxzhy])$");

                if (plural1.IsMatch(TableName))
                    TableName = plural1.Replace(TableName, "${keep}ies");
                else if (plural2.IsMatch(TableName))
                    TableName = plural2.Replace(TableName, "${keep}s");
                else if (plural3.IsMatch(TableName))
                    TableName = plural3.Replace(TableName, "${keep}es");
                else if (plural4.IsMatch(TableName))
                    TableName = plural4.Replace(TableName, "${keep}s");

            }     
        }

        public virtual bool IsCached(Guid entityID)
        {
            return DbSet.Count(t => t.Index == entityID) > 0;
        }

        public virtual void Clear()
        {
            DbContext.Database.ExecuteSqlCommand("DELETE * FROM " + TableName);
        }

        public virtual void Refresh()
        {
            Guid[] downlist;
            if (DbSet.Count() == 0)
                downlist = EntityContext.GetKeys(DateTime.MinValue);
            else
                downlist = EntityContext.GetKeys(DbSet.Max(t => t.UpdateTime));
            foreach (var index in downlist)
            {
                if (IsCached(index))
                    DbContext.Entry<TEntity>(EntityContext.GetEntity(index)).State = EntityState.Modified;
                else
                    DbSet.Add(EntityContext.GetEntity(index));
            }
            var removelist = DbSet.Select(t => t.Index).ToList().Except(GetKeys());
            foreach (var index in removelist)
                DbContext.Database.ExecuteSqlCommand("DELETE FROM " + TableName + " WHERE [BaseIndex] = {0}", index);
            DbContext.SaveChanges();
        }

        public virtual int CachedCount
        {
            get { return DbSet.Count(); }
        }

        public virtual Guid? Add(TEntity entity)
        {
            if (EntityContext.Add(entity) == null)
                return null;
            if (IsCached(entity.Index))
                DbContext.Entry<TEntity>(entity).State = EntityState.Modified;
            else
                DbSet.Add(entity);
            DbContext.SaveChanges();
            return entity.Index;
        }

        public virtual bool Remove(Guid entityID)
        {
            if (!EntityContext.Remove(entityID))
                return false;
            DbContext.Database.ExecuteSqlCommand("DELETE * FROM " + TableName + " WHERE [BaseIndex] = {0}", entityID);
            return true;
        }

        public virtual bool Edit(TEntity entity)
        {
            if (!EntityContext.Edit(entity))
                return false;
            if (IsCached(entity.Index))
                DbContext.Entry<TEntity>(entity).State = EntityState.Modified;
            else
                DbSet.Add(entity);
            DbContext.SaveChanges();
            return true;
        }

        public virtual TEntity GetEntity(Guid entityID)
        {
            TEntity item = DbSet.Find(entityID);
            if (item != default(TEntity))
                if (EntityContext.Contains(item.Index))
                    return item;
                else
                {
                    DbSet.Remove(item);
                    DbContext.SaveChanges();
                    return default(TEntity);
                }
            item = EntityContext.GetEntity(entityID);
            if (item == default(TEntity))
                return default(TEntity);
            DbSet.Add(item);
            DbContext.SaveChanges();
            return item;
        }

        public virtual Guid[] GetKeys()
        {
            return EntityContext.GetKeys();
        }

        public virtual IQueryable<TEntity> Query()
        {
            return DbSet;
        }

        public virtual Guid[] InParent(Guid[] parents)
        {
            return EntityContext.InParent(parents);
        }

        public virtual Guid[] Search(string content)
        {
            return EntityContext.Search(content);
        }

        public virtual Guid[] SearchInParent(string content, Guid[] parents)
        {
            return EntityContext.SearchInParent(content, parents);
        }

        public virtual bool Editable()
        {
            return EntityContext.Editable();
        }

        public virtual bool Addable()
        {
            return EntityContext.Addable();
        }

        public virtual bool Removeable()
        {
            return EntityContext.Removeable();
        }

        public virtual string[] EditableProperties()
        {
            return EntityContext.EditableProperties();
        }

        public virtual string[] VisableProperties()
        {
            return EntityContext.VisableProperties();
        }

        public virtual int Count()
        {
            return EntityContext.Count();
        }
        
        public Guid[] Where(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return EntityContext.Where(predicate);
        }


        public bool Contains(TEntity entity)
        {
            return Contains(entity.Index);
        }

        public bool Contains(Guid id)
        {
            return EntityContext.Contains(id);
        }
        
        public TEntity Create()
        {
            return DbSet.Create();
        }
    }
}
