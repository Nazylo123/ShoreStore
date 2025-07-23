using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Client.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        
        public int ProductCount { get; set; }
    }
} 