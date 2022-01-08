using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Posta.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Address { get; set; }
        public List<Delivery> Deliveries { get; set; }
    }
}