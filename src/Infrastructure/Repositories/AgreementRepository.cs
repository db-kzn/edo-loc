using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Doc;

namespace EDO_FOMS.Infrastructure.Repositories
{
    public class AgreementRepository : IAgreementRepository
    {
        private readonly IRepositoryAsync<Agreement, int> _repository;

        public AgreementRepository(IRepositoryAsync<Agreement, int> repository)
        {
            _repository = repository;
        }
    }
}
