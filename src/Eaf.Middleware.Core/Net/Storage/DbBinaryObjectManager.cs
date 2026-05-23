using Abp.Dependency;
using Abp.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Storage
{
    /// <summary>
    /// Representa a classe DbBinaryObjectManager.
    /// </summary>
    public class DbBinaryObjectManager : IBinaryObjectManager, ITransientDependency
    {
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;

        /// <summary>
        /// DbBinaryObjectManager.
        /// </summary>
        /// <param name="binaryObjectRepository">Parâmetro binaryObjectRepository.</param>
        /// <returns>Resultado da operação.</returns>
        public DbBinaryObjectManager(IRepository<BinaryObject, Guid> binaryObjectRepository)
        {
            _binaryObjectRepository = binaryObjectRepository;
        }

        /// <summary>
        /// DeleteAsync.
        /// </summary>
        /// <param name="id">Parâmetro id.</param>
        public Task DeleteAsync(Guid id)
        {
            return _binaryObjectRepository.DeleteAsync(id);
        }

        /// <summary>
        /// GetOrNullAsync.
        /// </summary>
        /// <param name="id">Parâmetro id.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<BinaryObject> GetOrNullAsync(Guid id)
        {
            return _binaryObjectRepository.FirstOrDefaultAsync(id);
        }

        /// <summary>
        /// GetOrNullAsync.
        /// </summary>
        /// <param name="fileName">Parâmetro fileName.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<BinaryObject> GetOrNullAsync(string fileName)
        {
            return _binaryObjectRepository.FirstOrDefaultAsync(x => x.FileName.Contains(fileName));
        }

        /// <summary>
        /// SaveAsync.
        /// </summary>
        /// <param name="file">Parâmetro file.</param>
        public Task SaveAsync(BinaryObject file)
        {
            return _binaryObjectRepository.InsertAsync(file);
        }

        /// <summary>
        /// SaveAndGetIdAsync.
        /// </summary>
        /// <param name="file">Parâmetro file.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<Guid> SaveAndGetIdAsync(BinaryObject file)
        {
            return _binaryObjectRepository.InsertAndGetIdAsync(file);
        }
    }
}