using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IStorage
    {
        Task<string> Put(Stream stream, string filename);

        Task<Stream> Get(string path);

        Task<bool> Delete(string path);
    }
}
