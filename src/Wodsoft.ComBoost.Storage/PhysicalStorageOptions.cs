using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class PhysicalStorageOptions
    {
        public string Root { get; set; }

        public Func<string> FolderSelector { get; set; }

        public Func<string, string> FilenameSelector { get; set; }
    }
}
