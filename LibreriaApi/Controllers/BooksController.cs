using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class BooksController: LibraryControllerBase {
		private readonly IBooksService _booksService;

		public BooksController( IBooksService booksService ) {
			this._booksService = booksService;
		}

		[HttpGet]
		public async Task<ActionResult<Response<IEnumerable<BookResponse>>>> GetAll() {
			Response<IEnumerable<BookResponse>> response = new();
			try {
				var books = await _booksService.ReadAsync();
				return Ok( response.Commit( "", books ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult<Response<BookResponse>>> GetById( int id ) {
			Response<BookResponse> response = new();
			try {
				var book = await _booksService.FindByIdAsync( id );

				if( book is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "", book ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPost]
		public async Task<ActionResult<Response<BookResponse>>> Create( BookRequest request ) {
			Response<BookResponse> response = new();
			try {
				var book = await _booksService.CreateAsync( request );

				return Ok( response.Commit( "Libro registrado correctamente.", book ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult<Response<BookResponse>>> Update( int id, BookRequest request ) {
			Response<BookResponse> response = new();
			try {
				var book = await _booksService.UpdateAsync( request, id );

				if( book is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Libro actualizado correctamente.", book ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult<Response<BookResponse>>> Delete( int id ) {
			Response<BookResponse> response = new();
			try {
				var book = await _booksService.DeleteAsync( id );

				if( book is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Libro eliminado correctamente.", book ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		private ActionResult GetNotFoundStatus<T>( Response<T> response ) {
			return NotFound( response.Defeat( "Libro no encontrado." ) );
		}
	}
}
