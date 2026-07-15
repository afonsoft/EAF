using Abp.Domain.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.ProjectName.Airplanes
{
    public interface IAirplaneManager : IDomainService
    {
        IQueryable<Airplane> Airplanes { get; }

        Task<Airplane> CreateAsync(Airplane airplane);

        Task<Airplane> UpdateAsync(Airplane airplane);

        Task DeleteAsync(int id);

        Task<Airplane> GetByIdAsync(int id);
    }
}