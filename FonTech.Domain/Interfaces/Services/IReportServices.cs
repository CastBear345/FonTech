using FonTech.Domain.DTO;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;

public interface IReportServices
{
    Task<CollectionResult<ReportDto>> GetReportsAsync(long userId);
}
