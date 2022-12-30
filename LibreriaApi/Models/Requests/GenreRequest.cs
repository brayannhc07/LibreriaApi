using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models.Requests
{
    public class GenreRequest : RequestBase
    {

        [DisplayName("Nombre")]
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [MaxLength(45, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE)]
        public string? Name { get; set; }
        [DisplayName("Url de la imagen")]
        [MaxLength(500, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE)]
        public string? ImageUrl { get; set; }
    }
}
