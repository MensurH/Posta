using System.ComponentModel.DataAnnotations;

namespace Posta.Models
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}