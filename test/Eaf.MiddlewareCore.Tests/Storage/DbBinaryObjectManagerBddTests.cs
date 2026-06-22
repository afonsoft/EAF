using Abp.Domain.Repositories;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Storage
{
    /// <summary>
    /// Testes BDD para DbBinaryObjectManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class DbBinaryObjectManagerBddTests
    {
        private readonly IRepository<BinaryObject, Guid> _repository;
        private readonly DbBinaryObjectManager _sut;

        public DbBinaryObjectManagerBddTests()
        {
            _repository = Substitute.For<IRepository<BinaryObject, Guid>>();
            _sut = new DbBinaryObjectManager(_repository);
        }

        #region GetOrNullAsync por Id

        [Fact]
        public async Task Dado_IdExistente_Quando_GetOrNullAsync_Entao_DeveRetornarBinaryObject()
        {
            // Dado
            var id = Guid.NewGuid();
            var binaryObject = new BinaryObject(null, new byte[] { 1, 2, 3 }, "application/pdf", "test.pdf");
            _repository.FirstOrDefaultAsync(id).Returns(binaryObject);

            // Quando
            var result = await _sut.GetOrNullAsync(id);

            // Entao
            result.ShouldBe(binaryObject);
        }

        [Fact]
        public async Task Dado_IdInexistente_Quando_GetOrNullAsync_Entao_DeveRetornarNull()
        {
            // Dado
            var id = Guid.NewGuid();
            _repository.FirstOrDefaultAsync(id).Returns((BinaryObject)null);

            // Quando
            var result = await _sut.GetOrNullAsync(id);

            // Entao
            result.ShouldBeNull();
        }

        #endregion

        #region GetOrNullAsync por FileName

        [Fact]
        public async Task Dado_FileNameExistente_Quando_GetOrNullAsync_Entao_DeveRetornarBinaryObject()
        {
            // Dado
            var binaryObject = new BinaryObject(null, new byte[] { 1, 2, 3 }, "application/pdf", "test.pdf");
            _repository.FirstOrDefaultAsync(Arg.Any<Expression<Func<BinaryObject, bool>>>())
                .Returns(binaryObject);

            // Quando
            var result = await _sut.GetOrNullAsync("test.pdf");

            // Entao
            result.ShouldBe(binaryObject);
        }

        #endregion

        #region SaveAsync

        [Fact]
        public async Task Dado_BinaryObject_Quando_SaveAsync_Entao_DeveInserirNoRepositorio()
        {
            // Dado
            var file = new BinaryObject(null, new byte[] { 10, 20, 30 }, "text/plain", "data.txt");

            // Quando
            await _sut.SaveAsync(file);

            // Entao
            await _repository.Received(1).InsertAsync(file);
        }

        #endregion

        #region SaveAndGetIdAsync

        [Fact]
        public async Task Dado_BinaryObject_Quando_SaveAndGetIdAsync_Entao_DeveRetornarId()
        {
            // Dado
            var expectedId = Guid.NewGuid();
            var file = new BinaryObject(null, new byte[] { 40, 50, 60 }, "image/png", "image.png");
            _repository.InsertAndGetIdAsync(file).Returns(expectedId);

            // Quando
            var result = await _sut.SaveAndGetIdAsync(file);

            // Entao
            result.ShouldBe(expectedId);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task Dado_Id_Quando_DeleteAsync_Entao_DeveDeletarNoRepositorio()
        {
            // Dado
            var id = Guid.NewGuid();

            // Quando
            await _sut.DeleteAsync(id);

            // Entao
            await _repository.Received(1).DeleteAsync(id);
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Repository_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<IBinaryObjectManager>();
        }

        #endregion
    }
}
