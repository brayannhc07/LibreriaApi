using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models.Requests {
	public class BorrowRequest: RequestBase {
		[DisplayName("Id del socio")]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		public int? MemberId { get; set; }
		[DisplayName( "Id del empleado" )]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		public int? EmployeeId { get; set; }
		[DisplayName("Fecha límite de entrega")]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		public DateTime? LimitDate { get; set; }
		[DisplayName("Libros")]
		[Required( ErrorMessage = REQUIRED_ERROR_MESSAGE )]
		[MinLength( 1, ErrorMessage = "Debes asignar al menos un libro." )]
		public IEnumerable<int>? Books { get; set; }
	}
}
