using FonTech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using FonTech.Application.Services;
using FonTech.Domain.DTO.UserRole;
using Microsoft.AspNetCore.Mvc;
using FonTech.Domain.DTO.Role;
using FonTech.Domain.Entities;
using FonTech.Domain.Result;
using System.Net.Mime;

namespace FonTech.Api.Controllers;

[ApiController]
[Route("api/role")]
[Authorize(Roles = "Admin, Moderator")]
[Consumes(MediaTypeNames.Application.Json)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create role:
    /// 
    ///     POST
    ///     {
    ///        "Id": 1
    ///        "Name": "Admin"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль создана</response>
    /// <response code="400">Если роль не была создана</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Role>>> Create([FromBody] CreateRoleDto dto)
    {
        var response = await _roleService.CreateRoleAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Удаление роли по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Request for create role:
    /// 
    ///     DELETE
    ///     {
    ///        "Id": 1
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль удалена</response>
    /// <response code="400">Если роль не была удалена</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Role>>> Delete([FromBody] long id)
    {
        var response = await _roleService.DeleteRoleAsync(id);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Изменение роли
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create role:
    /// 
    ///     PUT
    ///     {
    ///        "Id": 1,
    ///        "Name": "Admin"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль обновлена</response>
    /// <response code="400">Если роль не была обновлена</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Role>>> Update([FromBody] RoleDto dto)
    {
        var response = await _roleService.UpdateRoleAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Добавление роли для пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request add role for user:
    /// 
    ///     POST
    ///     {
    ///        "Login": "ZachKing",
    ///        "RoleName": "Admin"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль добавлена пользователю</response>
    /// <response code="400">Если роль не была добавлена пользователю</response>
    [HttpPost("add-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Role>>> AddRoleFroUser([FromBody] UserRoleDto dto)
    {
        var response = await _roleService.AddRoleForUserAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for remove role in user:
    /// 
    ///     DELETE
    ///     {
    ///        "Login": "ZachKing",
    ///        "RoleName": "Admin"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль удалена у пользователя</response>
    /// <response code="400">Если роль не была удалена у пользователя</response>
    [HttpDelete("remove-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Role>>> DeleteRoleFroUser([FromBody] DeleteUserRoleDto dto)
    {
        var response = await _roleService.DeleteRoleForUserAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Обновление роли у пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for update role in user:
    /// 
    ///     PUT
    ///     {
    ///        "Login": "ZachKing",
    ///        "FromRoleId": "10",
    ///        "ToRoleId": "1"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если роль обновлена у пользователя</response>
    /// <response code="400">Если роль не была обновлена у пользователя</response>
    [HttpPut("update-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Role>>> UpdateRoleFroUser([FromBody] UpdateUserRoleDto dto)
    {
        var response = await _roleService.UpdateRoleForUserAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
