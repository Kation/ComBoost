using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public static class MvcMockValueProviderExtensions
    {
        public static void SetResponseStream(this MockValueProvider valueProvider, Stream stream)
        {
            valueProvider.SetValue("$response", stream);
        }

        public static void SetRequestStream(this MockValueProvider valueProvider, Stream stream)
        {
            valueProvider.SetValue("$request", stream);            
        }

        public static void SetRequestUri(this MockValueProvider valueProvider, Uri uri)
        {
            valueProvider.SetValue(uri);
        }
    }
}
