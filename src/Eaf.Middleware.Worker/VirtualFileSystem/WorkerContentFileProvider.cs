using Abp;
using Abp.Dependency;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Worker.VirtualFileSystem
{
    /// <summary>
    /// Representa a classe WorkerContentFileProvider.
    /// </summary>
    public class WorkerContentFileProvider : IWorkerContentFileProvider, ISingletonDependency
    {
        private readonly Lazy<IFileProvider> _fileProvider;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly string _rootPath = "/";

        /// <summary>
        /// WorkerContentFileProvider.
        /// </summary>
        /// <param name="hostingEnvironment">Parâmetro hostingEnvironment.</param>
        /// <returns>Resultado da operação.</returns>
        public WorkerContentFileProvider(
        IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            _fileProvider = new Lazy<IFileProvider>(CreateFileProvider); // NOSONAR
        }

        /// <summary>
        /// GetFileInfo.
        /// </summary>
        /// <param name="subpath">Parâmetro subpath.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual IFileInfo GetFileInfo(string subpath)
        {
            Check.NotNullOrEmpty(subpath, nameof(subpath));

            if (PathUtils.PathNavigatesAboveRoot(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            var fileInfo = _fileProvider.Value.GetFileInfo(subpath);
            if (fileInfo.Exists)
            {
                return fileInfo;
            }

            return _fileProvider.Value.GetFileInfo(_rootPath + subpath);
        }

        /// <summary>
        /// GetDirectoryContents.
        /// </summary>
        /// <param name="subpath">Parâmetro subpath.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual IDirectoryContents GetDirectoryContents([NotNull] string subpath)
        {
            Check.NotNullOrEmpty(subpath, nameof(subpath));

            if (PathUtils.PathNavigatesAboveRoot(subpath))
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var directory = _fileProvider.Value.GetDirectoryContents(subpath);
            if (directory.Exists)
            {
                return directory;
            }

            return _fileProvider.Value.GetDirectoryContents(_rootPath + subpath);
        }

        /// <summary>
        /// Watch.
        /// </summary>
        /// <param name="filter">Parâmetro filter.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual IChangeToken Watch(string filter)
        {
            return new CompositeChangeToken(
                new[]
                {
                    _fileProvider.Value.Watch(_rootPath + filter),
                    _fileProvider.Value.Watch(filter)
                }
            );
        }

        protected virtual IFileProvider CreateFileProvider()
        {
            var fileProviders = new List<IFileProvider>
            {
                new PhysicalFileProvider(_hostingEnvironment.ContentRootPath)
            };

            return new CompositeFileProvider(
                fileProviders
            );
        }
    }
}