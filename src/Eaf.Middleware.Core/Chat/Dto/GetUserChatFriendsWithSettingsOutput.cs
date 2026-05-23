using Castle.Components.DictionaryAdapter;
using Eaf.Middleware.Friendships.Dto;
using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Representa a classe GetUserChatFriendsWithSettingsOutput.
    /// </summary>
    public class GetUserChatFriendsWithSettingsOutput
    {
        /// <summary>
        /// GetUserChatFriendsWithSettingsOutput.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public GetUserChatFriendsWithSettingsOutput()
        {
            Friends = new EditableList<FriendshipDto>();
        }

        public List<FriendshipDto> Friends { get; set; }
        /// <summary>
        /// Obtém ou define ServerTime.
        /// </summary>
        public DateTime ServerTime { get; set; }
    }
}