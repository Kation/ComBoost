using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class SelectedFile : ISelectedFile
    {
        public SelectedFile(IFormFile file)
        {
            ContentType = file.ContentType;
            Filename = file.FileName;
            Stream = file.OpenReadStream();
        }

        public string ContentType { get; private set; }

        public string Filename { get; private set; }

        public Stream Stream { get; private set; }
    }
}
