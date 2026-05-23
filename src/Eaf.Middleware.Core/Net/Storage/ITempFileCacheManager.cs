using Abp.Dependency;

namespace Eaf.Middleware.Storage
{
    /// <summary>
    /// Representa a interface ITempFileCacheManager.
    /// </summary>
    public interface ITempFileCacheManager : ITransientDependency
    {
        byte[] GetFile(string token);

        void SetFile(string token, byte[] content);
    }
}