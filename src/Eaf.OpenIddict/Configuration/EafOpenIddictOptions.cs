using System;
using System.Collections.Generic;

namespace Eaf.OpenIddict.Configuration
{
    /// <summary>
    /// Opções de configuração do servidor OpenIddict do EAF, vinculadas à seção
    /// <c>OpenIddict</c> de <c>appsettings.json</c>.
    /// </summary>
    public class EafOpenIddictOptions
    {
        /// <summary>Seção de configuração padrão.</summary>
        public const string SectionName = "OpenIddict";

        /// <summary>
        /// Issuer (iss) emitido pelo OpenIddict. Deve ser uma URI absoluta
        /// (ex.: <c>https://localhost:44301/</c>). Para que os tokens também sejam
        /// aceitos pelo JwtBearer existente, adicione esse valor em
        /// <c>Authentication:JwtBearer:AdditionalIssuers</c>.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Recurso (aud) embutido nos access tokens emitidos pelo endpoint
        /// <c>/connect/token</c>. Deve coincidir com
        /// <c>Authentication:JwtBearer:Audience</c>.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>Validade do access token em minutos (padrão: 60).</summary>
        public int AccessTokenLifetimeMinutes { get; set; } = 60;

        /// <summary>Validade do authorization code em minutos (padrão: 5).</summary>
        public int AuthorizationCodeLifetimeMinutes { get; set; } = 5;

        /// <summary>Aplicações (clients) semeados na inicialização.</summary>
        public List<EafOpenIddictApplicationConfig> Applications { get; set; } = new();
    }

    /// <summary>
    /// Configuração de uma aplicação (client) OpenIddict registrada via
    /// <c>appsettings.json</c>.
    /// </summary>
    public class EafOpenIddictApplicationConfig
    {
        /// <summary>Identificador público do client (obrigatório).</summary>
        public string ClientId { get; set; }

        /// <summary>Segredo do client (obrigatório para clients confidenciais).</summary>
        public string ClientSecret { get; set; }

        /// <summary>Nome de exibição.</summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Tipo de consentimento (<c>explicit</c>, <c>implicit</c> ou
        /// <c>systematic</c>). Padrão: <c>implicit</c>.
        /// </summary>
        public string ConsentType { get; set; } = "implicit";

        /// <summary>URIs de redirecionamento permitidas (authorization code flow).</summary>
        public List<string> RedirectUris { get; set; } = new();

        /// <summary>URIs de redirecionamento pós-logout.</summary>
        public List<string> PostLogoutRedirectUris { get; set; } = new();

        /// <summary>
        /// Permissões OpenIddict do client (ex.: <c>ept:token</c>,
        /// <c>gt:password</c>, <c>scp:default-api</c>). Quando vazio, recebe
        /// o conjunto padrão least-privilege (token endpoint + password +
        /// client_credentials + escopo <c>default-api</c>).
        /// </summary>
        public List<string> Permissions { get; set; } = new();
    }
}
