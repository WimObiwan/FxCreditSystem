using System;

namespace FxCreditSystem.API.DTO
{
    /// <summary>
    ///   Link between account and user 
    /// </summary>
    public class AccountResponse
    {
        /// <summary>
        ///   Id of account
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        ///   Description of account
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        ///   Minimum allowed credit balance
        /// </summary>
        public decimal MinimumCredits { get; set; }
        /// <summary>
        ///   Current credit balance
        /// </summary>
        public decimal Credits { get; set; }
        /// <summary>
        ///   Last credit change in UTC
        /// </summary>
        public DateTime LastChangeUtc { get; set; }
    }
}
