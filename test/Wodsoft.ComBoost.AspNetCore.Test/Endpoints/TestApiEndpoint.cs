using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.AspNetCore.Test.Services;

namespace Wodsoft.ComBoost.AspNetCore.Test.Endpoints
{
    [DomainEndpoint]
    public partial class TestApiEndpoint : ApiEndpoint<ITestDomainService>
    {
        protected override IEnumerable<ApiResponseType> GetSupportedResponse(MethodInfo method)
        {
            return new ApiResponseType[]
            {
                new ApiResponseType
                {
                    ApiResponseFormats = { new ApiResponseFormat { MediaType = "application/json" } },
                    ModelMetadata = new DomainEndpointModelMetadata(ModelMetadataIdentity.ForType(method.ReturnType == typeof(Task) ? typeof(ApiResult) : typeof(ApiResult<>).MakeGenericType(method.ReturnType.GetGenericArguments()[0]))),
                    StatusCode = 200
                }
            };
        }
    }
}
