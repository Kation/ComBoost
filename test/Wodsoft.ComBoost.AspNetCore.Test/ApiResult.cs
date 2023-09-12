using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore.Test
{
    public class ApiResult
    {
        public int Code { get; set; }

        public string Message { get; set; }
    }

    public class ApiResult<T> : ApiResult
    {
        public T Content { get; set; }
    }
}
