using System;

namespace FxCreditSystem.API.DTO
{
    /// <summary>
    ///   Link between account and user 
    /// </summary>
    public class AccountUserResponse
    {
        /// <summary>
        ///   Id of account
        /// </summary>
        public Guid AccountId { get; set; }
        /// <summary>
        ///   Description of account
        /// </summary>
        public string AccountDescription { get; set; }
        /// <summary>
        ///   Id of user
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        ///   Description of user
        /// </summary>
        public string UserDescription { get; set; }
    }
}
