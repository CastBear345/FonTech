using Serilog;
using FonTech.Domain.Result;
using FonTech.Domain.Entities;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using FonTech.Domain.DTO.Report;
using FonTech.Domain.Interfaces.Validations;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using FonTech.Producer.Interfaces;
using Microsoft.Extensions.Options;
using FonTech.Domain.Settings;

namespace FonTech.Application.Services;

public class ReportService(IBaseRepository<Report> reportRepository, ILogger logger,
    IBaseRepository<User> userRepository, IReportValidator reportValidator,
    IMapper mapper, IMessageProducer messageProducer, IOptions<RabbitMqSettings> rabbitMqOptions) : IReportServices
{
    private readonly IOptions<RabbitMqSettings> _rabbitMqOptions = rabbitMqOptions;
    private readonly IBaseRepository<Report> _reportRepository = reportRepository;
    private readonly IBaseRepository<User> _userRepository = userRepository;
    private readonly IReportValidator _reportValidator = reportValidator;
    private readonly IMessageProducer _messageProducer = messageProducer;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger _logger = logger;

    /// <inheritdoc />
    public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
    {
        ReportDto[] reports;

        try
        {
            reports = await _reportRepository
                .GetAll()
                .Where(r => r.UserId == userId)
                .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                .ToArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
            return new CollectionResult<ReportDto>
            {
                ErrorMessage = "Внутренняя ошибка сервера",
                ErrorCode = 10,
            };
        }

        if (!reports.Any())
        {
            _logger.Warning("Отчёты не найдены!", reports.Length);
            return new CollectionResult<ReportDto>
            {
                ErrorMessage = "Отчёты не найдены!",
                ErrorCode = 0,
            };
        }

        return new CollectionResult<ReportDto>
        {
            Data = reports,
            Count = reports.Length
        };
    }

    /// <inheritdoc />
    public Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
    {
        ReportDto? report;
        try
        {
            report = _reportRepository
                .GetAll()
                .AsEnumerable()
                .Select(r => new ReportDto(r.Id, r.Name, r.Description, r.CreatedAt.ToLongDateString()))
                .FirstOrDefault(r => r.Id == id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
            return Task.FromResult(new BaseResult<ReportDto>
            {
                ErrorMessage = "Внутренняя ошибка сервера",
                ErrorCode = 10,
            });
        }

        if (report == null)
        {
            _logger.Warning($"Отчёт с {id} не найден!", id);
            return Task.FromResult(new BaseResult<ReportDto>
            {
                ErrorMessage = "Отчёт не найден!",
                ErrorCode = 1,
            });
        }

        return Task.FromResult(new BaseResult<ReportDto>()
        {
            Data = report,
        });
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {
        var user = await _userRepository
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);
        var report = await _reportRepository
            .GetAll()
            .FirstOrDefaultAsync(r => r.Name == dto.Name);

        var result = _reportValidator.CreateValidator(report, user);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode,
            };
        }

        report = new Report()
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = user.Id,
        };

        await _reportRepository.CreateAsync(report);
        await _reportRepository.SaveChangesAsync();

        _messageProducer.SendMessage(report, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

        return new BaseResult<ReportDto>()
        {
            Data = _mapper.Map<ReportDto>(report),
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
    {
        var report = await _reportRepository
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == id);
        var result = _reportValidator.ValidateOnNull(report);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode,
            };
        }

        _reportRepository.Remove(report);
        await _reportRepository.SaveChangesAsync();

        return new BaseResult<ReportDto>()
        {
            Data = _mapper.Map<ReportDto>(report),
        };
    }

    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
    {
        var report = await _reportRepository
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == dto.Id);

        var result = _reportValidator.ValidateOnNull(report);
        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode,
            };
        }

        report.Name = dto.Name;
        report.Description = dto.Description;

        var updatedReport = _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync();

        return new BaseResult<ReportDto>()
        {
            Data = _mapper.Map<ReportDto>(updatedReport),
        };
    }
}
