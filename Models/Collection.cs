using System.ComponentModel.DataAnnotations;

namespace PokemonCardCollector.Models
{
    public class Collection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Collection Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key to the user
        public string? UserEmail { get; set; }
        public List<PokemonCard> Cards { get; set; }
    }
}
