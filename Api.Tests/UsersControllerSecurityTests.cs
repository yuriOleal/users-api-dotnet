using System.Security.Claims;
using System.Text.Json;
using Api.Controllers;
using Api.Data;
using Api.DTOs;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests;

public sealed class UsersControllerSecurityTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly AppDbContext _context;
    private readonly User _owner = new()
    {
        Name = "Owner",
        Email = "owner@example.test",
        PasswordHash = "private-owner-hash"
    };
    private readonly User _other = new()
    {
        Name = "Other",
        Email = "other@example.test",
        PasswordHash = "private-other-hash"
    };

    public UsersControllerSecurityTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _context.Users.AddRange(_owner, _other);
        _context.SaveChanges();
    }

    [Fact]
    public void GetById_DoesNotExposePasswordHash()
    {
        var response = Assert.IsType<OkObjectResult>(AsOwner().GetById(_owner.Id));
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(response.Value));

        Assert.Equal(_owner.Id.ToString(), json.RootElement.GetProperty("Id").GetGuid().ToString());
        Assert.False(json.RootElement.TryGetProperty("PasswordHash", out _));
    }

    [Fact]
    public void GetAll_ReturnsOnlyTheAuthenticatedUserWithoutPasswordHash()
    {
        var response = Assert.IsType<OkObjectResult>(AsOwner().GetAll());
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(response.Value));

        var item = Assert.Single(json.RootElement.EnumerateArray());
        Assert.Equal(_owner.Id, item.GetProperty("Id").GetGuid());
        Assert.False(item.TryGetProperty("PasswordHash", out _));
    }

    [Fact]
    public void Update_RejectsChangesToAnotherUser()
    {
        var response = AsOwner().Update(_other.Id, new UpdateUserDto
        {
            Name = "Tampered",
            Email = "tampered@example.test"
        });

        Assert.IsType<ForbidResult>(response);
        Assert.Equal("Other", _context.Users.AsNoTracking().Single(u => u.Id == _other.Id).Name);
    }

    [Fact]
    public void Delete_RejectsRemovalOfAnotherUser()
    {
        var response = AsOwner().Delete(_other.Id);

        Assert.IsType<ForbidResult>(response);
        Assert.True(_context.Users.AsNoTracking().Any(u => u.Id == _other.Id));
    }

    [Fact]
    public void Update_AllowsOwnerAndReturnsNoPasswordHash()
    {
        var response = Assert.IsType<OkObjectResult>(AsOwner().Update(_owner.Id, new UpdateUserDto
        {
            Name = "Updated Owner",
            Email = "updated@example.test"
        }));
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(response.Value));

        Assert.Equal("Updated Owner", json.RootElement.GetProperty("Name").GetString());
        Assert.False(json.RootElement.TryGetProperty("PasswordHash", out _));
    }

    [Fact]
    public void GetById_RejectsRequestWithoutUserIdentity()
    {
        var controller = new UsersController(new UserService(_context))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        Assert.IsType<ForbidResult>(controller.GetById(_owner.Id));
    }

    private UsersController AsOwner()
    {
        var controller = new UsersController(new UserService(_context));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, _owner.Id.ToString()) },
                    "TestAuthentication"))
            }
        };
        return controller;
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
