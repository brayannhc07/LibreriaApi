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
			try {
				return Ok( await genresService.ReadAsync() );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult> GetById( int id ) {
			try {
				var genre = await genresService.FindByIdAsync( id );

				if( genre is null ) return NotFound();

				return Ok( genre );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpPost]
		public async Task<ActionResult> Create( GenreRequest request ) {
			try {
				var genreId = await genresService.CreateAsync( request );
				var genre = await genresService.FindByIdAsync( genreId );

				return Ok( genre );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpPut( "{id:int}" )]
		public async Task<ActionResult> Update( int id, GenreRequest request ) {
			try {
				var genreId = await genresService.UpdateAsync( request, id );

				if( genreId is null ) return NotFound();

				var genre = await genresService.FindByIdAsync( ( int )genreId );

				return Ok( genre );
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

		[HttpDelete( "{id:int}" )]
		public async Task<ActionResult> Delete( int id ) {
			try {
				var genreId = await genresService.DeleteAsync( id );

				if( genreId is null ) return NotFound();

				return Ok();
			} catch( Exception ex ) {
				return StatusCode( StatusCodes.Status500InternalServerError, new { message = ex.Message } );
			}
		}

	}
}
