using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Client.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new();
        public string FormattedTotalAmount => TotalAmount.ToString("N0") + " VNĐ";
        public string FormattedOrderDate => OrderDate.ToString("dd/MM/yyyy HH:mm");
    }

    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Size { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string FormattedPrice => Price.ToString("N0") + " VNĐ";
        public string FormattedTotalPrice => TotalPrice.ToString("N0") + " VNĐ";
    }

    public class CreateOrderViewModel
    {
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;
        [Required]
        [Phone]
        public string CustomerPhone { get; set; } = string.Empty;
        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        public List<AddToCartViewModel> Items { get; set; } = new();
    }
} 