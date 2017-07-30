using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class PhysicalStorageFile : IStorageFile
    {
        public PhysicalStorageFile(FileInfo info, string path)
        {
            ModifiedDate = info.LastWriteTime;
            Path = path;
            Stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public DateTime ModifiedDate { get; private set; }

        public string Path { get; private set; }

        public Stream Stream { get; private set; }
    }
}
