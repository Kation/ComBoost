using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Aggregation.Mvc;
using Wodsoft.ComBoost.Aggregation.Test.Entities;
using Wodsoft.ComBoost.Aggregation.Test.Models;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Aggregation.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [AggregateResult]
        public Task<User[]> Get([FromServices] IEntityContext<UserEntity> userContext)
        {
            return userContext.Query().Select(t => new User
            {
                Id = t.Id,
                CreationDate = t.CreationDate,
                DisplayName = t.DisplayName,
                ModificationDate = t.ModificationDate,
                OrganizationId = t.OrganizationId,
                UserName = t.UserName
            }).ToArrayAsync();
        }
    }
}
