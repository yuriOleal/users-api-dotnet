using Api.DTOs;
using Api.Models;

namespace Api.Services;

public interface IUserService
{
    User? GetById(Guid id);
    User? Update(Guid id, UpdateUserDto dto);
    bool Delete(Guid id);
}

