using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models {
	public class MemberRequest: RequestBase {

		[DisplayName( "Nombre" )]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 45, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Name { get; set; }

		[DisplayName( "Dirección" )]
		//[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 40, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Address { get; set; }
		[DisplayName( "Número de teléfono" )]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[Phone( ErrorMessage = FORMAT_ERROR_MESSAGE )]
		[MaxLength( 10, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? PhoneNumber { get; set; }
		[DisplayName( "Correo electrónico" )]
		//[Required(ErrorMessage = "Es necesario indicar el {0}.")]
		[EmailAddress( ErrorMessage = FORMAT_ERROR_MESSAGE )]
		[MaxLength( 60, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Email { get; set; }
		[DisplayName( "Fecha de nacimiento" )]
		//[Required(ErrorMessage = "Es necesario indicar el {0}.")]
		public DateTime? Birthday { get; set; }
		[DisplayName( "Url de la imagen" )]
		[MaxLength( 250, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? ImageUrl { get; set; }
	}
}
