using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityController
    {
        public IEntityContextBuilder EntityBuilder { get; private set; }

        public EntityController(IEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            EntityBuilder = builder;
        }

        public virtual EntityViewer GetViewer<TEntity>() where TEntity : class,IEntity, new()
        {
            IEntityQueryable<TEntity> context = EntityBuilder.GetContext<TEntity>();

            EntityMetadata metadata = EntityAnalyzer.GetMetadata<TEntity>();

            EntityViewModel<TEntity> model = new EntityViewModel<TEntity>(context.OrderBy());
            model.Headers = metadata.ViewProperties;
            model.UpdateItems();

            EntityViewer viewer = new EntityViewer(this);
            viewer.Model = model;

            WpfViewButton createButton = new WpfViewButton();
            createButton.Name = "Create";
            createButton.Command = new EntityCommand(new EntityCommand.ExecuteDelegate(t =>
            {
                viewer.NavigationService.Navigate(GetEditor<TEntity>(context.Create()));
            }));
            model.Buttons = new EntityViewButton[] { createButton };

            return viewer;
        }

        public virtual EntityEditor GetEditor<TEntity>(TEntity entity) where TEntity : class, IEntity, new()
        {
            EntityMetadata metadata = EntityAnalyzer.GetMetadata<TEntity>();

            EntityEditModel<TEntity> model = new EntityEditModel<TEntity>(entity);
            model.Properties = metadata.EditProperties;
            EntityEditor editor = new EntityEditor(this);
            editor.Model = model;
            return editor;
        }
    }
}
