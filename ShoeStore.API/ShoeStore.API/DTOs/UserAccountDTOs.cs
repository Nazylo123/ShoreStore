using System.ComponentModel.DataAnnotations;

namespace ShoeStore.API.DTOs
{
    public class UserAccountResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    public class CreateUserAccountDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Admin|Customer)$", ErrorMessage = "Role must be either 'Admin' or 'Customer'")]
        public string Role { get; set; } = "Customer";

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
    }

    public class UpdateUserAccountDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Admin|Customer)$", ErrorMessage = "Role must be either 'Admin' or 'Customer'")]
        public string Role { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
    }
} 