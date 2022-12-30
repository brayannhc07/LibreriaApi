using LibreriaApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers
{
    public class LibraryControllerBase : ControllerBase {
		public ObjectResult GetServerErrorStatus<T>( Response<T> response, Exception ex ) {
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				response.Defeat( ex.Message )
			);
		}
	}
}
