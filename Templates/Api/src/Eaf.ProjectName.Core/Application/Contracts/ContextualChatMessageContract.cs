using System;

namespace Eaf.ProjectName.Contracts
{
    /// <summary>
    /// Representa metadata opcional para uma mensagem de chat contextual.
    /// </summary>
    public class ContextualChatMessageContract
    {
        /// <summary>
        /// Obtém ou define a conversa lógica da mensagem.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Obtém ou define o jogo associado à mensagem.
        /// </summary>
        public Guid? GameId { get; set; }

        /// <summary>
        /// Obtém ou define a partida associada à mensagem.
        /// </summary>
        public Guid? MatchId { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de contexto, por exemplo <c>game</c> ou <c>match</c>.
        /// </summary>
        public string ContextType { get; set; }

        /// <summary>
        /// Obtém ou define o identificador fornecido pelo cliente para idempotência.
        /// </summary>
        public string ClientMessageId { get; set; }
    }
}
