using Api.Models;

namespace Api.DTOs;

public sealed record UserResponseDto(Guid Id, string Name, string Email)
{
    public static UserResponseDto From(User user) => new(user.Id, user.Name, user.Email);
}
