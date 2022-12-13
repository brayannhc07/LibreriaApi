using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models {
	public class GenreRequest {

		[DisplayName( "Nombre" )]
		[Required( ErrorMessage = "Es necesario indicar el {0} del género." )]
		[MaxLength( 45, ErrorMessage = "El {0} del género excede el tamaño permitido({1})." )]
		public string? Name { get; set; }
		[DisplayName( "Url de la imagen" )]
		[MaxLength( 250, ErrorMessage = "El {0} del género excede el tamaño permitido({1})." )]
		public string? ImageUrl { get; set; }
	}
}
