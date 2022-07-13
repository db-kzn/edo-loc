using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Dir;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EDO_FOMS.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IRepositoryAsync<Company, int> _repository;

        public CompanyRepository(IRepositoryAsync<Company, int> repository)
        {
            _repository = repository;
        }

        public async Task<Company> GetByCodeAsync(string code)
        {
            return await _repository.Entities.FirstOrDefaultAsync(b => b.Code == code);
        }

        public async Task<Company> GetByInnAsync(string inn)
        {
            return await _repository.Entities.FirstOrDefaultAsync(b => b.Inn == inn);
        }
    }
}
