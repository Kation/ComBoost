using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IStorageProvider
    {
        IStorage GetStorage();

        IStorage GetStorage(string name);
    }
}
