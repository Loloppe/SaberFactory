using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace SaberFactory
{
    public class SaberFileWatcher
    {
        private const string Filter = "*.saber";

        public bool IsWatching { get; private set; }
        private readonly DirectoryInfo _dir;
        private readonly ICoroutineStarter _coroutineStarter;

        private FileSystemWatcher _watcher;

        public SaberFileWatcher(PluginDirectories dirs, ICoroutineStarter coroutineStarter)
        {
            _dir = dirs.CustomSaberDir;
            _coroutineStarter = coroutineStarter;
        }

        public event Action<string> OnSaberUpdate;

        public void Watch()
        {
            if (_watcher is { })
            {
                StopWatching();
            }

            _watcher = new FileSystemWatcher(_dir.FullName, Filter);

            _watcher.NotifyFilter = NotifyFilters.LastWrite;

            _watcher.Changed += WatcherOnCreated;

            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;

            IsWatching = true;
        }

        private void WatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            _coroutineStarter.StartCoroutine(Initiate(e.FullPath));
        }

        private IEnumerator Initiate(string filename)
        {
            var seconds = 0f;
            while (seconds < 10)
            {
                if (File.Exists(filename))
                {
                    yield return new WaitForSeconds(0.5f);
                    OnSaberUpdate?.Invoke(filename);
                    yield break;
                }

                yield return new WaitForSeconds(0.5f);
                seconds += 0.5f;
            }
        }

        public void StopWatching()
        {
            if (_watcher is null)
            {
                return;
            }

            _watcher.Changed -= WatcherOnCreated;
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _watcher = null;
            IsWatching = false;
        }
    }
}