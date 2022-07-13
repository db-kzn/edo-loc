using EDO_FOMS.Domain.Entities.Dir;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        public Task<Company> GetByCodeAsync(string inn);
        public Task<Company> GetByInnAsync(string inn);
    }
}
