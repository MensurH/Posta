using System.ComponentModel.DataAnnotations;

namespace Posta.Models
{
    public class UpdateStatusModel
    {
        [Required]
        [Display(Name = "Delivery Status")]
        public string OrderStatus { get; set; }
        [Required]
        public int DeliveryId { get; set; }
    }
}