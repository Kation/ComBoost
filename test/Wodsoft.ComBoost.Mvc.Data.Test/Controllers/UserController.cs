﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Test.Entities;
using Wodsoft.ComBoost.Test.Models;

namespace Wodsoft.ComBoost.Mvc.Data.Test.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : EntityDTOController<UserDto, UserDto, UserDto, UserDto>
    {
    }
}
