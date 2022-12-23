using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class GenresController: LibraryControllerBase {
		private readonly IGenresService _genresService;

		public GenresController( IGenresService genresService ) {
			this._genresService = genresService;
		}

		[HttpGet]
		public async Task<ActionResult<Response<IEnumerable<GenreResponse>>>> GetAll() {
			Response<IEnumerable<GenreResponse>> response = new();
			try {
				var genres = await _genresService.ReadAsync();
				return Ok( response.Commit( "", genres ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult<Response<GenreResponse>>> GetById( int id ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await _genresService.FindByIdAsync( id );

				if( genre is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "", genre ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPost]
		public async Task<ActionResult<Response<GenreResponse>>> Create( GenreRequest request ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await _genresService.CreateAsync( request );

				return Ok( response.Commit( "Género creado correctamente.", genre ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult<Response<GenreResponse>>> Update( int id, GenreRequest request ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await _genresService.UpdateAsync( request, id );

				if( genre is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Género actualizado correctamente.", genre ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult<Response<GenreResponse>>> Delete( int id ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await _genresService.DeleteAsync( id );

				if( genre is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Género eliminado correctamente.", genre ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		private ActionResult GetNotFoundStatus<T>( Response<T> response ) {
			return NotFound( response.Defeat( "Género no encontrado." ) );
		}
	}
}
