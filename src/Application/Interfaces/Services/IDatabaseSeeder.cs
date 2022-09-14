using EDO_FOMS.Application.Interfaces.Common;

namespace EDO_FOMS.Application.Interfaces.Services
{
    public interface IDatabaseSeeder : IService
    {
        void Initialize();
    }
}