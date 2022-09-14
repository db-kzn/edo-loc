using EDO_FOMS.Application.Interfaces.Common;
using System;

namespace EDO_FOMS.Application.Interfaces.Services
{
    public interface IDateTimeService : IService
    {
        DateTime NowUtc { get; }
    }
}