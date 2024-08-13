using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdajaAutomobila.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Brand { get; set; } = "";

        [MaxLength(100)]
        public string Category { get; set; } = "";

        [MaxLength(20)]  
        public string Price { get; set; } = "";  

        public string Description { get; set; } = "";

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public decimal PriceDecimal
        {
            get
            {
                if (decimal.TryParse(Price, out decimal result))
                {
                    return result;
                }
                return 0;
            }
        }
    }
}
