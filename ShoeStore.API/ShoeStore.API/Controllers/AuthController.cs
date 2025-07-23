using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeStore.API.Data;
using ShoeStore.API.DTOs;
using ShoeStore.API.Models;
using ShoeStore.API.Services;

namespace ShoeStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDto)
        {
            var user = await _context.UserAccounts
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || user.PasswordHash != loginDto.Password)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(user);
            var response = new AuthResponseDTO
            {
                Token = token,
                Username = user.Username,
                Role = user.Role,
                Email = user.Email ?? "",
                CustomerId = user.Customer?.Id ?? 0,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Check if username already exists
            if (await _context.UserAccounts.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Check if email already exists
            if (await _context.UserAccounts.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // 1. Tạo user account trước (chưa gán CustomerId)
            var userAccount = new UserAccount
            {
                Username = registerDto.Username,
                PasswordHash = registerDto.Password, // Lưu plain text
                Email = registerDto.Email,
                Role = "Customer"
            };
            _context.UserAccounts.Add(userAccount);
            await _context.SaveChangesAsync();

            // 2. Tạo customer, gán UserAccountId
            var customer = new Customer
            {
                Name = registerDto.FullName,
                FullName = registerDto.FullName,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                Email = registerDto.Email,
                UserAccountId = userAccount.Id
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // 3. Cập nhật lại UserAccount.CustomerId
            userAccount.CustomerId = customer.Id;
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtService.GenerateToken(userAccount);
            var response = new AuthResponseDTO
            {
                Token = token,
                Username = userAccount.Username,
                Role = userAccount.Role,
                Email = userAccount.Email ?? "",
                CustomerId = customer.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDTO>> GetProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _context.UserAccounts
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            var profile = new UserProfileDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email ?? "",
                Role = user.Role,
                Customer = user.Customer != null ? new CustomerDTO
                {
                    Id = user.Customer.Id,
                    Name = user.Customer.Name,
                    Phone = user.Customer.Phone,
                    Address = user.Customer.Address,
                    Email = user.Customer.Email,
                    FullName = user.Customer.FullName
                } : null
            };

            return Ok(profile);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDTO changePasswordDto)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            if (user.PasswordHash != changePasswordDto.CurrentPassword)
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            user.PasswordHash = changePasswordDto.NewPassword;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<List<UserProfileDTO>>> GetAllUsers()
        {
            var users = await _context.UserAccounts
                .Include(u => u.Customer)
                .Select(u => new UserProfileDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email ?? "",
                    Role = u.Role,
                    Customer = u.Customer != null ? new CustomerDTO
                    {
                        Id = u.Customer.Id,
                        Name = u.Customer.Name,
                        Phone = u.Customer.Phone,
                        Address = u.Customer.Address,
                        Email = u.Customer.Email,
                        FullName = u.Customer.FullName
                    } : null
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/role")]
        public async Task<ActionResult> UpdateUserRole(int id, [FromBody] string role)
        {
            if (role != "Admin" && role != "Customer")
            {
                return BadRequest(new { message = "Invalid role. Must be 'Admin' or 'Customer'" });
            }

            var user = await _context.UserAccounts.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Role = role;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User role updated successfully" });
        }
    }
}
