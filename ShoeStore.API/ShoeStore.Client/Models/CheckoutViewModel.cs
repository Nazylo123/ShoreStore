using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Client.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string Address { get; set; } = string.Empty;

        public string? Note { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "COD";
    }
} 