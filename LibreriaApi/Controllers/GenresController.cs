using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class GenresController: LibraryControllerBase {
		private readonly IGenresService genresService;

		public GenresController( IGenresService genresService ) {
			this.genresService = genresService;
		}

		[HttpGet]
		public async Task<ActionResult> GetAll() {
			Response<IEnumerable<GenreResponse>> response = new();
			try {
				var genres = await genresService.ReadAsync();
				return Ok( response.Commit( "", genres ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult> GetById( int id ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.FindByIdAsync( id );

				if( genre is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "", genre ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPost]
		public async Task<ActionResult> Create( GenreRequest request ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.CreateAsync( request );

				return Ok( response.Commit( "Género creado correctamente.", genre ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult> Update( int id, GenreRequest request ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.UpdateAsync( request, id );

				if( genre is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Género actualizado correctamente.", genre ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult> Delete( int id ) {
			Response<GenreResponse> response = new();
			try {
				var genre = await genresService.DeleteAsync( id );

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
