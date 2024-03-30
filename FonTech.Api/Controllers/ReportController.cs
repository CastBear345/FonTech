using FonTech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using FonTech.Domain.DTO.Report;
using Microsoft.AspNetCore.Mvc;
using FonTech.Domain.Result;
using Asp.Versioning;

namespace FonTech.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/report")]
public class ReportController : ControllerBase
{
    private readonly IReportServices _reportService;

    public ReportController(IReportServices reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Получение отчетов пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create report
    /// 
    ///     GET
    ///     {
    ///        "userId": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если отчёт получен</response>
    /// <response code="400">Если отчёт не был получен</response>
    [HttpGet("reports/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReports(long userId)
    {
        var response = await _reportService.GetReportsAsync(userId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Получение отчётов по идентификатору
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create report
    /// 
    ///     GET
    ///     {
    ///        "Id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если отчёт получен</response>
    /// <response code="400">Если отчёт не был получен</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
    {
        var response = await _reportService.GetReportByIdAsync(id);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Создание отчета
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create report
    /// 
    ///     POST
    ///     {
    ///        "name": "Report #1",
    ///        "description": "test report",
    ///        "userId": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если отчёт создан</response>
    /// <response code="400">Если отчёт не был создан</response>
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> Create([FromBody] CreateReportDto dto)
    {
        var response = await _reportService.CreateReportAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Удаление отчета по идентификатору
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create report
    /// 
    ///     DELETE
    ///     {
    ///        "Id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если отчёт успешно удалён</response>
    /// <response code="400">Если отчёт не был удалён</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> Delete(long id)
    {
        var response = await _reportService.DeleteReportAsync(id);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Обновление отчета с указанием основных свойств
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create report
    /// 
    ///     PUT
    ///     {
    ///        "Id": 1
    ///        "name": "Report #2",
    ///        "description": "changed report",
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если отчёт обновлён</response>
    /// <response code="400">Если отчёт не был обновлён</response>
    [HttpPut()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> Update([FromBody] UpdateReportDto dto)
    {
        var response = await _reportService.UpdateReportAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

}
