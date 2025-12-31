using Api.DTOs;
using Api.Models;

namespace Api.Services;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User? GetById(Guid id);
    User Create(CreateUserDto dto);
    User? Update(Guid id, UpdateUserDto dto);
    bool Delete(Guid id);

}

