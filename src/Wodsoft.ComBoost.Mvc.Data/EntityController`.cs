using Microsoft.OData.Edm.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityController<T> : EntityController
        where T : class, IEntity, new()
    {
        public EntityController()
        {
            EntityService = DomainProvider.GetService<EntityDomainService<T>>();
            Metadata = EntityDescriptor.GetMetadata<T>();
        }

        protected EntityDomainService<T> EntityService { get; private set; }

        protected IEntityMetadata Metadata { get; private set; }

        public async Task<IActionResult> Index()
        {
            var context = CreateDomainContext();
            try
            {
                EntityService.ListQuery += t =>
                {
                    return t;
                };
                await EntityService.ExecuteAsync(context,
                    new Func<IDatabaseContext, IAuthenticationProvider, Task>(EntityService.List));
                return View(context.Result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        public async Task<IActionResult> Create()
        {
            var context = CreateDomainContext();
            try
            {
                await EntityService.ExecuteAsync(context,
                    new Func<IDatabaseContext, IAuthenticationProvider, Task>(EntityService.Create));
                return View(context.Result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        public async Task<IActionResult> Edit()
        {
            var context = CreateDomainContext();
            try
            {
                await EntityService.ExecuteAsync(context,
                    new Func<IDatabaseContext, IAuthenticationProvider, object, Task>(EntityService.Edit));
                return View(context.Result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
        }

        public async Task<IActionResult> Detail()
        {
            var context = CreateDomainContext();
            try
            {
                await EntityService.ExecuteAsync(context,
                    new Func<IDatabaseContext, IAuthenticationProvider, object, Task>(EntityService.Detail));
                return View(context.Result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
        }

        public async Task<IActionResult> Remove()
        {
            var context = CreateDomainContext();
            try
            {
                await EntityService.ExecuteAsync(context,
                    new Func<IDatabaseContext, IAuthenticationProvider, object, Task>(EntityService.Remove));
                return View(context.Result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
        }

        public async Task<IActionResult> Update()
        {
            var context = CreateDomainContext();
            try
            {
                await EntityService.ExecuteAsync(context,
                    new Func<IDatabaseContext, IAuthenticationProvider, IValueProvider, Task>(EntityService.Update));
                var result = (EntityUpdateModel)context.Result;
                if (result.IsSuccess)
                    return StatusCode(204);
                return Json(result.ErrorMessage.Select(t =>
                    new
                    {
                        Property = t.Key.ClrName,
                        Text = t.Value
                    }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
        }


    }
}
