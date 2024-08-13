using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace ProdajaAutomobila.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime CreatedAt { get; set; }



    }
}
