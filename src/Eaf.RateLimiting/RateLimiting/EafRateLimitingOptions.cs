using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Eaf.RateLimiting
{
    /// <summary>
    /// Opções do módulo Eaf.RateLimiting.
    /// </summary>
    [Serializable]
    public class EafRateLimitingOptions : IOptions<EafRateLimitingOptions>
    {
        /// <summary>
        /// Regras de limite avaliadas na ordem; a primeira que casa com o endpoint vence.
        /// </summary>
        public List<EafRateLimitRule> Rules { get; set; } = new();

        /// <summary>
        /// Quando true, usuários autenticados são limitados por `AbpSession.UserId`;
        /// quando false ou anônimo, por endereço IP.
        /// </summary>
        public bool LimitByUser { get; set; } = true;

        /// <summary>Obtém o valor das opções.</summary>
        public EafRateLimitingOptions Value => this;
    }

    /// <summary>
    /// Regra de rate limit para um padrão de endpoint.
    /// </summary>
    [Serializable]
    public class EafRateLimitRule
    {
        /// <summary>
        /// Padrão de endpoint (ex.: <c>/api/auth/*</c> ou <c>*</c> para global).
        /// </summary>
        public string Endpoint { get; set; } = "*";

        /// <summary>
        /// Período da janela no formato <c>Ns</c>, <c>Nm</c>, <c>Nh</c> ou <c>Nd</c> (ex.: <c>1m</c>).
        /// </summary>
        public string Period { get; set; } = "1m";

        /// <summary>
        /// Número máximo de requisições permitidas no período.
        /// </summary>
        public long Limit { get; set; }

        /// <summary>
        /// Converte <see cref="Period"/> em <see cref="TimeSpan"/>.
        /// </summary>
        public TimeSpan GetWindow()
        {
            if (string.IsNullOrWhiteSpace(Period) || Period.Length < 2)
            {
                throw new Abp.UI.UserFriendlyException($"Período de rate limit inválido: '{Period}'.");
            }

            var value = int.Parse(Period[..^1]);
            return Period[^1] switch
            {
                's' => TimeSpan.FromSeconds(value),
                'm' => TimeSpan.FromMinutes(value),
                'h' => TimeSpan.FromHours(value),
                'd' => TimeSpan.FromDays(value),
                _ => throw new Abp.UI.UserFriendlyException($"Unidade de período inválida em '{Period}' (use s, m, h ou d).")
            };
        }
    }
}
