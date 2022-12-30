using LibreriaApi.Interfaces;
using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers
{
    [Route( "api/[controller]" )]
	[ApiController]
	public class EmployeesController: LibraryControllerBase {
		private readonly IEmployeesService _employeesService;

		public EmployeesController( IEmployeesService employeesService ) {
			_employeesService = employeesService;
		}

		[HttpGet]
		public async Task<ActionResult<Response<IEnumerable<EmployeeResponse>>>> GetAll() {
			Response<IEnumerable<EmployeeResponse>> response = new();
			try {
				var employees = await _employeesService.GetAllAsync();
				return Ok( response.Commit( "", employees ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult<Response<EmployeeResponse>>> GetById( int id ) {
			Response<EmployeeResponse> response = new();
			try {
				var employee = await _employeesService.FindByIdAsync( id );

				if( employee is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "", employee ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}


		[HttpPost]
		public async Task<ActionResult<Response<EmployeeResponse>>> Create( EmployeeRequest request ) {
			Response<EmployeeResponse> response = new();
			try {
				var employee = await _employeesService.CreateAsync( request );

				return Ok( response.Commit( "Empleado registrado correctamente.", employee ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult<Response<EmployeeResponse>>> Update( int id, EmployeeRequest request ) {
			Response<EmployeeResponse> response = new();
			try {
				var employee = await _employeesService.UpdateAsync( request, id );

				if( employee is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Empleado actualizado correctamente.", employee ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult<Response<EmployeeResponse>>> Delete( int id ) {
			Response<EmployeeResponse> response = new();
			try {
				var employee = await _employeesService.DeleteAsync( id );

				if( employee is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Empleado dado de baja correctamente.", employee ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		private ActionResult GetNotFoundStatus( Response<EmployeeResponse> response ) {
			return NotFound( response.Defeat( "Empleado no encontrado." ) );
		}
	}
}
