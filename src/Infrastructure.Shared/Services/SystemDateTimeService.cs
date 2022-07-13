using EDO_FOMS.Application.Interfaces.Services;
using System;

namespace EDO_FOMS.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}