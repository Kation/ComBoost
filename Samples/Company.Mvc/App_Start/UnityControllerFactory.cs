using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Company.Entity;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Company.Mvc.App_Start.UnityControllerFactory), "Start")]
namespace Company.Mvc.App_Start
{
    public class UnityControllerFactory : EntityControllerFactory
    {
        public static void Start()
        {
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(UnityConfig.GetConfiguredContainer()));
        }
    
        IUnityContainer _container;

        public UnityControllerFactory(IUnityContainer container)
        {
            _container = container;

            //Change EntityContextBuilder to your class that inherit IEntityContextBuilder interface.
            //If your entity context builder has constructor with arguments, continue register types that you need.
            container.RegisterType<DbContext,DataContext>(new MvcLifetimeManager());
            container.RegisterType<IEntityContextBuilder, EntityContextBuilder>(new MvcLifetimeManager());

            //Register your entity here:
            //RegisterController<EntityType>();
            //...
            RegisterController<Employee>();
            RegisterController<EmployeeGroup>();

            System.Web.Security.ComBoostPrincipal.Resolve = EntityResolve;
        }

        private IRoleEntity EntityResolve(Type entityType, string username)
        {
            IEntityContextBuilder builder = _container.Resolve<IEntityContextBuilder>();
            dynamic context = builder.GetContext(entityType);
            return context.GetEntity(new Guid(username));
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, "Controller Not Found.");
            }
            return _container.Resolve(controllerType) as IController;
        }
    }

    public class MvcLifetimeManager : LifetimeManager
    {
        private readonly Guid _Key = Guid.NewGuid();

        public override object GetValue()
        {
            return HttpContext.Current.Items[_Key];
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[_Key] = newValue;
        }

        public override void RemoveValue()
        {
            var obj = GetValue();
            HttpContext.Current.Items.Remove(obj);
        }
    }
}