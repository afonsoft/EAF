namespace Eaf.AspNetCore.SignalR.Chat
{
    /// <summary>
    /// Representa a classe SendFriendshipRequestInput.
    /// </summary>
    public class SendFriendshipRequestInput
    {
        public int? TenantId { get; set; }
        /// <summary>
        /// Obtém ou define UserId.
        /// </summary>
        public long UserId { get; set; }
    }
}