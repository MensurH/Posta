using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Posta.Models
{
    public class AddUserViewModel
    {
        public AddUserViewModel()
        {
            Users = new List<IdentityUser>();
        }
        [Required]
        [Display(Name = "Select a user: ")]
        public string UserId { get; set; }
        [Required]
        public string RoleId { get; set; }
        public IEnumerable<IdentityUser> Users { get; set; }
    }
}
