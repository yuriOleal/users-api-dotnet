namespace Api.DTOs
{
    public class CreateUserDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}
