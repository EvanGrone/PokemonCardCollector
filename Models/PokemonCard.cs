using System.ComponentModel.DataAnnotations;

namespace PokemonCardCollector.Models
{
    public class PokemonCard
    {
        [Key]
        public int Id { get; set; } // primary key

        [Required(ErrorMessage = "Card name is required")]
        public string Name { get; set; } // card name

        [Display(Name = "Set Name")]
        public string SetName { get; set; } // which set the card is from 
        public int SetNumber { get; set; } // card number in the set
        public string Type { get; set; } // fire, water, fighting, etc 

        [DataType(DataType.Currency)]
        [Range(0, 10000, ErrorMessage = "Price must be between $0 and $100,000")]
        public decimal? Price { get; set; } // current market price 

        [Display(Name = "In My Collection")]
        public bool IsOwned { get; set; }

        [Display(Name = "On My Wishlist")]
        public bool IsWanted { get; set; }

        // Add this to associate cards with users email 
        public string UserEmail { get; set; }
    }
}
