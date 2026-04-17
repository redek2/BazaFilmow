using System.ComponentModel.DataAnnotations;

namespace BazaFilmow.Models
{
    public class Film
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Tytul { get; set; } = string.Empty;
        [MaxLength(150)]
        public string Dlugosc { get; set; }
        [MaxLength(150)]
        public string Rezyser { get; set; } = string.Empty;
        public string? SciezkaOkladki { get; set; }
    }
}
