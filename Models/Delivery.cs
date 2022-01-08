using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Posta.Models
{
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string OrderStatus { get; set; }
        [Required]
        [Display(Name = "Reciever Name")]
        public string RecieverName { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public int PhoneNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime DeliverdAt { get; set; }
    }
}