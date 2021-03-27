using System;

namespace FxCreditSystem.API.DTO
{
    /// <summary>
    ///   Identity of a user 
    /// </summary>
   public class UserIdentityResponse
    {
        /// <summary>
        ///   Id of a user 
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        ///   Identity of a user 
        /// </summary>
        public string Identity { get; set; }
    }
}
