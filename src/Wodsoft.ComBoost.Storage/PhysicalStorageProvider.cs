using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class PhysicalStorageProvider : IStorageProvider
    {
        private PhysicalStorageOptions _Options;

        public PhysicalStorageProvider(PhysicalStorageOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _Options = options;
        }

        public IStorage GetStorage()
        {
            return new PhysicalStorage(_Options);
        }

        public IStorage GetStorage(string name)
        {
            throw new NotSupportedException("不支持的方法。");
        }
    }
}
