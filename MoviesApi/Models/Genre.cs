
namespace MoviesApi.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // beccause i do not use (int) -- byte do not able me to use this Id as a primarykey
        public byte Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
