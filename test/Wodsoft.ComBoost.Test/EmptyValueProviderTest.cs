using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wodsoft.ComBoost.Test
{
    public class EmptyValueProviderTest
    {
        [Fact]
        public void ConvertTest()
        {
            EmptyValueProvider valueProvider = new EmptyValueProvider();
            valueProvider.SetValue("a", 1);
            Assert.Equal("1", valueProvider.GetValue<string>("a"));
        }
    }
}
