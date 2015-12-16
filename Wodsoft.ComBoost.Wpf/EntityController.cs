using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityController<TEntity> : IEntityController<TEntity>
        where TEntity : class, IEntity, new()
    {
        public IEntityContextBuilder EntityBuilder { get; private set; }

        public IEntityQueryable<TEntity> EntityQueryable { get; private set; }

        public IEntityMetadata Metadata { get; private set; }

        public EntityController(IEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            EntityBuilder = builder;
            EntityQueryable = EntityBuilder.GetContext<TEntity>();
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
        }

        public virtual async Task<EntityViewer> GetViewer()
        {
            EntityViewModel<TEntity> model = await GetViewModel(EntityQueryable.OrderBy(), 1, 20);

            EntityViewer viewer = new EntityViewer();
            viewer.Model = model;

            return viewer;
        }

        protected virtual Task<EntityViewModel<TEntity>> GetViewModel(IQueryable<TEntity> queryable, int page, int size)
        {
            return Task.Run<EntityViewModel<TEntity>>(() =>
            {
                EntityViewModel<TEntity> model = new EntityViewModel<TEntity>(queryable, page, size);
                model.Headers = Metadata.ViewProperties;

                EntityViewButton createButton = new EntityViewButton();
                createButton.Name = "Create";
                createButton.GetInvokeDelegate = new EntityViewButtonCommandDelegate(viewer =>
                {
                    return new Task(async () =>
                    {
                        var item = EntityQueryable.Create();
                        bool? result = null;
                        await viewer.Dispatcher.Invoke(async () =>
                        {
                            viewer.IsLoading = true;
                            var editor = await GetEditor(item);
                            result = editor.ShowDialog();
                        });
                        if (result == true)
                        {
                            await Update(item);
                            model.UpdateTotalPage();
                            model.Items = model.Items.Concat(new TEntity[] { item }).ToArray();
                        }
                        viewer.Dispatcher.Invoke(() =>
                        {
                            viewer.IsLoading = false;
                        });
                    });
                });
                model.ViewButtons = new IViewButton[] { createButton };

                EntityItemButton editButton = new EntityItemButton();
                editButton.Name = "Edit";
                editButton.GetInvokeDelegate = new EntityItemButtonCommandDelegate((viewer, entity) =>
                {
                    return new Task(async () =>
                    {
                        var item = (TEntity)entity;
                        bool? result = null;
                        await viewer.Dispatcher.Invoke(async () =>
                        {
                            viewer.IsLoading = true;
                            var editor = await GetEditor(item);
                            result = editor.ShowDialog();
                        });
                        if (result == true)
                        {
                            await Update(item);
                        }
                        viewer.Dispatcher.Invoke(() =>
                        {
                            viewer.IsLoading = false;
                        });
                    });
                });
                EntityItemButton removeButton = new EntityItemButton();
                removeButton.Name = "Remove";
                removeButton.GetInvokeDelegate = new EntityItemButtonCommandDelegate((viewer, entity) =>
                {
                    return new Task(async () =>
                    {
                        viewer.Dispatcher.Invoke(() =>
                        {
                            viewer.IsLoading = true;
                        });
                        await EntityQueryable.RemoveAsync(entity.Index);
                        model.Items = await EntityQueryable.ToArrayAsync(queryable.Skip(page).Take(size));
                        viewer.Dispatcher.Invoke(() =>
                        {
                            viewer.IsLoading = false;
                        });
                    });
                });
                model.ItemButtons = new IEntityViewButton[] { editButton, removeButton };

                return model;
            });
        }

        public virtual async Task<EntityEditor> GetEditor(Guid id)
        {
            EntityEditor editor = new EntityEditor();
            editor.Model = await GetEditModel(id);
            return editor;
        }

        public virtual async Task<EntityEditor> GetEditor(TEntity entity)
        {
            EntityEditor editor = new EntityEditor();
            editor.Model = await GetEditModel(entity);
            return editor;
        }

        protected virtual async Task<EntityEditModel<TEntity>> GetEditModel(Guid id)
        {
            var item = await EntityQueryable.GetEntityAsync(id);
            return await GetEditModel(item);
        }

        protected virtual Task<EntityEditModel<TEntity>> GetEditModel(TEntity entity)
        {
            return Task.Run<EntityEditModel<TEntity>>(() =>
            {
                EntityEditModel<TEntity> model = new EntityEditModel<TEntity>(entity);
                model.Properties = Metadata.EditProperties;
                return model;
            });
        }

        public virtual async Task<bool> Update(TEntity entity)
        {
            if (entity.Index == Guid.Empty)
                return await EntityQueryable.AddAsync(entity);
            else
                return await EntityQueryable.EditAsync(entity);
        }

        public virtual async Task<EntitySelector> GetSelector()
        {

            EntityViewModel<TEntity> model = await GetSelectorModel(EntityQueryable.OrderBy());

            EntitySelector selector = new EntitySelector();
            selector.Model = model;

            return selector;
        }

        protected virtual Task<EntityViewModel<TEntity>> GetSelectorModel(IQueryable<TEntity> queryable)
        {
            return Task.Run<EntityViewModel<TEntity>>(() =>
            {
                EntityViewModel<TEntity> model = new EntityViewModel<TEntity>(queryable, 1, 20);
                model.Headers = Metadata.ViewProperties;

                EntityViewButton cancelButton = new EntityViewButton();
                cancelButton.Name = "Cancel";
                cancelButton.GetInvokeDelegate = new EntityViewButtonCommandDelegate(viewer =>
                {
                    return new Task(() =>
                    {
                        viewer.Dispatcher.Invoke(() => viewer.DialogResult = false);
                    });
                });
                model.ViewButtons = new IViewButton[] { cancelButton };

                EntityItemButton okButton = new EntityItemButton();
                okButton.Name = "OK";
                okButton.GetInvokeDelegate = new EntityItemButtonCommandDelegate((viewer, entity) =>
                {
                    return new Task(() =>
                    {
                        viewer.Dispatcher.Invoke(() => viewer.DialogResult = true);
                    });
                });
                model.ItemButtons = new IEntityViewButton[] { okButton };

                return model;
            });
        }

        public virtual async Task<EntityMultipleSelector> GetMultipleSelector()
        {

            EntityViewModel<TEntity> model = await GetMultipleSelectorModel(EntityQueryable.OrderBy());

            EntityMultipleSelector selector = new EntityMultipleSelector();
            selector.Model = model;

            return selector;
        }

        protected virtual Task<EntityViewModel<TEntity>> GetMultipleSelectorModel(IQueryable<TEntity> queryable)
        {
            return Task.Run<EntityViewModel<TEntity>>(() =>
            {
                EntityViewModel<TEntity> model = new EntityViewModel<TEntity>(queryable, 1, 20);
                model.Headers = Metadata.ViewProperties;

                EntityViewButton cancelButton = new EntityViewButton();
                cancelButton.Name = "Cancel";
                cancelButton.GetInvokeDelegate = new EntityViewButtonCommandDelegate(viewer =>
                {
                    return new Task(() =>
                    {
                        viewer.Dispatcher.Invoke(() => viewer.DialogResult = false);
                    });
                });
                model.ViewButtons = new IViewButton[] { cancelButton };

                EntityItemButton okButton = new EntityItemButton();
                okButton.Name = "OK";
                okButton.GetInvokeDelegate = new EntityItemButtonCommandDelegate((viewer, entity) =>
                {
                    return new Task(() =>
                    {
                        viewer.Dispatcher.Invoke(() => viewer.DialogResult = true);
                    });
                });
                model.ItemButtons = new IEntityViewButton[] { okButton };

                return model;
            });
        }
    }
}
