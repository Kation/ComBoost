using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcDomainContext : DomainContext
    {
        public MvcDomainContext(Controller controller)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            Controller = controller;
        }

        public Controller Controller { get; private set; }

        private ReadableStringProvider _QueryProvider;
        public ReadableStringProvider QueryProvider
        {
            get
            {
                if (_QueryProvider == null)
                    _QueryProvider = new ReadableStringProvider(Controller.Request.Query);
                return _QueryProvider;
            }
        }

        private ReadableStringProvider _FormProvider;
        public ReadableStringProvider FormProvider
        {
            get
            {
                if (_FormProvider == null)
                    _FormProvider = new ReadableStringProvider(Controller.Request.Form);
                return _FormProvider;
            }
        }

        public override object GetService(Type serviceType)
        {
            return base.GetService(serviceType);
        }
    }
}
