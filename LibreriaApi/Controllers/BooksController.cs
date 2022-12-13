using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class BooksController: ControllerBase {
		private readonly IBooksService booksService;

		public BooksController( IBooksService booksService ) {
			this.booksService = booksService;
		}

		[HttpGet]
		public async Task<ActionResult> GetAll() {
			try {
				return Ok( await booksService.ReadAsync() );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult> GetById( int id ) {
			try {
				var book = await booksService.FindByIdAsync( id );

				if( book is null ) return NotFound();

				return Ok( book );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpPost]
		public async Task<ActionResult> Create( BookRequest request ) {
			try {
				var bookId = await booksService.CreateAsync( request );
				var book = await booksService.FindByIdAsync( bookId );

				return Ok( book );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult> Update( int id, BookRequest request ) {
			try {
				var bookId = await booksService.UpdateAsync( request, id );

				if(bookId is null ) return NotFound();

				var book = await booksService.FindByIdAsync( ( int )bookId);

				return Ok( book );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult> Delete( int id ) {
			try {
				var bookId = await booksService.DeleteAsync( id );

				if(bookId is null ) return NotFound();

				return Ok();
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

	}
}
