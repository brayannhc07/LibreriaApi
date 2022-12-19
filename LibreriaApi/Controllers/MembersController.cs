using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class MembersController: LibraryControllerBase {
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
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult> GetById( int id ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.FindByIdAsync( id );

				if( member is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "", member ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}


		[HttpPost]
		public async Task<ActionResult> Create( MemberRequest request ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.CreateAsync( request );

				return Ok( response.Commit( "Socio registrado correctamente.", member ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult> Update( int id, MemberRequest request ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.UpdateAsync( request, id );

				if( member is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Socio actualizado correctamente.", member ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult> Delete( int id ) {
			Response<MemberResponse> response = new();
			try {
				var member = await membersService.DeleteAsync( id );

				if( member is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Socio dado de baja correctamente.", member ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		private ActionResult GetNotFoundStatus( Response<MemberResponse> response ) {
			return NotFound( response.Defeat( "Socio no encontrado." ) );
		}
	}
}
