namespace ShoeStore.API.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public int UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
