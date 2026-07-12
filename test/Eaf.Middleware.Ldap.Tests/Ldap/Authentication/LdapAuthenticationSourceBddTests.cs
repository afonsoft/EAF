using Abp;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Eaf.Middleware.Ldap.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using Novell.Directory.Ldap;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests.Ldap.Authentication
{
    public class LdapAuthenticationSourceBddTests
    {
        private static TestableLdapAuthenticationSource CriarSut(
            bool isEnabled = true,
            bool settingsIsEnabled = true,
            string? domain = null,
            string? container = null,
            string? userName = null,
            string? password = null,
            object? contextType = null)
        {
            var settings = Substitute.For<ILdapSettings>();
            var ldapModuleConfig = Substitute.For<IEafMiddlewareLdapModuleConfig>();

            ldapModuleConfig.IsEnabled.Returns(isEnabled);
            settings.GetIsEnabled(Arg.Any<int?>()).Returns(settingsIsEnabled);

            settings.GetDomain(Arg.Any<int?>()).Returns(Task.FromResult(Cifrar(domain ?? "localhost")));
            settings.GetContainer(Arg.Any<int?>()).Returns(Task.FromResult(Cifrar(container ?? string.Empty)));
            settings.GetUserName(Arg.Any<int?>()).Returns(Task.FromResult(Cifrar(userName ?? string.Empty)));
            settings.GetPassword(Arg.Any<int?>()).Returns(Task.FromResult(Cifrar(password ?? string.Empty)));
            settings.GetContextType(Arg.Any<int?>()).Returns(Task.FromResult(contextType ?? new object()));

            return new TestableLdapAuthenticationSource(settings, ldapModuleConfig);
        }

        private static string Cifrar(string? value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : SimpleStringCipher.Instance.Encrypt(value);
        }

        private static ILdapSearchResults CriarSearchResults(params LdapEntry[] entries)
        {
            var search = Substitute.For<ILdapSearchResults>();
            var queue = new Queue<LdapEntry>(entries);
            search.HasMoreAsync().Returns(callInfo => queue.Count > 0);
            search.NextAsync().Returns(callInfo =>
            {
                if (queue.Count == 0)
                    throw new LdapException("No more entries", 1, "No more entries");
                return queue.Dequeue();
            });
            return search;
        }

        private static LdapEntry CriarLdapEntry(string cn, string? samAccountName = null, string? displayName = null, string? mail = null, string? userPrincipalName = null)
        {
            var attributeSet = new LdapAttributeSet
            {
                new LdapAttribute("SamAccountName", samAccountName ?? cn),
                new LdapAttribute("DisplayName", displayName ?? cn),
                new LdapAttribute("mail", mail ?? string.Empty),
                new LdapAttribute("UserPrincipalName", userPrincipalName ?? string.Empty)
            };
            return new LdapEntry($"CN={cn},DC=example,DC=com", attributeSet);
        }

        [Fact]
        public void Dado_LdapAuthenticationSource_Quando_ObterNome_Entao_DeveRetornarLDAP()
        {
            var sut = CriarSut();
            sut.Name.ShouldBe("LDAP");
        }

        [Fact]
        public void Dado_LdapAuthenticationSource_Quando_ObterSourceName_Entao_DeveSerLDAP()
        {
            LdapAuthenticationSource<TestTenant, TestUser>.SourceName.ShouldBe("LDAP");
        }

        [Fact]
        public async Task Dado_ModuloDesabilitado_Quando_CreateUserAsync_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut(isEnabled: false);
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateUserAsync("user", new TestTenant()));
            ex.Message.ShouldContain("Ldap Authentication module is disabled globally");
        }

        [Fact]
        public async Task Dado_ConfigDesabilitada_Quando_CreateUserAsync_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut(settingsIsEnabled: false);
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateUserAsync("user", new TestTenant()));
            ex.Message.ShouldContain("Ldap Authentication is disabled for given tenant");
        }

        [Fact]
        public async Task Dado_UsuarioComEmail_Quando_CreateUserAsync_Entao_DeveRemoverDominioDoEmailELancarExcecao()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = new LdapConnection();
            await Should.ThrowAsync<Exception>(async () => await sut.CreateUserAsync("user@example.com", new TestTenant()));
            sut.LastUserNameOrEmailAddress.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_CreateLdapContextFalhando_Quando_CreateUserAsync_Entao_DeveLancarExcecao()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = null!;
            await Should.ThrowAsync<Exception>(async () => await sut.CreateUserAsync("user", new TestTenant()));
        }

        [Fact]
        public async Task Dado_ModuloDesabilitado_Quando_UpdateUserAsync_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut(isEnabled: false);
            var user = new TestUser { UserName = "user", EmailAddress = "user@example.com" };
            await Should.ThrowAsync<AbpException>(async () => await sut.UpdateUserAsync(user, new TestTenant()));
        }

        [Fact]
        public async Task Dado_LdapContextInvalido_Quando_UpdateUserAsync_Entao_DeveCapturarExcecaoELogar()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            var user = new TestUser { UserName = "user", EmailAddress = "user@example.com" };
            await Should.NotThrowAsync(async () => await sut.UpdateUserAsync(user, new TestTenant()));
            user.UserName.ShouldBe("user");
        }

        [Fact]
        public async Task Dado_UserNameVazio_Quando_GetUsersAsync_Entao_DeveRetornarListaVazia()
        {
            var sut = CriarSut();
            var result = await sut.GetUsersAsync(string.Empty);
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Dado_LdapContextInvalido_Quando_GetUsersAsync_Entao_DeveLancarExcecao()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<bool>()).ReturnsNull();
            await Should.ThrowAsync<Exception>(async () => await sut.GetUsersAsync("user"));
        }

        [Fact]
        public async Task Dado_ModuloDesabilitado_Quando_TryAuthenticateAsync_Entao_DeveRetornarFalse()
        {
            var sut = CriarSut(isEnabled: false);
            var result = await sut.TryAuthenticateAsync("user", "pass", new TestTenant());
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_ConfigDesabilitada_Quando_TryAuthenticateAsync_Entao_DeveRetornarFalse()
        {
            var sut = CriarSut(settingsIsEnabled: false);
            var result = await sut.TryAuthenticateAsync("user", "pass", new TestTenant());
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_UsuarioComEmail_Quando_TryAuthenticateAsync_Entao_DeveRemoverDominioDoEmail()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = new LdapConnection();
            var result = await sut.TryAuthenticateAsync("user@example.com", "pass", new TestTenant());
            result.ShouldBeFalse();
            sut.LastUserNameOrEmailAddress.ShouldBe("user");
            sut.LastPassword.ShouldBe("pass");
        }

        [Fact]
        public async Task Dado_LdapNaoConectado_Quando_TryAuthenticateAsync_Entao_DeveRetornarFalse()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = new LdapConnection();
            var result = await sut.TryAuthenticateAsync("user", "pass", new TestTenant());
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_CreateLdapContextLancandoExcecao_Quando_TryAuthenticateAsync_Entao_DevePropagar()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = null!;
            await Should.ThrowAsync<Exception>(async () => await sut.TryAuthenticateAsync("user", "pass", new TestTenant()));
        }

        [Fact]
        public async Task Dado_ParametrosValidos_Quando_CreateLdapContext_Entao_DeveTentarConectarELancarAbpException()
        {
            var sut = CriarSut();
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ContainerComPonto_Quando_CreateLdapContext_Entao_DeveConverterParaDcFormat()
        {
            var sut = CriarSut(container: "example.com", domain: "localhost");
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UserNameSemDominio_Quando_CreateLdapContext_Entao_DevePrefixarComDominio()
        {
            var sut = CriarSut(domain: "example");
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UserNameComBackslash_Quando_CreateLdapContext_Entao_NaoDevePrefixar()
        {
            var sut = CriarSut(domain: "example");
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "domain\\user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_DominioComPonto_Quando_CreateLdapContext_Entao_NaoDevePrefixarUserName()
        {
            var sut = CriarSut(domain: "example.com");
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_DominioComDC_Quando_CreateLdapContext_Entao_NaoDevePrefixarUserName()
        {
            var sut = CriarSut(domain: "DC=example,DC=com");
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ContainerVazioComDominioComPonto_Quando_CreateLdapContext_Entao_DeveTransformarContainer()
        {
            var sut = CriarSut(domain: "example.com", container: string.Empty);
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ContainerComDC_Quando_CreateLdapContext_Entao_DeveManterContainer()
        {
            var sut = CriarSut(domain: "localhost", container: "DC=example,DC=com");
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "user", "pass"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UserNameEPasswordVazios_Quando_CreateLdapContext_Entao_DeveUsarConfiguracoes()
        {
            var sut = CriarSut(userName: "admin", password: "secret");
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, null, null));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UserNameEPasswordFornecidos_Quando_CreateLdapContext_Entao_DeveUsarParametros()
        {
            var sut = CriarSut();
            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.CreateLdapContextBaseAsync(null, "admin", "secret"));
            ex.Message.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_SistemaLinux_Quando_CreatePrincipalContext_Entao_DeveLancarNotImplementedException()
        {
            var sut = CriarSut();
            await Should.ThrowAsync<NotImplementedException>(async () => await sut.CreatePrincipalContextBaseAsync(new TestTenant()));
        }

        [Fact]
        public async Task Dado_FillUsersLdapNulo_Quando_Executar_Entao_DeveLancarExcecao()
        {
            var sut = CriarSut();
            await Should.ThrowAsync<Exception>(async () => await sut.InvokeFillUsersLdapAsync(null!));
        }

        [Fact]
        public async Task Dado_FillUsersLdapComResultado_Quando_Executar_Entao_DeveRetornarUsuariosConvertidos()
        {
            var sut = CriarSut();
            var entry = CriarLdapEntry("john.doe", "jdoe", "John Doe", "john@example.com", "john@example.com");
            var search = CriarSearchResults(entry);
            var result = await sut.InvokeFillUsersLdapAsync(search);
            result.Item1.Count.ShouldBe(1);
            result.Item1[0].UserName.ShouldBe("jdoe");
            result.Item1[0].Name.ShouldBe("John");
            result.Item1[0].EmailAddress.ShouldBe("john@example.com");
        }

        [Fact]
        public async Task Dado_FillUsersLdapSemMail_Quando_Executar_Entao_DeveUsarUserPrincipalName()
        {
            var sut = CriarSut();
            var entry = CriarLdapEntry("john.doe", "jdoe", "John Doe", mail: null, userPrincipalName: "john@example.com");
            var search = CriarSearchResults(entry);
            var result = await sut.InvokeFillUsersLdapAsync(search);
            result.Item1[0].EmailAddress.ShouldBe("john@example.com");
        }

        [Fact]
        public async Task Dado_FillUsersLdapComNextLancandoExcecao_Quando_Executar_Entao_DeveContinuarProcessando()
        {
            var sut = CriarSut();
            var search = Substitute.For<ILdapSearchResults>();
            search.HasMoreAsync().Returns(true, false);
            search.NextAsync().Returns(callInfo => Task.FromException<LdapEntry>(new LdapException("error", 1, "error")));
            var result = await sut.InvokeFillUsersLdapAsync(search);
            result.Item1.ShouldBeEmpty();
            result.Item2.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_UsuarioPrincipal_Quando_UpdateUserFromLdap_Entao_DeveAtualizarUsuario()
        {
            var sut = CriarSut();
            var user = new TestUser { UserName = "OLD", Name = "Old", Surname = "Old", EmailAddress = "OLD@EXAMPLE.COM" };
            var principal = new TestUser { UserName = "NEW", Name = "New", Surname = "New", EmailAddress = "NEW@EXAMPLE.COM" };
            sut.UpdateUserFromLdapPublic(user, principal);
            user.UserName.ShouldBe("new");
            user.Name.ShouldBe("New");
            user.Surname.ShouldBe("New");
            user.EmailAddress.ShouldBe("new@example.com");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Dado_UsuarioPrincipalNulo_Quando_UpdateUserFromLdap_Entao_DeveManterUsuarioOriginal()
        {
            var sut = CriarSut();
            var user = new TestUser { UserName = "user", Name = "Name", Surname = "Surname", EmailAddress = "USER@EXAMPLE.COM" };
            sut.UpdateUserFromLdapPublic(user, null!);
            user.UserName.ShouldBe("user");
            user.Name.ShouldBe("Name");
            user.Surname.ShouldBe("Surname");
            user.EmailAddress.ShouldBe("user@example.com");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Dado_SistemaLinux_Quando_UpdateUserFromPrincipal_Entao_DeveRetornarSemAlterar()
        {
            var sut = CriarSut();
            var user = new TestUser { UserName = "USER", Name = "Name", EmailAddress = "user@example.com" };
            sut.UpdateUserFromPrincipalPublic(user, null!);
            user.UserName.ShouldBe("USER");
            user.Name.ShouldBe("Name");
        }

        [Fact]
        public void Dado_SistemaLinux_Quando_ValidateCredentials_Entao_DeveRetornarFalse()
        {
            var sut = CriarSut();
            var result = sut.ValidateCredentialsPublic(null!, "user", "pass");
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("   ", null)]
        [InlineData("value", "value")]
        public void Dado_Valores_Quando_ConvertToNullIfEmpty_Entao_DeveRetornarEsperado(string? input, string? expected)
        {
            TestableLdapAuthenticationSource.ConvertToNullIfEmptyPublic(input).ShouldBe(expected);
        }

        [Fact]
        public async Task Dado_LdapContextComResultado_Quando_CreateUserAsync_Entao_DeveAtualizarUsuario()
        {
            var entry = CriarLdapEntry("john.doe", "jdoe", "John Doe", "john@example.com", "john@example.com");
            var sut = CriarSut(domain: "example.com");
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<bool>()).Returns(callInfo => CriarSearchResults(entry));

            var user = await sut.CreateUserAsync("user@example.com", new TestTenant());

            user.ShouldNotBeNull();
            user.UserName.ShouldBe("jdoe");
            user.Name.ShouldBe("John");
            user.EmailAddress.ShouldBe("john@example.com");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_LdapContextComResultado_Quando_UpdateUserAsync_Entao_DeveAtualizarUsuario()
        {
            var entry = CriarLdapEntry("john.doe", "jdoe", "John Doe", "john@example.com", "john@example.com");
            var sut = CriarSut(domain: "example.com");
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<bool>()).Returns(callInfo => CriarSearchResults(entry));

            var user = new TestUser { UserName = "olduser", Name = "Old", Surname = "Surname", EmailAddress = "old@example.com" };
            await sut.UpdateUserAsync(user, new TestTenant());

            user.UserName.ShouldBe("jdoe");
            user.Name.ShouldBe("John");
            user.EmailAddress.ShouldBe("john@example.com");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_LdapContextComResultado_Quando_GetUsersAsync_Entao_DeveRetornarUsuarios()
        {
            var entry = CriarLdapEntry("john.doe", "jdoe", "John Doe", "john@example.com", "john@example.com");
            var sut = CriarSut(domain: "example.com");
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<bool>()).Returns(callInfo => CriarSearchResults(entry));

            var users = await sut.GetUsersAsync("john@example.com");

            users.ShouldNotBeEmpty();
            users[0].UserName.ShouldBe("jdoe");
            users[0].Name.ShouldBe("John");
            users[0].EmailAddress.ShouldBe("john@example.com");
        }

        [Fact]
        public async Task Dado_LdapConectado_Quando_TryAuthenticateAsync_Entao_DeveRetornarTrue()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.Connected.Returns(true);

            var result = await sut.TryAuthenticateAsync("user", "pass", new TestTenant());

            result.ShouldBeTrue();
            sut.LastUserNameOrEmailAddress.ShouldBe("user");
            sut.LastPassword.ShouldBe("pass");
        }

        [Fact]
        public async Task Dado_LdapConectadoComEmail_Quando_TryAuthenticateAsync_Entao_DeveRemoverDominio()
        {
            var sut = CriarSut();
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.Connected.Returns(true);

            var result = await sut.TryAuthenticateAsync("user@example.com", "pass", new TestTenant());

            result.ShouldBeTrue();
            sut.LastUserNameOrEmailAddress.ShouldBe("user");
            sut.LastPassword.ShouldBe("pass");
        }

        [Fact]
        public async Task Dado_LdapContextSemResultado_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioSemAlterar()
        {
            var sut = CriarSut(domain: "example.com");
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<bool>()).Returns(callInfo => CriarSearchResults());

            var user = await sut.CreateUserAsync("user@example.com", new TestTenant());

            user.ShouldNotBeNull();
            user.UserName.ShouldBe("user");
        }

        [Fact]
        public async Task Dado_LdapContextSemResultado_Quando_UpdateUserAsync_Entao_DeveManterUsuarioOriginal()
        {
            var sut = CriarSut(domain: "example.com");
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<bool>()).Returns(callInfo => CriarSearchResults());

            var user = new TestUser { UserName = "olduser", Name = "Old", Surname = "Surname", EmailAddress = "old@example.com" };
            await sut.UpdateUserAsync(user, new TestTenant());

            user.UserName.ShouldBe("olduser");
            user.Name.ShouldBe("Old");
            user.EmailAddress.ShouldBe("old@example.com");
        }

        [Fact]
        public async Task Dado_LdapContextComErroNoResultado_Quando_GetUsersAsync_Entao_DeveLancarAggregateException()
        {
            var sut = CriarSut(domain: "example.com");
            sut.LdapContextToReturn = Substitute.For<ILdapConnection>();
            sut.LdapContextToReturn.SearchAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<bool>()).Returns(callInfo => CriarSearchResultsComErro());

            var ex = await Should.ThrowAsync<AggregateException>(async () => await sut.GetUsersAsync("user@example.com"));
            ex.InnerExceptions.ShouldNotBeEmpty();
        }

        private static ILdapSearchResults CriarSearchResultsComErro()
        {
            var search = Substitute.For<ILdapSearchResults>();
            search.HasMoreAsync().Returns(true, false);
            search.NextAsync().Returns(x => (LdapEntry)null!);
            return search;
        }

        public class TestTenant : AbpTenant<TestUser>
        {
            public TestTenant()
                : base("Default", "Default")
            {
            }
        }

        public class TestUser : AbpUserBase
        {
        }

        public class TestableLdapAuthenticationSource : LdapAuthenticationSource<TestTenant, TestUser>
        {
            public TestableLdapAuthenticationSource(ILdapSettings settings, IEafMiddlewareLdapModuleConfig ldapModuleConfig)
                : base(settings, ldapModuleConfig)
            {
                LdapContextToReturn = new LdapConnection();
            }

            public ILdapConnection LdapContextToReturn { get; set; }
            public PrincipalContext? PrincipalContextToReturn { get; set; }
            public string? LastUserNameOrEmailAddress { get; set; }
            public string? LastPassword { get; set; }

            protected override Task<ILdapConnection> CreateLdapContext(TestTenant tenant, string? userNameOrEmailAddress, string? plainPassword)
            {
                LastUserNameOrEmailAddress = userNameOrEmailAddress;
                LastPassword = plainPassword;
                return Task.FromResult(LdapContextToReturn);
            }

            protected override Task<PrincipalContext> CreatePrincipalContext(TestTenant tenant, string? userNameOrEmailAddress, string? plainPassword)
            {
                return Task.FromResult(PrincipalContextToReturn ?? null!);
            }

            public async Task<LdapConnection> CreateLdapContextBaseAsync(TestTenant? tenant, string? userNameOrEmailAddress = null, string? plainPassword = null)
            {
#pragma warning disable CS8604
                return (LdapConnection)(await base.CreateLdapContext(tenant!, userNameOrEmailAddress, plainPassword));
#pragma warning restore CS8604
            }

            public async Task<PrincipalContext> CreatePrincipalContextBaseAsync(TestTenant? tenant, string? userNameOrEmailAddress = null, string? plainPassword = null)
            {
#pragma warning disable CS8604
                return await base.CreatePrincipalContext(tenant!, userNameOrEmailAddress, plainPassword);
#pragma warning restore CS8604
            }

            public async Task CheckIsEnabledPublic(TestTenant? tenant)
            {
                await base.CheckIsEnabled(tenant!);
            }

            public void UpdateUserFromLdapPublic(TestUser user, TestUser? userPrincipal)
            {
                base.UpdateUserFromLdap(user, userPrincipal!);
            }

            public void UpdateUserFromPrincipalPublic(TestUser user, UserPrincipal? userPrincipal)
            {
                base.UpdateUserFromPrincipal(user, userPrincipal!);
            }

            public bool ValidateCredentialsPublic(PrincipalContext? principalContext, string userNameOrEmailAddress, string plainPassword)
            {
                return base.ValidateCredentials(principalContext!, userNameOrEmailAddress, plainPassword);
            }

            public static string? ConvertToNullIfEmptyPublic(string? str)
            {
                return ConvertToNullIfEmpty(str!);
            }

            public async Task<Tuple<List<TestUser>, List<Exception>>> InvokeFillUsersLdapAsync(ILdapSearchResults search)
            {
                var method = typeof(LdapAuthenticationSource<TestTenant, TestUser>).GetMethod("FillUsersLdap", BindingFlags.NonPublic | BindingFlags.Instance);
                var result = method!.Invoke(this, new object[] { search });
                return await (Task<Tuple<List<TestUser>, List<Exception>>>)result!;
            }
        }
    }
}
