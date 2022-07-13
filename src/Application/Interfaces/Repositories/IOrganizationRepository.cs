using EDO_FOMS.Domain.Entities.Org;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Repositories
{
    public interface IOrganizationRepository
    {
        public Task<Organization> GetByInnAsync(string inn);
    }
}
