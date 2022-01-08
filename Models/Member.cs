namespace Posta.Models
{
    public class Member
    {
        public ApplicationUser User { get; set; }
        public string Role { get; set; }
    }
}