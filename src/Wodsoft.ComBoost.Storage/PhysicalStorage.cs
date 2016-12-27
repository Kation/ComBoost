using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class PhysicalStorage : IStorage
    {
        private PhysicalStorageOptions _Options;

        public PhysicalStorage(PhysicalStorageOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Root == null)
                throw new ArgumentException();
            if (options.FolderSelector == null)
                throw new ArgumentException();
            if (options.FilenameSelector == null)
                throw new ArgumentException();
            _Options = options;
        }

        public Task<bool> DeleteAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            path = Path.Combine(_Options.Root, path);
            if (!File.Exists(path))
                return Task.FromResult(false);
            File.Delete(path);
            return Task.FromResult(true);
        }

        public Task<Stream> GetAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            path = Path.Combine(_Options.Root, path);
            if (!File.Exists(path))
                return null;
            return Task.FromResult<Stream>(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public async Task<string> PutAsync(Stream stream, string filename)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            string folder = _Options.FolderSelector();
            filename = _Options.FilenameSelector(filename);
            var path = Path.Combine(_Options.Root, folder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, filename);
            var file = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            await stream.CopyToAsync(file);
            await file.FlushAsync();
#if NET451
            file.Close();
#endif
            file.Dispose();
            return Path.Combine(folder, filename).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
