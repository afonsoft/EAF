using Abp;
using Abp.Domain.Repositories;
using Abp.UI;
using Eaf.Middleware.Authentication.Passkeys;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Authentication
{
    /// <summary>
    /// Testes do PasskeyManager — listagem, inserção, busca por credentialId e remoção com ownership.
    /// </summary>
    public class PasskeyManagerTests
    {
        private static PasskeyManager CreateManager(out IRepository<UserPasskey, long> repo)
        {
            repo = Substitute.For<IRepository<UserPasskey, long>>();
            return new PasskeyManager(repo);
        }

        private static UserPasskey Sample(long id = 1, long userId = 10, int? tenantId = null)
            => new UserPasskey
            {
                Id = id,
                UserId = userId,
                TenantId = tenantId,
                Name = "Meu dispositivo",
                CredentialId = "Y3JlZGVudGlhbA",
                PublicKey = new byte[] { 1, 2, 3 },
                SignCount = 0,
                CreationTime = DateTime.UtcNow
            };

        [Fact]
        public async Task GetForUserAsync_RetornaOrdenadoPorCriacaoDesc()
        {
            // Dado
            var manager = CreateManager(out var repo);
            var user = new UserIdentifier(5, 10);
            var older = Sample(1, 10, 5);
            older.CreationTime = DateTime.UtcNow.AddHours(-1);
            var newer = Sample(2, 10, 5);
            repo.GetAllListAsync(Arg.Any<Expression<Func<UserPasskey, bool>>>())
                .Returns(new List<UserPasskey> { older, newer });

            // Quando
            var result = await manager.GetForUserAsync(user);

            // Então
            result.Count.ShouldBe(2);
            result[0].Id.ShouldBe(newer.Id);
        }

        [Fact]
        public async Task AddAsync_InsereERetornaEntidade()
        {
            var manager = CreateManager(out var repo);
            var key = Sample();

            var result = await manager.AddAsync(key);

            await repo.Received(1).InsertAsync(key);
            result.ShouldBe(key);
        }

        [Fact]
        public async Task FindByCredentialIdAsync_ChamarepoComPredicado()
        {
            var manager = CreateManager(out var repo);
            var key = Sample();
            repo.FirstOrDefaultAsync(Arg.Any<Expression<Func<UserPasskey, bool>>>())
                .Returns(key);

            var result = await manager.FindByCredentialIdAsync("Y3JlZGVudGlhbA");

            result.ShouldBe(key);
            await repo.Received(1).FirstOrDefaultAsync(Arg.Any<Expression<Func<UserPasskey, bool>>>());
        }

        [Fact]
        public async Task DeleteAsync_RemoveQuandoPertenceAoUsuario()
        {
            var manager = CreateManager(out var repo);
            var key = Sample(id: 7, userId: 10, tenantId: 3);
            repo.FirstOrDefaultAsync(7).Returns(key);

            await manager.DeleteAsync(new UserIdentifier(3, 10), 7);

            await repo.Received(1).DeleteAsync(key);
        }

        [Fact]
        public async Task DeleteAsync_FalhaQuandoPasskeyNaoExiste()
        {
            var manager = CreateManager(out var repo);
            repo.FirstOrDefaultAsync(99).Returns((UserPasskey)null);

            await Should.ThrowAsync<UserFriendlyException>(
                () => manager.DeleteAsync(new UserIdentifier(null, 10), 99));
        }

        [Fact]
        public async Task DeleteAsync_FalhaQuandoPertenceAOutroUsuario()
        {
            var manager = CreateManager(out var repo);
            repo.FirstOrDefaultAsync(7).Returns(Sample(7, userId: 42, tenantId: null));

            await Should.ThrowAsync<UserFriendlyException>(
                () => manager.DeleteAsync(new UserIdentifier(null, 10), 7));
            await repo.DidNotReceive().DeleteAsync(Arg.Any<UserPasskey>());
        }

        [Fact]
        public async Task DeleteAsync_FalhaQuandoTenantDiverge()
        {
            var manager = CreateManager(out var repo);
            repo.FirstOrDefaultAsync(7).Returns(Sample(7, userId: 10, tenantId: 3));

            await Should.ThrowAsync<UserFriendlyException>(
                () => manager.DeleteAsync(new UserIdentifier(9, 10), 7));
        }

        [Fact]
        public async Task UpdateAsync_PersisteEntidade()
        {
            var manager = CreateManager(out var repo);
            var key = Sample();
            key.SignCount = 5;
            key.LastUsedTime = DateTime.UtcNow;

            await manager.UpdateAsync(key);

            await repo.Received(1).UpdateAsync(key);
        }
    }
}
