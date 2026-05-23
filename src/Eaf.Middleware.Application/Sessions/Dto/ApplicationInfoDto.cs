using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Sessions.Dto
{
    /// <summary>
    /// Representa a classe ApplicationInfoDto.
    /// </summary>
    public class ApplicationInfoDto
    {
        /// <summary>
        /// Obtém ou define Currency.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Obtém ou define CurrencySign.
        /// </summary>
        public string CurrencySign { get; set; }
        public Dictionary<string, bool> Features { get; set; }
        /// <summary>
        /// Obtém ou define ReleaseDate.
        /// </summary>
        public DateTime ReleaseDate { get; set; }
        /// <summary>
        /// Obtém ou define TwoFactorCodeExpireSeconds.
        /// </summary>
        public double TwoFactorCodeExpireSeconds { get; set; }
        /// <summary>
        /// Obtém ou define Version.
        /// </summary>
        public string Version { get; set; }
    }
}