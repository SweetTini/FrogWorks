using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FrogWorks
{
    public static class AssetManager
    {
        internal static Dictionary<Type, Dictionary<string, object>> Cache { get; }

        static AssetManager()
        {
            Cache = new Dictionary<Type, Dictionary<string, object>>();
        }

        static void CreateIfNotExist(Type type)
        {
            if (!Cache.ContainsKey(type))
                Cache.Add(type, new Dictionary<string, object>());
        }

        static void DisposeIfAny(Type type)
        {
            if (Cache.ContainsKey(type) && type.GetInterfaces().Contains(typeof(IDisposable)))
                foreach (IDisposable disposable in Cache[type].Values)
                    disposable.Dispose();
        }

        internal static T GetFromCache<T>(string fileName, Func<string, T> callback)
        {
            var loadedAsset = null as object;
            var tag = Path.ChangeExtension(fileName, null);

            CreateIfNotExist(typeof(T));

            if (!Cache[typeof(T)].TryGetValue(tag, out loadedAsset))
            {
                try
                {
                    loadedAsset = callback(fileName);

                    if (loadedAsset != null)
                        Cache[typeof(T)].Add(tag, loadedAsset);
                }
                catch { }
            }

            return (T)loadedAsset;
        }

        public static void ClearCache<T>()
        {
            if (Cache.ContainsKey(typeof(T)))
            {
                DisposeIfAny(typeof(T));
                Cache[typeof(T)].Clear();
            }
        }

        public static void ClearCache()
        {
            foreach (var type in Cache.Keys)
            {
                DisposeIfAny(type);
                Cache[type].Clear();
            }
        }

        public static FileStream GetStream(string path, params string[] fileTypes)
        {
            var fullPath = GetFullPath(path, fileTypes);
            return !string.IsNullOrEmpty(fullPath) ? File.OpenRead(fullPath) : null;
        }

        public static string GetFullPath(string path, params string[] fileTypes)
        {
            var fullPath = Path.Combine(Runner.Application.ContentDirectory, path);

            if (Path.HasExtension(fullPath) && File.Exists(fullPath))
            {
                if (!fileTypes.Any() || fileTypes
                    .Select(x => x.ToLower()).Contains(Path.GetExtension(fullPath)))
                    return fullPath;
            }
            else if (fileTypes.Any())
            {
                return fileTypes
                    .Select(ft => Path.ChangeExtension(fullPath, ft.ToLower()))
                    .Where(fp => File.Exists(fp))
                    .FirstOrDefault();
            }

            return null;
        }
    }
}
