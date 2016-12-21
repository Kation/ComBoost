using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class PhysicalStorageOptions
    {
        public string Root { get; set; }

        public Func<string> FolderSelector { get; set; }

        public Func<string, string> FilenameSelector { get; set; }

        public static readonly Func<string> DefaultFolderSelector = () => Path.DirectorySeparatorChar + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + Path.DirectorySeparatorChar + DateTime.Now.Day.ToString().PadLeft(2, '0');

        public static readonly Func<string, string> DefaultFilenameSelector = (filename) => Guid.NewGuid().ToString() + Path.GetExtension(filename);

        public static PhysicalStorageOptions CreateDefault(string root)
        {
            return new PhysicalStorageOptions { Root = root, FilenameSelector = DefaultFilenameSelector, FolderSelector = DefaultFolderSelector };
        }
    }
}
