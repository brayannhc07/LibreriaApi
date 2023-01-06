using LibreriaApi.Interfaces;
using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Net;

namespace LibreriaApi.Services {
	public class BorrowsService: IBorrowsService {
		private readonly MySqlConnection _connection;
		private readonly IMembersService _membersService;
		private readonly IEmployeesService _employeesService;
		private readonly IBooksService _booksService;
		private readonly IDevolutionsService _devolutionsService;
		private const string SELECT_COMMAND = "SELECT * FROM prestamos ORDER BY id_prestamo desc";
		private const string INSERT_COMMAND = "INSERT INTO prestamos(id_socio, id_empleado, fecha_lim_entrega) VALUES(@memberId, @employeeId, @limitDate)";
		private const string DELETE_COMMAND = "DELETE FROM prestamos WHERE id_prestamo = @borrowId";

		private const string SELECT_BY_ID_COMMAND = "SELECT * FROM prestamos WHERE id_prestamo = @borrowId";
		private const string INSERT_BORROW_BOOKS_COMMAND = "INSERT INTO prestamo_libros(id_prestamo, id_libro) VALUES(@borrowId, @bookId)";

		public BorrowsService( MySqlConnection connection, IMembersService membersService,
			IEmployeesService employeesService, IBooksService booksService, IDevolutionsService devolutionsService ) {
			_connection = connection;
			_membersService = membersService;
			_employeesService = employeesService;
			_booksService = booksService;
			_devolutionsService = devolutionsService;
		}

		public async Task<BorrowResponse?> FindByIdAsync( int borrowId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			AddBorrowIdParam( command, borrowId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();
			return await GetResponseFromReader( reader );
		}

		public async Task<IEnumerable<BorrowResponse>> GetAllAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var borrows = new List<BorrowResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					borrows.Add( await GetResponseFromReader( reader ) );
				}
			}
			return borrows;
		}

		public async Task<BorrowResponse> RegisterAsync( BorrowRequest request ) {
			using var transaction = await _connection.BeginTransactionAsync();
			using var command = new MySqlCommand( INSERT_COMMAND, _connection, transaction );
			AddRequestParams( command, request );
			int borrowId;
			try {
				var member = await _membersService.FindByIdAsync( request.MemberId ?? 0 );

				if( member is null )
					throw new Exception( "El socio no se pudo encontrar." );

				var employee = await _employeesService.FindByIdAsync( request.EmployeeId ?? 0 );

				if( employee is null )
					throw new Exception( "El empleado no se pudo encontrar." );

				if( await command.ExecuteNonQueryAsync() < 1 )
					throw new Exception( "No se pudo registrar el préstamo, intenta más tarde." );

				borrowId = ( int )command.LastInsertedId;

				await SetBorrowBooks( borrowId, request.Books!, transaction );

				await transaction.CommitAsync();
			} catch( Exception ) {
				await transaction.RollbackAsync();
				throw;
			}
			return ( await FindByIdAsync( borrowId ) )!;
		}

		public async Task<BorrowResponse?> UnregisterAsync( int borrowId ) {
			var borrow = await FindByIdAsync( borrowId );
			if( borrow is null ) return null;

			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			AddBorrowIdParam( command, borrowId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo cancelar el préstamo, intenta más tarde." );

			return borrow;
		}

		public async Task<BorrowResponse?> DevolutionAsync( int borrowId ) {
			var borrow = await FindByIdAsync( borrowId );

			if( borrow is null ) return null;

			if( borrow.Devolution is not null ) 
				throw new Exception( "Los libros de este préstamo ya se han devuelto." );

			borrow.Devolution = await _devolutionsService.RegisterAsync( borrowId );

			return borrow;
		}

		private async Task SetBorrowBooks( int borrowId, IEnumerable<int> bookIds, MySqlTransaction transaction ) {
			bookIds = bookIds.Distinct().Where( x => x > 0 ); // Limpiar lista

			if( !bookIds.Any() ) throw new Exception( "Debes asignar al menos un libro con un id válido." );

			using var addBooksCommand = new MySqlCommand( INSERT_BORROW_BOOKS_COMMAND,
				_connection, transaction );

			// Agregar géneros
			foreach( var bookId in bookIds ) {
				var book = await _booksService.FindByIdAsync( bookId );

				if( book is null )
					throw new Exception( "El libro no está disponible, intenta de nuevo" );

				if( !book.Available )
					throw new Exception( $"El libro {book.Title} ya ha sido prestado." );

				AddBorrowIdParam( addBooksCommand, borrowId );
				AddBookIdParam( addBooksCommand, bookId );

				await addBooksCommand.ExecuteNonQueryAsync();

				addBooksCommand.Parameters.Clear();
			}
		}

		private async Task<BorrowResponse> GetResponseFromReader( DbDataReader reader ) {
			var id = await reader.GetFieldValueAsync<int>( "id_prestamo" );
			return new BorrowResponse(
				id: id,
				member: ( await _membersService.FindByIdAsync( await reader.GetFieldValueAsync<int>( "id_socio" ) ) )!,
				employee: ( await _employeesService.FindByIdAsync( await reader.GetFieldValueAsync<int>( "id_empleado" ) ) )!,
				borrowDate: await reader.GetFieldValueAsync<DateTime>( "fecha_prestamo" ),
				limitDate: await reader.GetFieldValueAsync<DateTime>( "fecha_lim_entrega" ),
				books: await _booksService.GetByBorrowIdAsync( id ),
				devolution: await _devolutionsService.FindByBorrowIdAsync( id )
			);
		}

		private static void AddRequestParams( MySqlCommand command, BorrowRequest request ) {
			command.Parameters.AddWithValue( "@memberId", request.MemberId );
			command.Parameters.AddWithValue( "@employeeId", request.EmployeeId );
			command.Parameters.AddWithValue( "@limitDate", request.LimitDate );
		}

		private static void AddBorrowIdParam( MySqlCommand command, int borrowId ) {
			command.Parameters.AddWithValue( "@borrowId", borrowId );
		}

		private static void AddBookIdParam( MySqlCommand command, int bookId ) {
			command.Parameters.AddWithValue( "@bookId", bookId );
		}
	}
}
