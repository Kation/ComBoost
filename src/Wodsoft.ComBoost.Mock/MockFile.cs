using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockFile : ISelectedFile
    {
        public MockFile(string contentType, string filename, Stream stream)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (contentType == null)
                contentType = "application/octet-stream";
            ContentType = contentType;
            Filename = filename;
            Stream = stream;
        }

        public string ContentType { get; private set; }

        public string Filename { get; private set; }

        public Stream Stream { get; private set; }
    }
}
