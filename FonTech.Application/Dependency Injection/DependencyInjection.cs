using FluentValidation;
using FonTech.Application.Mapping;
using FonTech.Application.Services;
using FonTech.Application.Validations;
using FonTech.Application.Validations.FluentValidations.Report;
using FonTech.Domain.DTO.Report;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Validations;
using Microsoft.Extensions.DependencyInjection;

namespace FonTech.Application.Dependency_Injection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ReportMapping));

        InitService(services);
    }

    private static void InitService(this IServiceCollection services)
    {
        services.AddScoped<IReportValidator, ReportValidator>();

        services.AddScoped<IValidator<CreateReportDto>, CreateReportValidator>();
        services.AddScoped<IValidator<UpdateReportDto>, UpdateReportValidator>();

        services.AddScoped<IReportServices, ReportService>();
    }
}
