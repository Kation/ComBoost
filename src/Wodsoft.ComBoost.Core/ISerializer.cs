using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ISerializer
    {
        void Serialize(Stream stream, object value);

        object Deserialize(Stream stream);
    }
}
