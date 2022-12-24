using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models {
	public class EmployeeRequest: RequestBase {
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 45, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Name { get; set; }
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 20, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Role { get; set; }
		[MaxLength( 40, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Address { get; set; }
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 10, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? PhoneNumber { get; set; }
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MaxLength( 60, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? Email { get; set; }
		public DateTime? Birthday { get; set; }
		[MaxLength( 250, ErrorMessage = MAX_LENGTH_ERROR_MESSAGE )]
		public string? ImageUrl { get; set; }

	}
}
