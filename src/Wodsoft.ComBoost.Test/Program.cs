using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceCollection collection = new ServiceCollection();
            ValueProvider valueProvider = new ValueProvider();
            valueProvider.Add("text", "Hello world");
            collection.AddSingleton<IValueProvider>(valueProvider);
            var serviceProvider = collection.BuildServiceProvider();

            TestService service = new TestService();

            DomainContext context = new DomainContext(serviceProvider);

            service.ExecuteAsync<string>(context, service.Test).Wait();

            Console.ReadLine();
        }
    }
}
