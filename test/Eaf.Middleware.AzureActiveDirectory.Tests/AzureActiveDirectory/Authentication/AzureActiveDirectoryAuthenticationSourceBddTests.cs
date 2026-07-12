using Abp;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using Eaf.Middleware.AzureActiveDirectory.Authentication;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests.AzureActiveDirectory.Authentication
{
    public class AzureActiveDirectoryAuthenticationSourceBddTests
    {
        private static TestableAzureActiveDirectoryAuthenticationSource CriarSut(
            bool isEnabled = true,
            bool settingsIsEnabled = true,
            string tenant = "tenant.onmicrosoft.com",
            string? clientId = null,
            string? clientSecret = null)
        {
            var settings = Substitute.For<IAzureActiveDirectorySettings>();
            var moduleConfig = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();

            moduleConfig.IsEnabled.Returns(isEnabled);
            settings.GetIsEnabled().Returns(settingsIsEnabled);
            settings.GetTenant().Returns(Task.FromResult(tenant));
            settings.GetClientId().Returns(Task.FromResult(Cifrar(clientId ?? "12345678-1234-1234-1234-123456789012")));
            settings.GetClientSecret().Returns(Task.FromResult(Cifrar(clientSecret ?? "secret")));

            return new TestableAzureActiveDirectoryAuthenticationSource(settings, moduleConfig);
        }

        private static string Cifrar(string? value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : SimpleStringCipher.Instance.Encrypt(value);
        }

        private static IGraphServiceUsersCollectionRequestBuilder CriarGraphBuilderComUser(User user)
        {
            var page = Substitute.For<IGraphServiceUsersCollectionPage>();
            var list = new List<User> { user };
            page.Count.Returns(1);
            page[0].Returns(user);
            page.GetEnumerator().Returns(callInfo => list.GetEnumerator());

            var request = Substitute.For<IGraphServiceUsersCollectionRequest>();
            request.Filter(Arg.Any<string>()).Returns(request);
            request.Top(Arg.Any<int>()).Returns(request);
            request.GetAsync().Returns(Task.FromResult(page));

            var builder = Substitute.For<IGraphServiceUsersCollectionRequestBuilder>();
            builder.Request().Returns(request);
            return builder;
        }

        private static IGraphServiceUsersCollectionRequestBuilder CriarGraphBuilderVazio()
        {
            var page = Substitute.For<IGraphServiceUsersCollectionPage>();
            var list = new List<User>();
            page.Count.Returns(0);
            page.GetEnumerator().Returns(callInfo => list.GetEnumerator());

            var request = Substitute.For<IGraphServiceUsersCollectionRequest>();
            request.Filter(Arg.Any<string>()).Returns(request);
            request.Top(Arg.Any<int>()).Returns(request);
            request.GetAsync().Returns(Task.FromResult(page));

            var builder = Substitute.For<IGraphServiceUsersCollectionRequestBuilder>();
            builder.Request().Returns(request);
            return builder;
        }

        private static User CriarUser(string principalName, string displayName, string surname, string? mail = null)
        {
            return new User
            {
                UserPrincipalName = principalName,
                DisplayName = displayName,
                Surname = surname,
                Mail = mail
            };
        }

        [Fact]
        public void Dado_AzureActiveDirectoryAuthenticationSource_Quando_ObterNome_Entao_DeveRetornarAzureActiveDirectory()
        {
            var sut = CriarSut();
            sut.Name.ShouldBe("ActiveDirectory");
        }

        [Fact]
        public async Task Dado_ModuloDesabilitado_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioBase()
        {
            var sut = CriarSut(isEnabled: false);
            var user = await sut.CreateUserAsync("user", new TestTenant());
            user.ShouldNotBeNull();
            user.UserName.ShouldBe("user");
            user.IsEmailConfirmed.ShouldBeTrue();
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ConfigDesabilitada_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioBase()
        {
            var sut = CriarSut(settingsIsEnabled: false);
            var user = await sut.CreateUserAsync("user", new TestTenant());
            user.ShouldNotBeNull();
            user.UserName.ShouldBe("user");
        }

        [Fact]
        public async Task Dado_UsuarioNoGraph_Quando_CreateUserAsync_Entao_DeveAtualizarComDadosDoAzure()
        {
            var graphUser = CriarUser("user@tenant.onmicrosoft.com", "User Name", "Surname", "user@example.com");
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderComUser(graphUser);

            var user = await sut.CreateUserAsync("user", new TestTenant());
            user.UserName.ShouldBe("user");
            user.Name.ShouldBe("User Name");
            user.Surname.ShouldBe("Surname");
            user.EmailAddress.ShouldBe("user@example.com");
            user.IsEmailConfirmed.ShouldBeTrue();
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_GraphRetornandoVazio_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioBaseAtivo()
        {
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderVazio();

            var user = await sut.CreateUserAsync("user", new TestTenant());
            user.UserName.ShouldBe("user");
            user.IsActive.ShouldBeTrue();
            user.IsEmailConfirmed.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_GraphLancandoMsalException_Quando_CreateUserAsync_Entao_DeveCapturarELogar()
        {
            var sut = CriarSut();
            var request = Substitute.For<IGraphServiceUsersCollectionRequest>();
            request.Filter(Arg.Any<string>()).Returns(request);
            request.GetAsync().Returns(Task.FromException<IGraphServiceUsersCollectionPage>(new MsalException("code", "error")));

            var builder = Substitute.For<IGraphServiceUsersCollectionRequestBuilder>();
            builder.Request().Returns(request);
            sut.UsersRequestBuilderToReturn = builder;

            var user = await sut.CreateUserAsync("user", new TestTenant());
            user.UserName.ShouldBe("user");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ModuloDesabilitado_Quando_GetUsersAsync_Entao_DeveRetornarListaVazia()
        {
            var sut = CriarSut(isEnabled: false);
            var result = await sut.GetUsersAsync("user");
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Dado_UsuarioNoGraph_Quando_GetUsersAsync_Entao_DeveRetornarUsuarios()
        {
            var graphUser = CriarUser("user@tenant.onmicrosoft.com", "User Name", "Surname", "user@example.com");
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderComUser(graphUser);

            var result = await sut.GetUsersAsync("user");
            result.Count.ShouldBe(1);
            result[0].UserName.ShouldBe("user");
            result[0].Name.ShouldBe("User Name");
            result[0].Surname.ShouldBe("Surname");
            result[0].EmailAddress.ShouldBe("user@example.com");
        }

        [Fact]
        public async Task Dado_GraphVazio_Quando_GetUsersAsync_Entao_DeveRetornarListaVazia()
        {
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderVazio();

            var result = await sut.GetUsersAsync("user");
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Dado_UserNameVazio_Quando_GetUsersAsync_Entao_DeveRetornarListaVazia()
        {
            var sut = CriarSut();
            var result = await sut.GetUsersAsync(string.Empty);
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Dado_UsuarioNoGraph_Quando_UpdateUserAsync_Entao_DeveAtualizarUsuario()
        {
            var graphUser = CriarUser("user@tenant.onmicrosoft.com", "New Name", "Surname", "new@example.com");
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderComUser(graphUser);

            var user = new TestUser { UserName = "user", Name = "Old", Surname = "Old", EmailAddress = "old@example.com" };
            await sut.UpdateUserAsync(user, new TestTenant());
            user.UserName.ShouldBe("user");
            user.Name.ShouldBe("New Name");
            user.Surname.ShouldBe("Surname");
            user.EmailAddress.ShouldBe("new@example.com");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_GraphVazio_Quando_UpdateUserAsync_Entao_DeveManterUsuario()
        {
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderVazio();

            var user = new TestUser { UserName = "user", Name = "Old", EmailAddress = "old@example.com" };
            await sut.UpdateUserAsync(user, new TestTenant());
            user.UserName.ShouldBe("user");
            user.Name.ShouldBe("Old");
            user.EmailAddress.ShouldBe("old@example.com");
        }

        [Fact]
        public async Task Dado_GraphLancandoMsalException_Quando_UpdateUserAsync_Entao_DeveCapturarELogar()
        {
            var sut = CriarSut();
            var request = Substitute.For<IGraphServiceUsersCollectionRequest>();
            request.Filter(Arg.Any<string>()).Returns(request);
            request.GetAsync().Returns(Task.FromException<IGraphServiceUsersCollectionPage>(new MsalException("code", "error")));

            var builder = Substitute.For<IGraphServiceUsersCollectionRequestBuilder>();
            builder.Request().Returns(request);
            sut.UsersRequestBuilderToReturn = builder;

            var user = new TestUser { UserName = "user", Name = "Old", EmailAddress = "old@example.com" };
            await sut.UpdateUserAsync(user, new TestTenant());
            user.Name.ShouldBe("Old");
        }

        [Fact]
        public async Task Dado_UserNameVazio_Quando_GetUserAsync_Entao_DeveRetornarNulo()
        {
            var sut = CriarSut();
            var result = await sut.GetUserAsync(string.Empty);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_ModuloDesabilitado_Quando_GetUserAsync_Entao_DeveRetornarNulo()
        {
            var sut = CriarSut(isEnabled: false);
            var result = await sut.GetUserAsync("user");
            result.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioNoGraph_Quando_GetUserAsync_Entao_DeveRetornarUsuario()
        {
            var graphUser = CriarUser("user@tenant.onmicrosoft.com", "User Name", "Surname", "user@example.com");
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderComUser(graphUser);

            var result = await sut.GetUserAsync("user");
            result.ShouldNotBeNull();
            result!.UserName.ShouldBe("user");
            result.Name.ShouldBe("User Name");
            result.Surname.ShouldBe("Surname");
            result.EmailAddress.ShouldBe("user@example.com");
        }

        [Fact]
        public async Task Dado_UsuarioSemMail_Quando_GetUserAsync_Entao_DeveCompuserEmailComTenant()
        {
            var graphUser = CriarUser("user@tenant.onmicrosoft.com", "User Name", "Surname", mail: null);
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderComUser(graphUser);

            var result = await sut.GetUserAsync("user");
            result.ShouldNotBeNull();
            result!.EmailAddress.ShouldBe("user@tenant.onmicrosoft.com");
        }

        [Fact]
        public async Task Dado_GraphVazio_Quando_GetUserAsync_Entao_DeveRetornarUsuarioVazio()
        {
            var sut = CriarSut();
            sut.UsersRequestBuilderToReturn = CriarGraphBuilderVazio();

            var result = await sut.GetUserAsync("user");
            result.ShouldNotBeNull();
            result!.UserName.ShouldBeNull();
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
        public async Task Dado_UsuarioComEmail_Quando_TryAuthenticateAsync_Entao_DeveManterEmail()
        {
            var sut = CriarSut();
            var app = Substitute.For<IPublicClientApplication>();
            app.AcquireTokenByUsernamePassword(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<SecureString>())
                .Returns(callInfo => throw new MsalException("other", "error"));
            sut.PublicClientApplicationToReturn = app;

            var result = await sut.TryAuthenticateAsync("user@tenant.onmicrosoft.com", "pass", new TestTenant());
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_MsalExceptionExpired_Quando_TryAuthenticateAsync_Entao_DeveLancarUserFriendlyException()
        {
            var sut = CriarSut();
            var app = Substitute.For<IPublicClientApplication>();
            app.AcquireTokenByUsernamePassword(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<SecureString>())
                .Returns(callInfo => throw new MsalException("expired", "msg"));
            sut.PublicClientApplicationToReturn = app;

            var ex = await Should.ThrowAsync<UserFriendlyException>(async () => await sut.TryAuthenticateAsync("user", "pass", new TestTenant()));
            ex.Message.ShouldBe("LoginFailed");
        }

        [Fact]
        public async Task Dado_MsalExceptionBlocked_Quando_TryAuthenticateAsync_Entao_DeveLancarUserFriendlyException()
        {
            var sut = CriarSut();
            var app = Substitute.For<IPublicClientApplication>();
            app.AcquireTokenByUsernamePassword(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<SecureString>())
                .Returns(callInfo => throw new MsalException("blocked", "msg"));
            sut.PublicClientApplicationToReturn = app;

            var ex = await Should.ThrowAsync<UserFriendlyException>(async () => await sut.TryAuthenticateAsync("user", "pass", new TestTenant()));
            ex.Message.ShouldBe("LoginFailed");
        }

        [Fact]
        public async Task Dado_MsalExceptionInvalidGrant_Quando_TryAuthenticateAsync_Entao_DeveLancarUserFriendlyException()
        {
            var sut = CriarSut();
            var app = Substitute.For<IPublicClientApplication>();
            app.AcquireTokenByUsernamePassword(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<SecureString>())
                .Returns(callInfo => throw new MsalException("invalid_grant", "msg"));
            sut.PublicClientApplicationToReturn = app;

            var ex = await Should.ThrowAsync<UserFriendlyException>(async () => await sut.TryAuthenticateAsync("user", "pass", new TestTenant()));
            ex.Message.ShouldBe("LoginFailed");
        }

        [Fact]
        public async Task Dado_MsalExceptionComAADS_Quando_TryAuthenticateAsync_Entao_DeveLancarUserFriendlyException()
        {
            var sut = CriarSut();
            var app = Substitute.For<IPublicClientApplication>();
            app.AcquireTokenByUsernamePassword(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<SecureString>())
                .Returns(callInfo => throw new MsalException("code", "AADS error"));
            sut.PublicClientApplicationToReturn = app;

            var ex = await Should.ThrowAsync<UserFriendlyException>(async () => await sut.TryAuthenticateAsync("user", "pass", new TestTenant()));
            ex.Message.ShouldBe("LoginFailed");
        }

        [Fact]
        public async Task Dado_MsalExceptionGenerica_Quando_TryAuthenticateAsync_Entao_DeveRetornarFalse()
        {
            var sut = CriarSut();
            var app = Substitute.For<IPublicClientApplication>();
            app.AcquireTokenByUsernamePassword(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<SecureString>())
                .Returns(callInfo => throw new MsalException("other", "error"));
            sut.PublicClientApplicationToReturn = app;

            var result = await sut.TryAuthenticateAsync("user", "pass", new TestTenant());
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_CheckIsEnabledComModuloDesabilitado_Quando_Executar_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut(isEnabled: false);
            await Should.ThrowAsync<AbpException>(async () => await sut.CheckIsEnabledPublic(new TestTenant()));
        }

        [Fact]
        public async Task Dado_CheckIsEnabledComSettingsDesabilitado_Quando_Executar_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut(settingsIsEnabled: false);
            await Should.ThrowAsync<AbpException>(async () => await sut.CheckIsEnabledPublic(new TestTenant()));
        }

        [Fact]
        public void Dado_UsuarioPrincipal_Quando_UpdateUserFromAzureActiveDirectory_Entao_DeveAtualizarUsuario()
        {
            var sut = CriarSut();
            var user = new TestUser { UserName = "OLD", Name = "Old", Surname = "Old", EmailAddress = "OLD@EXAMPLE.COM" };
            var principal = new TestUser { UserName = "new", Name = "New", Surname = "Surname", EmailAddress = "NEW@EXAMPLE.COM" };

            sut.UpdateUserFromAzureActiveDirectoryPublic(user, principal);

            user.UserName.ShouldBe("new");
            user.Name.ShouldBe("New");
            user.Surname.ShouldBe("Surname");
            user.EmailAddress.ShouldBe("new@example.com");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Dado_UsuarioPrincipalNulo_Quando_UpdateUserFromAzureActiveDirectory_Entao_DeveManterUsuarioOriginal()
        {
            var sut = CriarSut();
            var user = new TestUser { UserName = "user", Name = "Name", Surname = "Surname", EmailAddress = "USER@EXAMPLE.COM" };

            sut.UpdateUserFromAzureActiveDirectoryPublic(user, null!);

            user.UserName.ShouldBe("user");
            user.Name.ShouldBe("Name");
            user.Surname.ShouldBe("Surname");
            user.EmailAddress.ShouldBe("user@example.com");
            user.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ConfiguracoesValidas_Quando_CreateAzureApplication_Entao_DeveRetornarIPublicClientApplication()
        {
            var sut = CriarSut();
            var app = await sut.CreateAzureApplicationPublic();
            app.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ConfiguracoesValidas_Quando_CreateAzureConfidential_Entao_DeveRetornarIConfidentialClientApplication()
        {
            var sut = CriarSut();
            var app = await sut.CreateAzureConfidentialPublic();
            app.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_CreateAzureConfidentialLancandoMsalException_Quando_CreateGraphServiceClient_Entao_DevePropagarMsalException()
        {
            var sut = CriarSut();
            var app = Substitute.For<IConfidentialClientApplication>();
            app.AcquireTokenForClient(Arg.Any<IEnumerable<string>>())
                .Returns(callInfo => throw new MsalException("code", "error"));
            sut.ConfidentialClientApplicationToReturn = app;

            await Should.ThrowAsync<MsalException>(async () => await sut.CreateGraphServiceClientPublic());
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

        public class FakeGraphServiceClient : GraphServiceClient
        {
            private readonly IGraphServiceUsersCollectionRequestBuilder _users;

            public FakeGraphServiceClient(IGraphServiceUsersCollectionRequestBuilder users)
                : base(Substitute.For<IAuthenticationProvider>())
            {
                _users = users;
            }

            public override IGraphServiceUsersCollectionRequestBuilder Users => _users;
        }

        public class TestableAzureActiveDirectoryAuthenticationSource : AzureActiveDirectoryAuthenticationSource<TestTenant, TestUser>
        {
            public TestableAzureActiveDirectoryAuthenticationSource(IAzureActiveDirectorySettings settings, IEafMiddlewareAzureActiveDirectoryModuleConfig azureActiveDirectoryModuleConfig)
                : base(settings, azureActiveDirectoryModuleConfig)
            {
            }

            public IGraphServiceUsersCollectionRequestBuilder UsersRequestBuilderToReturn { get; set; } = null!;
            public IPublicClientApplication PublicClientApplicationToReturn { get; set; } = null!;
            public IConfidentialClientApplication ConfidentialClientApplicationToReturn { get; set; } = null!;

            protected override Task<GraphServiceClient> CreateGraphServiceClient()
            {
                return Task.FromResult<GraphServiceClient>(new FakeGraphServiceClient(UsersRequestBuilderToReturn));
            }

            protected override Task<IPublicClientApplication> CreateAzureApplication()
            {
                return Task.FromResult(PublicClientApplicationToReturn);
            }

            protected override Task<IConfidentialClientApplication> CreateAzureConfidential()
            {
                return Task.FromResult(ConfidentialClientApplicationToReturn);
            }

            public async Task CheckIsEnabledPublic(TestTenant? tenant)
            {
                await base.CheckIsEnabled(tenant!);
            }

            public void UpdateUserFromAzureActiveDirectoryPublic(TestUser user, TestUser? userPrincipal)
            {
                base.UpdateUserFromAzureActiveDirectory(user, userPrincipal!);
            }

            public async Task<IPublicClientApplication> CreateAzureApplicationPublic()
            {
                return await base.CreateAzureApplication();
            }

            public async Task<IConfidentialClientApplication> CreateAzureConfidentialPublic()
            {
                return await base.CreateAzureConfidential();
            }

            public async Task<GraphServiceClient> CreateGraphServiceClientPublic()
            {
                return await base.CreateGraphServiceClient();
            }
        }
    }
}
