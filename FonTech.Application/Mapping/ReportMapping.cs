using AutoMapper;
using FonTech.Domain.DTO.Report;
using FonTech.Domain.Entities;

namespace FonTech.Application.Mapping;

public class ReportMapping : Profile
{
    public ReportMapping()
    {
        CreateMap<Report, ReportDto>().ReverseMap();
    }
}
