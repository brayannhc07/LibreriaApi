using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class MembersController: ControllerBase {
		private readonly IMembersService membersService;

		public MembersController( IMembersService membersService ) {
			this.membersService = membersService;
		}

		[HttpGet]
		public async Task<ActionResult> GetAll() {
			Response<IEnumerable<MemberResponse>> response = new();
			try {
				var members = await membersService.ReadAsync();
				return Ok( response.Commit( "", members ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult> GetById( int id ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.FindByIdAsync( id );

				if( member is null ) return NotFound( response.Defeat( "Socio no encontrado." ) );

				return Ok( response.Commit( "", member ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpPost]
		public async Task<ActionResult> Create( MemberRequest request ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.CreateAsync( request );

				return Ok( response.Commit( "Socio creado correctamente.", member ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult> Update( int id, MemberRequest request ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.UpdateAsync( request, id );

				if( member is null ) return NotFound( response.Defeat( "Socio no encontrado." ) );

				return Ok( response.Commit( "Socio actualizado correctamente.", member ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult> Delete( int id ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.DeleteAsync( id );

				if( member is null ) return NotFound( response.Defeat( "Socio no encontrado." ) );

				return Ok( response.Commit( "Socio eliminado correctamente.", member ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}
	}
}
