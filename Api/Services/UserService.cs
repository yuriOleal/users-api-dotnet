using Api.Data;
using Api.DTOs;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users.ToList();
    }

    public User? GetById(Guid id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public User Create(CreateUserDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public User? Update(Guid id, UpdateUserDto dto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return null;

        user.Name = dto.Name;
        user.Email = dto.Email;

        _context.SaveChanges();
        return user;
    }

    public bool Delete(Guid id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        _context.Users.Remove(user);
        _context.SaveChanges();
        return true;
    }
}
