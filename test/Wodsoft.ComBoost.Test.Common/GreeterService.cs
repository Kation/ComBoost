using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [DomainTemplateImplementer(typeof(IGreeterTemplate))]
    public class GreeterService : DomainService
    {
        public Task<HelloResponse> SayHi([FromValue] HelloRequest request, [FromValue(false)] InvokeType invokeType)
        {
            return Task.FromResult(new HelloResponse { Answer = $"Hi {request.Name}." });
        }

        public Task<string> Hello([FromValue(false)] string text)
        {
            Debug.WriteLine("Receive hello: " + text);
            return Task.FromResult("Hi.");
        }

        [SkipFilter]
        public Task<bool> NoExecute()
        {
            return Task.FromResult(true);
        }

        public Task Test([FromValue(false)] Guid id)
        {
            return Task.CompletedTask;
        }

        public async Task EventTest()
        {
            await RaiseEvent(new RequestEventArgs());
        }
    }
}
