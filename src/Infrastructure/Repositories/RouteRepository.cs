using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Infrastructure.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly IRepositoryAsync<Route, int> _repository;

        public RouteRepository(IRepositoryAsync<Route, int> repository)
        {
            _repository = repository;
        }
    }
}
