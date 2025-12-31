using Api.DTOs;
using Api.Models;

namespace Api.Services;

public class UserService : IUserService
{
    private static readonly List<User> _users = new();

    public IEnumerable<User> GetAll()
        => _users;

    public User? GetById(Guid id)
        => _users.FirstOrDefault(u => u.Id == id);

    public User Create(CreateUserDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email
        };

        _users.Add(user);
        return user;
    }
    public User? Update(Guid id, UpdateUserDto dto)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null) return null;

        user.Name = dto.Name;
        user.Email = dto.Email;

        return user;
    }
    public bool Delete(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        _users.Remove(user);
        return true;
    }
}
