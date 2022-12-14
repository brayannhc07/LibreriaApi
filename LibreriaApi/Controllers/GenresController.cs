using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class GenresController: ControllerBase {
		private readonly IGenresService genresService;

		public GenresController( IGenresService genresService ) {
			this.genresService = genresService;
		}

		[HttpGet]
		public async Task<ActionResult> GetAll() {
			Response<IEnumerable<GenreResponse>> response = new();
			try {
				var users = await genresService.ReadAsync();
				return Ok( response.Commit( "", users ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult> GetById( int id ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.FindByIdAsync( id );

				if( genre is null ) return NotFound( response.Defeat( "Género no encontrado." ) );

				return Ok( response.Commit( "", genre ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpPost]
		public async Task<ActionResult> Create( GenreRequest request ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.CreateAsync( request );

				return Ok( response.Commit( "Género creado correctamente.", genre ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult> Update( int id, GenreRequest request ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.UpdateAsync( request, id );

				if( genre is null ) return NotFound( response.Defeat( "Género no encontrado." ) );

				return Ok( response.Commit( "Género actualizado correctamente.", genre ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult> Delete( int id ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.DeleteAsync( id );

				if( genre is null ) return NotFound( response.Defeat( "Género no encontrado." ) );

				return Ok( response.Commit( "Género elimindo correctamente.", genre ) );
			} catch( Exception ex ) {
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					response.Defeat( ex.Message )
				);
			}
		}

	}
}
