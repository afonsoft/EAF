using System;
using System.Data;

namespace Eaf.Dapper
{
    /// <summary>
    /// Fornece acesso à <see cref="IDbConnection"/> ativa do Unit of Work corrente
    /// para um <see cref="Microsoft.EntityFrameworkCore.DbContext"/> concreto.
    /// </summary>
    public interface IDapperConnectionProvider
    {
        /// <summary>
        /// Retorna a conexão aberta do <paramref name="dbContextType"/> no UoW corrente.
        /// Lança <see cref="Abp.UI.UserFriendlyException"/> fora de um UoW ativo.
        /// </summary>
        IDbConnection GetConnection(Type dbContextType);
    }
}
