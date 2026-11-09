using Abp.Dapper.Repositories;
using Abp.Dependency;
using Abp.EntityFrameworkCore;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace Eaf.Dapper
{
    /// <summary>
    /// Implementação de <see cref="IDapperConnectionProvider"/> que resolve
    /// <see cref="IDbContextProvider{TDbContext}"/> e retorna a conexão do UoW corrente.
    /// </summary>
    public class DapperConnectionProvider : IDapperConnectionProvider, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;

        /// <summary>
        /// Cria o provedor de conexão Dapper.
        /// </summary>
        public DapperConnectionProvider(IIocResolver iocResolver, ICurrentUnitOfWorkProvider unitOfWorkProvider)
        {
            _iocResolver = iocResolver;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        /// <inheritdoc />
        public IDbConnection GetConnection(Type dbContextType)
        {
            if (_unitOfWorkProvider.Current == null)
            {
                throw new UserFriendlyException("Dapper requer um Unit of Work ativo. Chame dentro de [UnitOfWork] ou IUnitOfWorkManager.");
            }

            var providerType = typeof(IDbContextProvider<>).MakeGenericType(dbContextType);
            var provider = _iocResolver.Resolve(providerType);
            var getDbContext = System.Linq.Enumerable.Single(
                providerType.GetMethods(),
                m => m.Name == nameof(IDbContextProvider<DbContext>.GetDbContext) && m.GetParameters().Length == 0 && !m.IsGenericMethod);
            var context = (DbContext)getDbContext.Invoke(provider, null);
            return context.Database.GetDbConnection();
        }
    }
}
