using LibreriaApi.Interfaces;
using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]
	public class BorrowsController: LibraryControllerBase {
		private readonly IBorrowsService _borrowsService;

		public BorrowsController( IBorrowsService borrowsService ) {
			_borrowsService = borrowsService;
		}
		[HttpGet]
		public async Task<ActionResult<Response<IEnumerable<BorrowResponse>>>> GetAll() {
			Response<IEnumerable<BorrowResponse>> response = new();
			try {
				var borrows = await _borrowsService.GetAllAsync();
				return Ok( response.Commit( "", borrows ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpGet( "{id:int}" )]
		public async Task<ActionResult<Response<BorrowResponse>>> GetById( int id ) {
			Response<BorrowResponse> response = new();
			try {
				var borrow = await _borrowsService.FindByIdAsync( id );

				if( borrow is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "", borrow ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPost]
		public async Task<ActionResult<Response<BorrowResponse>>> Create( BorrowRequest request ) {
			Response<BorrowResponse> response = new();
			try {
				var borrow = await _borrowsService.RegisterAsync( request );

				return Ok( response.Commit( "Préstamo registrado correctamente.", borrow ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		[HttpPost( "{id:int}/devolution" )]
		public async Task<ActionResult<Response<BorrowResponse>>> Devolution( int id ) {
			Response<BorrowResponse> response = new();
			try {
				var borrow = await _borrowsService.DevolutionAsync( id );

				if( borrow is null ) return GetNotFoundStatus( response );

				return Ok( response.Commit( "Devolución registrada correctamente.", borrow ) );
			} catch( Exception ex ) {
				return GetServerErrorStatus( response, ex );
			}
		}

		private ActionResult GetNotFoundStatus<T>( Response<T> response ) {
			return NotFound( response.Defeat( "Préstamo no encontrado." ) );
		}
	}
}
