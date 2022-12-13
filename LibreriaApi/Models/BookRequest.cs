using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models {
	public class BookRequest {

        [DisplayName("Isbn")]
        [Range(0, 9999999999999, ErrorMessage = "El {0} del libro excede el tamaño permitido({1}).")]
        public int? Isbn { get; set; }
        [DisplayName( "Titulo" )]
		[Required( ErrorMessage = "Es necesario indicar el {0} del género." )]
		[MaxLength( 45, ErrorMessage = "El {0} del libro excede el tamaño permitido({1})." )]
		public string? Titulo { get; set; }
        [DisplayName("Autor")]
        [MaxLength(250, ErrorMessage = "El {0} del libro excede el tamaño permitido({1}).")]
        public string? Autor { get; set; }
        [DisplayName("Sinopsis")]
        [MaxLength(500, ErrorMessage = "El {0} del libro excede el tamaño permitido({1}).")]
        public string? Sinopsis { get; set; }
        [DisplayName("Editorial")]
        [MaxLength(250, ErrorMessage = "El {0} del libro excede el tamaño permitido({1}).")]
        public string? Editorial { get; set; }
        [DisplayName("Numero Paginas")]
        [Range(0, 20000, ErrorMessage = "El {0} del libro excede el tamaño permitido({1}).")]
        public int? Numero_pag { get; set; }
        [DisplayName( "Url de la portada" )]
		[MaxLength( 250, ErrorMessage = "El {0} del libro excede el tamaño permitido({1}).")]
		public string? ImageUrl { get; set; }
        [DisplayName("status")]
        [Range(typeof(bool),"false","true", ErrorMessage = "El {0} del libro excede el tamaño permitido({1}).")]
        public bool? Status { get; set; }
    }
}
