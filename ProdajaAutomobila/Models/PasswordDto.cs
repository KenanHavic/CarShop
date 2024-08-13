using System.ComponentModel.DataAnnotations;

namespace ProdajaAutomobila.Models
{
	public class PasswordDto
	{
		[Required(ErrorMessage = "Trenutna šifra je potrebna"), MaxLength(100)]
		public string CurrentPassword { get; set; } = "";
		[Required(ErrorMessage = "Nova šifra je potrebna"), MaxLength(100)]
		public string NewPassword { get; set; } = "";

		[Required(ErrorMessage = "Potrebno je unijeti ispravnu lozinku")]
		[Compare("NewPassword", ErrorMessage = "Lozinke se ne podudaraju")]
		public string ConfirmPassword { get; set; } = "";
	}
}
