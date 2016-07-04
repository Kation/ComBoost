using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ISelectedFile
    {
        string Filename { get; }

        string ContentType { get; }

        Stream Stream { get; }
    }
}
