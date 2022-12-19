using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class BooksController: LibraryControllerBase {
		private readonly IBooksService booksService;

		public BooksController( IBooksService booksService ) {
			this.booksService = booksService;
		}

		[HttpGet]
		public async Task<ActionResult> GetAll() {
			Response<IEnumerable<BookResponse>> response = new();
			try {
				var books = await booksService.ReadAsync();
				return Ok( response.Commit( "", books ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult> GetById( int id ) {
			Response<BookResponse> response = new();
			try {
				var book = await booksService.FindByIdAsync( id );

				if( book is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "", book ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPost]
		public async Task<ActionResult> Create( BookRequest request ) {
			Response<BookResponse> response = new();
			try {
				var book = await booksService.CreateAsync( request );

				return Ok( response.Commit( "Libro registrado correctamente.", book ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult> Update( int id, BookRequest request ) {
			Response<BookResponse> response = new();
			try {
				var book = await booksService.UpdateAsync( request, id );

				if( book is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Libro actualizado correctamente.", book ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult> Delete( int id ) {
			Response<BookResponse> response = new();
			try {
				var book = await booksService.DeleteAsync( id );

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
