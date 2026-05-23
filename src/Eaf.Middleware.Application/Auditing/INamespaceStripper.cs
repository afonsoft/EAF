namespace Eaf.Middleware.Auditing
{
    /// <summary>
    /// Representa a interface INamespaceStripper.
    /// </summary>
    public interface INamespaceStripper
    {
        string StripNameSpace(string serviceName);
    }
}