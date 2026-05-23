using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Friendships.Dto
{
    /// <summary>
    /// Representa a classe CreateFriendshipRequestByUserNameInput.
    /// </summary>
    public class CreateFriendshipRequestByUserNameInput
    {
        [Required(AllowEmptyStrings = true)]
        public string TenancyName { get; set; }

        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}