using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EDO_FOMS.Infrastructure.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IRepositoryAsync<Organization, int> _repository;

        public OrganizationRepository(IRepositoryAsync<Organization, int> repository)
        {
            _repository = repository;
        }

        public async Task<Organization> GetByInnAsync(string inn)
        {
            return await _repository.Entities.FirstOrDefaultAsync(b => b.Inn == inn);
        }
    }
}
