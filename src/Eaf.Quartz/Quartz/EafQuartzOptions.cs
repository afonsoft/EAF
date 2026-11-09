using Microsoft.Extensions.Options;
using System;

namespace Eaf.Quartz
{
    /// <summary>
    /// Opções de configuração do módulo Eaf.Quartz.
    /// </summary>
    [Serializable]
    public class EafQuartzOptions : IOptions<EafQuartzOptions>
    {
        /// <summary>
        /// Obtém ou define o nome do scheduler Quartz. Padrão: <c>EafQuartz</c>.
        /// </summary>
        public string SchedulerName { get; set; } = "EafQuartz";

        /// <summary>
        /// Obtém ou define o tipo de job store. Padrão: <see cref="EafQuartzJobStoreType.Ram"/>.
        /// </summary>
        public EafQuartzJobStoreType JobStoreType { get; set; } = EafQuartzJobStoreType.Ram;

        /// <summary>
        /// Obtém ou define a connection string usada pelos stores <see cref="EafQuartzJobStoreType.Ado"/>
        /// e <see cref="EafQuartzJobStoreType.Mongo"/>.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Obtém ou define o provider ADO.NET usado pelo store
        /// <see cref="EafQuartzJobStoreType.Ado"/> (ex.: <c>SqlServer</c>, <c>Postgres</c>).
        /// </summary>
        public string AdoProvider { get; set; } = "SqlServer";

        /// <summary>
        /// Obtém ou define o identificador da instância do scheduler (clustering). Padrão: <c>AUTO</c>.
        /// </summary>
        public string InstanceId { get; set; } = "AUTO";

        /// <summary>
        /// Obtém o valor das opções (implementação de <see cref="IOptions{TOptions}.Value"/>).
        /// </summary>
        public EafQuartzOptions Value => this;
    }

    /// <summary>
    /// Tipo de job store persistido usado pelo scheduler Quartz.
    /// </summary>
    public enum EafQuartzJobStoreType
    {
        /// <summary>Job store em memória (padrão; não persiste entre reinícios).</summary>
        Ram,

        /// <summary>Job store ADO.NET (tabelas Quartz no banco relacional).</summary>
        Ado,

        /// <summary>Job store MongoDB.</summary>
        Mongo
    }
}
