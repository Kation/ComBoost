using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public class TestService : DomainService
    {
        public Task Test([FromValue] string text)
        {
            Console.WriteLine(text);
            return Task.FromResult(0);
        }
    }
}
