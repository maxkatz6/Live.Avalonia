using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Live.Avalonia
{
    public sealed class LiveViewHost : Window, IDisposable
    {
        private readonly LiveFileWatcher _assemblyWatcher;
        private readonly LiveSourceWatcher _sourceWatcher;
        private readonly IDisposable _subscription;
        private readonly Action<string> _logger;
        private readonly string _assemblyPath;

        public LiveViewHost(ILiveView view, Action<string> logger)
        {
            _logger = logger;
            _sourceWatcher = new LiveSourceWatcher(logger);
            _assemblyWatcher = new LiveFileWatcher(logger);
            _assemblyPath = view.GetType().Assembly.Location;
            
            var loader = new LiveControlLoader(logger);
            _subscription = _assemblyWatcher
                .FileChanged
                .ObserveOn(AvaloniaScheduler.Instance)
                .Select(path => loader.LoadControl(path, this))
                .Subscribe(control => Content = control);

            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                Dispose();
                Process.GetCurrentProcess().Kill();
            };
            
            Console.CancelKeyPress += (sender, args) =>
            {
                Dispose();
                Process.GetCurrentProcess().Kill();
            };
        }

        public void StartWatchingSourceFilesForHotReloading()
        {
            _logger("Starting source and assembly file watchers...");
            var liveAssemblyPath = _sourceWatcher.StartRebuildingAssemblySources(_assemblyPath);
            _assemblyWatcher.StartWatchingFileCreation(liveAssemblyPath);
        }

        public void Dispose()
        {
            _logger("Disposing LiveViewHost internals...");
            _sourceWatcher.Dispose();
            _assemblyWatcher.Dispose();
            _subscription.Dispose();
            _logger("Successfully disposed LiveViewHost internals.");
        }
    }
}