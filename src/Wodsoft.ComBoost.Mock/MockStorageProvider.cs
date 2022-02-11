using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockStorageProvider : IStorageProvider
    {
        private MockStorage? _Storage;
        private IFileProvider? _FileProvider;

        public MockStorageProvider() { }

        public MockStorageProvider(string root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            _FileProvider = new PhysicalFileProvider(root);
        }

        public MockStorageProvider(IFileProvider fileProvider)
        {
            if (fileProvider == null)
                throw new ArgumentNullException(nameof(fileProvider));
            _FileProvider = fileProvider;
        }

        public IStorage GetStorage()
        {
            if (_Storage == null)
                _Storage = new MockStorage(_FileProvider);
            return _Storage;
        }

        public IStorage GetStorage(string name)
        {
            throw new NotSupportedException("不支持的方法。");
        }
    }
}
