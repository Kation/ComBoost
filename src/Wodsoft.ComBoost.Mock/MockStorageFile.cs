using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockStorageFile : IStorageFile
    {
        public MockStorageFile(string path, DateTime modifiedDate, Stream stream)
        {
            Stream = stream;
            Path = path;
            ModifiedDate = modifiedDate;
        }

        public MockStorageFile(IFileInfo file, string path) : this(path, file.LastModified.LocalDateTime, file.CreateReadStream())
        {

        }

        public DateTime ModifiedDate { get; private set; }

        public string Path { get; private set; }

        public Stream Stream { get; private set; }
    }
}
