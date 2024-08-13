using System.ComponentModel.DataAnnotations;

namespace ProdajaAutomobila.Models
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "Polje za adresu je obavezno ispuniti.")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
