namespace ShoeStore.API.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }  // "Admin" hoặc "Customer"
        public string? Email { get; set; }
        public int? CustomerId { get; set; }

        public Customer? Customer { get; set; }
    }
}
