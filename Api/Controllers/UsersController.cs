using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.DTOs;
using System.Collections.Generic;
using System.Linq;
using Api.Dtos;

namespace Api.Controllers
{
    // Marca esta classe como Controller de API
    [ApiController]

    // Define a rota base como: /api/users
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Lista em memória simulando um banco de dados
        private static List<User> users = new()
        {
            new User
            {
                Id = 1,
                Name = "Yuri",
                Email = "yurilisco@gmail.com"
            }
        };

        // GET /api/users
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(users);
        }

        // GET /api/users/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            return Ok(user);
        }

        // POST /api/users
        [HttpPost]
        public IActionResult Create(CreateUserDto dto)
        {
            // Validação simples
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Nome é obrigatório" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email é obrigatório" });

            var newUser = new User
            {
                Id = users.Any() ? users.Max(u => u.Id) + 1 : 1,
                Name = dto.Name,
                Email = dto.Email
            };

            users.Add(newUser);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newUser.Id },
                newUser
            );
        }

        // PUT api/users/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateUserDto dto)
        {
            // Procura o usuário existente
            var user = users.FirstOrDefault(u => u.Id == id);

            // Se não existir, 404 (padrão REST)
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            // Atualiza apenas os campos permitidos
            user.Name = dto.Name;
            user.Email = dto.Email;

            // Retorna 200 com o usuário atualizado
            return Ok(user);
        }

        // DELETE api/users/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Procura o usuário
            var user = users.FirstOrDefault(u => u.Id == id);

            // Se não existir, retorna 404
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            // Remove da lista em memória
            users.Remove(user);

            // 204 = sucesso sem conteúdo (padrão REST)
            return NoContent();
        }
    }
}
