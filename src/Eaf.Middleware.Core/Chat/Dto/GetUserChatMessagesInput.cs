namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Representa a classe GetUserChatMessagesInput.
    /// </summary>
    public class GetUserChatMessagesInput
    {
        public long? MinMessageId { get; set; }
        public int? TenantId { get; set; }

        public long? UserId { get; set; }

        public long? GroupId { get; set; }
    }
}