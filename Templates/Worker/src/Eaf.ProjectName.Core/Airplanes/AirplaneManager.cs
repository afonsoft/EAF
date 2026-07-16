using System.Threading.Tasks;
using Abp.Domain.Repositories;
using System.Linq;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;

namespace Eaf.ProjectName.Airplanes
{
    public class AirplaneManager : ProjectNameDomainServiceBase, IAirplaneManager
    {
        private readonly IRepository<Airplane> _repositoryAirplane;

        public virtual IQueryable<Airplane> Airplanes
        {
            get
            {
                return _repositoryAirplane.GetAll();
            }
        }

        public AirplaneManager(
            IRepository<Airplane> repositoryAirplane
        )
        {
            _repositoryAirplane = repositoryAirplane;
        }

        public async Task<Airplane> GetByIdAsync(int id)
        {
            return await _repositoryAirplane.FirstOrDefaultAsync(id);
        }

        public async Task<Airplane> CreateAsync(Airplane airplane)
        {
            if (await (await _repositoryAirplane.GetAllAsync()).AnyAsync(e => e.Number.ToLower() == airplane.Number))
                throw new UserFriendlyException(L("AirplaneValidate"));

            return await _repositoryAirplane.InsertAsync(airplane);
        }

        public async Task DeleteAsync(int id)
        {
            await _repositoryAirplane.DeleteAsync(id);
        }

        public async Task<Airplane> UpdateAsync(Airplane airplane)
        {
            return await _repositoryAirplane.UpdateAsync(airplane);
        }
    }
}