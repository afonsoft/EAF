namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe IsTenantAvailableOutput.
    /// </summary>
    public class IsTenantAvailableOutput
    {
        /// <summary>
        /// IsTenantAvailableOutput.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public IsTenantAvailableOutput()
        {
        }

        /// <summary>
        /// IsTenantAvailableOutput.
        /// </summary>
        /// <param name="state">Parâmetro state.</param>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public IsTenantAvailableOutput(TenantAvailabilityState state, int? tenantId = null)
        {
            State = state;
            TenantId = tenantId;
        }

        /// <summary>
        /// IsTenantAvailableOutput.
        /// </summary>
        /// <param name="state">Parâmetro state.</param>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <param name="serverRootAddress">Parâmetro serverRootAddress.</param>
        /// <returns>Resultado da operação.</returns>
        public IsTenantAvailableOutput(TenantAvailabilityState state, int? tenantId, string serverRootAddress)
        {
            State = state;
            TenantId = tenantId;
            ServerRootAddress = serverRootAddress;
        }

        /// <summary>
        /// Obtém ou define ServerRootAddress.
        /// </summary>
        public string ServerRootAddress { get; set; }
        /// <summary>
        /// Obtém ou define State.
        /// </summary>
        public TenantAvailabilityState State { get; set; }

        public int? TenantId { get; set; }
    }
}