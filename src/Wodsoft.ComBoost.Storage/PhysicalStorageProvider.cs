using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class PhysicalStorageProvider : IStorageProvider
    {
        public PhysicalStorageProvider()
        {

        }

        public IStorage GetStorage()
        {
            throw new NotImplementedException();
        }

        public IStorage GetStorage(string name)
        {
            throw new NotSupportedException("不支持的方法。");
        }
    }
}
