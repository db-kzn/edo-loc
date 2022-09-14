using EDO_FOMS.Application.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Services
{
    public interface IExcelService : IService
    {
        Task<string> ExportAsync<TData>(IEnumerable<TData> data
            , Dictionary<string, Func<TData, object>> mappers
            , string sheetName = "Sheet1");
    }
}