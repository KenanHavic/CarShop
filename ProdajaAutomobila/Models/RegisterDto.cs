using System.ComponentModel.DataAnnotations;

namespace ProdajaAutomobila.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Polje ime je potreno unijeti"), MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Polje prezime je potreno unijeti"), MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";

        [Phone(ErrorMessage = "Format mobilnog telefona nije validan"), MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; } = "";

        [Required, MaxLength(100)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Potrebno je unijeti ispravnu lozinku")]
        [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju")]
        public string ConfirmPassword { get; set; } = "";
    }
}
