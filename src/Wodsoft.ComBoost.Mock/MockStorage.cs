using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockStorage : IStorage
    {
        private IFileProvider _FileProvider;
        private Dictionary<string, MemoryStream> _MemoryStore;
        private Dictionary<string, DateTime> _MemoryDateStore;
        private List<string> _Removed;

        public MockStorage(IFileProvider fileProvider)
        {
            _FileProvider = fileProvider;
            _MemoryDateStore = new Dictionary<string, DateTime>();
            _MemoryStore = new Dictionary<string, MemoryStream>();
            _Removed = new List<string>();
        }

        public Task<bool> DeleteAsync(string path)
        {
            if (!_Removed.Contains(path))
            {
                if (_MemoryStore.Keys.Contains(path))
                {
                    _MemoryStore[path].Dispose();
                    _MemoryStore.Remove(path);
                    _MemoryDateStore.Remove(path);
                    _Removed.Add(path);
                    return Task.FromResult(true);
                }
                if (_FileProvider == null)
                    return Task.FromResult(false);
                var file = _FileProvider.GetFileInfo(path);
                if (file.Exists)
                {
                    _Removed.Add(path);
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public async Task<Stream> GetAsync(string path)
        {
            if (!_Removed.Contains(path))
            {
                if (_MemoryStore.Keys.Contains(path))
                {
                    MemoryStream cacheStream = _MemoryStore[path];
                    MemoryStream newStream = new MemoryStream();
                    cacheStream.Position = 0;
                    await cacheStream.CopyToAsync(newStream);
                    newStream.Position = 0;
                    return newStream;
                }
                if (_FileProvider == null)
                    return null;
                var file = _FileProvider.GetFileInfo(path);
                if (file.Exists)
                    return file.CreateReadStream();
            }
            return null;
        }

        public async Task<IStorageFile> GetFileAsync(string path)
        {
            if (!_Removed.Contains(path))
            {
                if (_MemoryStore.Keys.Contains(path))
                {
                    MemoryStream cacheStream = _MemoryStore[path];
                    MemoryStream newStream = new MemoryStream();
                    cacheStream.Position = 0;
                    await cacheStream.CopyToAsync(newStream);
                    newStream.Position = 0;
                    return new MockStorageFile(path, _MemoryDateStore[path], newStream);
                }
                if (_FileProvider == null)
                    return null;
                var file = _FileProvider.GetFileInfo(path);
                if (file.Exists)
                {
                    return new MockStorageFile(file, path);
                }
            }
            return null;
        }

        public async Task<string> PutAsync(Stream stream, string filename)
        {
            var path = Guid.NewGuid().ToString() + Path.GetExtension(filename);
            MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            _MemoryStore.Add(path, memoryStream);
            _MemoryDateStore.Add(path, DateTime.Now);
            return path;
        }
    }
}
