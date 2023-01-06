using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models.Requests
{
    public class BookRequest : RequestBase
    {

        [DisplayName("ISBN")]
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [Range(0, int.MaxValue, ErrorMessage = RANGE_ERROR_MESSAGE)]
        public int? Isbn { get; set; }
        [DisplayName("Titulo")]
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [MaxLength(35, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE)]
        public string? Title { get; set; }
        [DisplayName("Autor")]
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [MaxLength(500, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE)]
        public string? Author { get; set; }
        [DisplayName("Sinopsis")]
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [MaxLength(250, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE)]
        public string? Synopsis { get; set; }
        [DisplayName("Editorial")]
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [MaxLength(45, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE)]
        public string? Editorial { get; set; }
        [DisplayName("Numero Paginas")]
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [Range(1, int.MaxValue, ErrorMessage = RANGE_ERROR_MESSAGE)]
        public int? Pages { get; set; }
        [DisplayName("Url de la portada")]
        [MaxLength(500, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE)]
        public string? ImageUrl { get; set; }
        [DisplayName("Géneros")]
        [Required(ErrorMessage = "Es necesario asignar los géneros.")]
        [MinLength(1, ErrorMessage = "Necesitas asignar al menos un género.")]
        public IEnumerable<int>? Genres { get; set; }
    }
}
