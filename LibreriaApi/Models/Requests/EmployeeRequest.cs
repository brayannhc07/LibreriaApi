using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models.Requests {
	public class EmployeeRequest: RequestBase {
		[DisplayName( "Nombre" )]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 45, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Name { get; set; }
		[DisplayName( "Cargo" )]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 20, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Role { get; set; }
		[DisplayName( "Dirección" )]
		[MaxLength( 40, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Address { get; set; }
		[DisplayName( "Número de teléfono" )]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 15, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? PhoneNumber { get; set; }
		[DisplayName( "Correo electrónico" )]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 60, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Email { get; set; }
		[DisplayName( "Fecha de nacimiento" )]
		public DateTime? Birthday { get; set; }
		[DisplayName( "Url de la imagen" )]
		[MaxLength( 500, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? ImageUrl { get; set; }

	}
}
