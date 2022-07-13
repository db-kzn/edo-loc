using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;

namespace EDO_FOMS.Infrastructure.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly IRepositoryAsync<Certificate, int> _repository;

        public CertificateRepository(IRepositoryAsync<Certificate, int> repository)
        {
            _repository = repository;
        }
    }
}
