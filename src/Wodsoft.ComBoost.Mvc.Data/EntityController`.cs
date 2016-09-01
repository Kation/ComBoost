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
    public class EntityController<T> : EntityController, IHaveEntityMetadata
        where T : class, IEntity, new()
    {
        public EntityController()
        {
            EntityService = DomainProvider.GetService<EntityDomainService<T>>();
            Metadata = EntityDescriptor.GetMetadata<T>();
        }

        protected EntityDomainService<T> EntityService { get; private set; }

        public IEntityMetadata Metadata { get; private set; }

        public async Task<IActionResult> Index()
        {
            var context = CreateDomainContext();
            try
            {
                EntityService.ListQuery += t =>
                {
                    return t;
                };
                var model = await EntityService.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, IEntityViewModel<T>>(context, EntityService.List);
                return View(model);
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
                var model = await EntityService.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, IEntityEditModel<T>>(context, EntityService.Create);
                return View(model);
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
                var model = await EntityService.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, object, IEntityEditModel<T>>(context, EntityService.Edit);
                return View(model);
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
                var model = await EntityService.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, object, IEntityEditModel<T>>(context, EntityService.Detail);
                return View(model);
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
                await EntityService.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, object>(context, EntityService.Remove);
                return StatusCode(200);
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
                var result = await EntityService.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, IValueProvider, EntityUpdateModel>(context, EntityService.Update);
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
