using System.Security.Claims;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        if (!TryGetCurrentUserId(out var currentUserId)) return Forbid();

        var user = _service.GetById(currentUserId);
        return Ok(user is null
            ? Array.Empty<UserResponseDto>()
            : new[] { UserResponseDto.From(user) });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        if (!TryGetCurrentUserId(out var currentUserId) || id != currentUserId)
            return Forbid();

        var user = _service.GetById(id);
        if (user is null) return NotFound();
        return Ok(UserResponseDto.From(user));
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, UpdateUserDto dto)
    {
        if (!TryGetCurrentUserId(out var currentUserId) || id != currentUserId)
            return Forbid();

        var user = _service.Update(id, dto);
        if (user == null) return NotFound();

        return Ok(UserResponseDto.From(user));
    }
   
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        if (!TryGetCurrentUserId(out var currentUserId) || id != currentUserId)
            return Forbid();

        var deleted = _service.Delete(id);

        if (!deleted)
            return NotFound();

        return NoContent(); // 204 padrão REST
    }

    private bool TryGetCurrentUserId(out Guid userId)
        => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}
