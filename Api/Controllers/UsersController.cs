using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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
        => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var user = _service.GetById(id);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create(CreateUserDto dto)
    {
        var user = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, UpdateUserDto dto)
    {
        var user = _service.Update(id, dto);
        if (user == null) return NotFound();

        return Ok(user);
    }
   
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var deleted = _service.Delete(id);

        if (!deleted)
            return NotFound();

        return NoContent(); // 204 padrão REST
    }
}
