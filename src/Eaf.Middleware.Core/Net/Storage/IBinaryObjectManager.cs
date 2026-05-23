using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Storage
{
    /// <summary>
    /// Representa a interface IBinaryObjectManager.
    /// </summary>
    public interface IBinaryObjectManager
    {
        Task DeleteAsync(Guid id);

        Task<BinaryObject> GetOrNullAsync(Guid id);

        Task<BinaryObject> GetOrNullAsync(string fileName);

        Task SaveAsync(BinaryObject file);

        Task<Guid> SaveAndGetIdAsync(BinaryObject file);
    }
}