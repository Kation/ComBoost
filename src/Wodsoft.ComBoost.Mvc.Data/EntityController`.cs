using System;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.ExceptionServices;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityController<T> : EntityController, IHaveEntityMetadata
        where T : class, IEntity, new()
    {
        private EntityDomainService<T> _EntityService;
        protected EntityDomainService<T> EntityService
        {
            get
            {
                if (_EntityService == null)
                    _EntityService = GetEntityDomainService();
                return _EntityService;
            }
        }

        private IEntityMetadata _Metadata;
        public IEntityMetadata Metadata
        {
            get
            {
                if (_Metadata == null)
                    _Metadata = EntityDescriptor.GetMetadata<T>();
                return _Metadata;
            }
        }

        [HttpGet]
        [EntityAuthorize]
        public virtual async Task<IActionResult> Index()
        {
            var context = CreateIndexContext();
            try
            {
                var model = await EntityService.ExecuteList(context);
                foreach (var button in model.ViewButtons)
                    button.SetTarget(HttpContext.RequestServices);
                if (IsJsonRequest())
                {
                    EntityJsonConverter entityConverter = new Mvc.EntityJsonConverter(EntityDomainAuthorizeOption.View, HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>().GetAuthentication());
                    return Content(JsonConvert.SerializeObject(model, entityConverter, EntityMetadataJsonConverter.Converter, PropertyMetadataJsonConverter.Converter, EntityViewModelJsonConverter.Converter), "application/json", System.Text.Encoding.UTF8);
                }
                return View(model);
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateIndexContext()
        {
            return CreateDomainContext();
        }

        [HttpGet]
        [EntityAuthorize]
        public virtual async Task<IActionResult> Create()
        {
            var context = CreateCreateContext();
            try
            {
                var model = await EntityService.ExecuteCreate(context);
                if (IsJsonRequest())
                {
                    EntityJsonConverter entityConverter = new Mvc.EntityJsonConverter(EntityDomainAuthorizeOption.Create, HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>().GetAuthentication());
                    return Content(JsonConvert.SerializeObject(model, entityConverter, EntityMetadataJsonConverter.Converter, PropertyMetadataJsonConverter.Converter, EntityEditModelJsonConverter.Converter), "application/json", System.Text.Encoding.UTF8);
                }
                return View("Edit", model);
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateCreateContext()
        {
            return CreateDomainContext();
        }

        [HttpGet]
        [EntityAuthorize]
        public virtual async Task<IActionResult> Edit()
        {
            var context = CreateEditContext();
            try
            {
                var model = await EntityService.ExecuteEdit(context);
                if (IsJsonRequest())
                {
                    EntityJsonConverter entityConverter = new Mvc.EntityJsonConverter(EntityDomainAuthorizeOption.Edit, HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>().GetAuthentication());
                    return Content(JsonConvert.SerializeObject(model, entityConverter, EntityMetadataJsonConverter.Converter, PropertyMetadataJsonConverter.Converter, EntityEditModelJsonConverter.Converter), "application/json", System.Text.Encoding.UTF8);
                }
                return View(model);
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else if (ex.InnerException is EntityNotFoundException)
                    return StatusCode(404, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateEditContext()
        {
            return CreateDomainContext();
        }

        [HttpGet]
        [EntityAuthorize]
        public virtual async Task<IActionResult> Detail()
        {
            var context = CreateDetailContext();
            try
            {
                var model = await EntityService.ExecuteDetail(context);
                if (IsJsonRequest())
                {
                    EntityJsonConverter entityConverter = new Mvc.EntityJsonConverter(EntityDomainAuthorizeOption.Detail, HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>().GetAuthentication());
                    return Content(JsonConvert.SerializeObject(model, entityConverter, EntityMetadataJsonConverter.Converter, PropertyMetadataJsonConverter.Converter, EntityEditModelJsonConverter.Converter), "application/json", System.Text.Encoding.UTF8);
                }
                return View(model);
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else if (ex.InnerException is EntityNotFoundException)
                    return StatusCode(404, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateDetailContext()
        {
            return CreateDomainContext();
        }

        [HttpPost]
        [EntityAuthorize]
        public virtual async Task<IActionResult> Remove()
        {
            var context = CreateRemoveContext();
            try
            {
                await EntityService.ExecuteRemove(context);
                return StatusCode(200);
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else if (ex.InnerException is EntityNotFoundException)
                    return StatusCode(404, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateRemoveContext()
        {
            return CreateDomainContext();
        }

        [HttpPost]
        [EntityAuthorize]
        public virtual async Task<IActionResult> Update()
        {
            var context = CreateUpdateContext();
            try
            {
                var result = await EntityService.ExecuteUpdate(context);
                if (result.IsSuccess)
                    return StatusCode(204);
                Response.StatusCode = 400;
                return Json(result.ErrorMessage.Select(t =>
                    new
                    {
                        Property = t.Key.ClrName,
                        Name = t.Key.Name,
                        ErrorMessage = t.Value
                    }));
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else if (ex.InnerException is EntityNotFoundException)
                    return StatusCode(404, ex.InnerException.Message);
                else if (ex.InnerException is ArgumentException || ex.InnerException is ArgumentNullException || ex.InnerException is ArgumentOutOfRangeException)
                    return StatusCode(400, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateUpdateContext()
        {
            return CreateDomainContext();
        }

        [HttpGet]
        [EntityAuthorize]
        public virtual async Task<IActionResult> Selector()
        {
            var context = CreateSelectorContext();
            try
            {
                var model = await EntityService.ExecuteList(context);
                if (IsJsonRequest())
                {
                    EntityJsonConverter entityConverter = new Mvc.EntityJsonConverter(EntityDomainAuthorizeOption.View, HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>().GetAuthentication());
                    return Content(JsonConvert.SerializeObject(model, entityConverter, EntityMetadataJsonConverter.Converter, PropertyMetadataJsonConverter.Converter, EntityViewModelJsonConverter.Converter), "application/json", System.Text.Encoding.UTF8);
                }
                return View(model);
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateSelectorContext()
        {
            return CreateDomainContext();
        }

        [HttpGet]
        [EntityAuthorize]
        public virtual async Task<IActionResult> MultipleSelector()
        {
            var context = CreateMultipleSelectorContext();
            try
            {
                var model = await EntityService.ExecuteList(context);
                if (IsJsonRequest())
                {
                    EntityJsonConverter entityConverter = new Mvc.EntityJsonConverter(EntityDomainAuthorizeOption.View, HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>().GetAuthentication());
                    return Content(JsonConvert.SerializeObject(model, entityConverter, EntityMetadataJsonConverter.Converter, PropertyMetadataJsonConverter.Converter, EntityViewModelJsonConverter.Converter), "application/json", System.Text.Encoding.UTF8);
                }
                return View(model);
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is UnauthorizedAccessException)
                    return StatusCode(401, ex.InnerException.Message);
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        protected virtual ControllerDomainContext CreateMultipleSelectorContext()
        {
            return CreateDomainContext();
        }

        protected virtual EntityDomainService<T> GetEntityDomainService()
        {
            return DomainProvider.GetService<EntityDomainService<T>>();
        }

        protected virtual bool IsJsonRequest()
        {
            var accept = Request.Headers["accept"].ToString();
            var json = accept.IndexOf("application/json");
            if (json == -1)
                return false;
            var html = accept.IndexOf("text/html");
            if (html == -1)
                return true;
            return json < html;
        }

        protected override ControllerDomainContext CreateDomainContext()
        {
            var context = base.CreateDomainContext();
            context.ValueProvider.SetAlias("id", "index");
            return context;
        }
    }
}
