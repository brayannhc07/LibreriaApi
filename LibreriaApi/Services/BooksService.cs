using LibreriaApi.Interfaces;
using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Transactions;

namespace LibreriaApi.Services {
	public class BooksService: IBooksService {
		private readonly MySqlConnection _connection;
		private readonly IGenresService _genresService;

		private const string SELECT_COMMAND = "SELECT * FROM libros_view";
		private const string INSERT_COMMAND = "CALL crearLibro(@isbn ,@title, @author, @synopsis, @editorial, @pages, @imageUrl, @count)";
		private const string UPDATE_COMMAND = "CALL editarLibro(@isbn, @title, @author, @synopsis, @editorial, @pages, @imageUrl, @bookId, @count)";
		private const string DELETE_COMMAND = "CALL eliminarLibro(@bookId)";

		private const string SELECT_BY_ID_COMMAND = "CALL buscarLibroPorId(@bookId)";
		private const string SELECT_BY_BORROW_ID_COMMAND = "CALL buscarLibrosPorIdPrestamo(@borrowId)";

		private const string DELETE_BOOK_GENRES_COMMAND = "CALL eliminarLibroGenero(@bookId, @genreId)";
		private const string INSERT_BOOK_GENRES_COMMAND = "CALL crearLibroGenero(@bookId, @genreId)";

		public BooksService( MySqlConnection connection, IGenresService genresService ) {
			_connection = connection;
			_genresService = genresService;
		}

		public async Task<IEnumerable<BookResponse>> GetAllAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var books = new List<BookResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					books.Add( await GetResponseFromReader( reader ) );
				}
			}
			return books;
		}

		public async Task<IEnumerable<BookResponse>> GetByBorrowIdAsync( int borrowId ) {
			using var command = new MySqlCommand( SELECT_BY_BORROW_ID_COMMAND, _connection );
			AddBorrowIdParam( command, borrowId );

			using var reader = await command.ExecuteReaderAsync();

			var books = new List<BookResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					books.Add( await GetResponseFromReader( reader ) );
				}
			}

			return books;
		}

		public async Task<BookResponse?> FindByIdAsync( int bookId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			AddBookIdParam( command, bookId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();
			return await GetResponseFromReader( reader );
		}

		public async Task<BookResponse> CreateAsync( BookRequest request ) {
			using var transaction = await _connection.BeginTransactionAsync();
			using var command = new MySqlCommand( INSERT_COMMAND, _connection, transaction );
			AddRequestParams( command, request );
			int bookId;
			try {

				using var reader = await command.ExecuteReaderAsync();

				if( !reader.HasRows )
					throw new Exception( "No se pudo registrar el socio, intenta más tarde." );

				await reader.ReadAsync();

				bookId = await GetIdFromReader( reader );

				await reader.DisposeAsync();

				await ManageBookGenres( bookId, request.Genres!, transaction );

				await transaction.CommitAsync();
			} catch( Exception ) {
				await transaction.RollbackAsync();
				throw;
			}
			return await GetResponseFromRequest( bookId, request );
		}

		public async Task<BookResponse?> UpdateAsync( BookRequest request, int bookId ) {
			var book = await FindByIdAsync( bookId );
			if( book is null ) return null;

			using var transaction = await _connection.BeginTransactionAsync();
			using var command = new MySqlCommand( UPDATE_COMMAND, _connection, transaction );
			AddRequestParams( command, request );
			AddBookIdParam( command, bookId );

			try {
				if( await command.ExecuteNonQueryAsync() < 1 )
					throw new Exception( "No se pudo editar el libro, intenta más tarde." );

				await ManageBookGenres( bookId, request.Genres!, transaction );

				await transaction.CommitAsync();
			} catch( Exception ) {
				await transaction.RollbackAsync();
				throw;
			}


			return await GetResponseFromRequest( bookId, request );
		}

		public async Task<BookResponse?> DeleteAsync( int bookId ) {
			var book = await FindByIdAsync( bookId );
			if( book is null ) return null;

			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			AddBookIdParam( command, bookId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo eliminar el libro, intenta más tarde." );

			return book;
		}

		private async Task<BookResponse> GetResponseFromReader( DbDataReader reader ) {
			var id = await reader.GetFieldValueAsync<int>( "id_libro" );
			return new BookResponse(
				id: id,
				isbn: int.Parse( await reader.GetFieldValueAsync<string>( "isbn" ) ),
				title: await reader.GetFieldValueAsync<string>( "titulo" ),
				author: await reader.GetFieldValueAsync<string>( "autor" ),
				synopsis: await reader.GetFieldValueAsync<string>( "sinopsis" ),
				editorial: await reader.GetFieldValueAsync<string>( "editorial" ),
				pages: await reader.GetFieldValueAsync<int>( "numero_pag" ),
				count: await reader.GetFieldValueAsync<int>( "existencias" ),
				imageUrl: await reader.GetFieldValueAsync<string>( "imagen_url" ),
				genres: await _genresService.GetByBookIdAsync( id )
			);
		}

		private async Task<BookResponse> GetResponseFromRequest( int id, BookRequest request) {
			return new BookResponse(
				id: id,
				isbn: request.Isbn ?? 0,
				title: request.Title!,
				author: request.Author!,
				synopsis: request.Synopsis!,
				editorial: request.Editorial!,
				pages: request.Pages ?? 0,
				imageUrl: request.ImageUrl ?? string.Empty,
				count: request.Count ?? 0,
				genres: await _genresService.GetByBookIdAsync( id )
			);
		}

		private static async Task<int> GetIdFromReader( DbDataReader reader ) {
			return ( int )await reader.GetFieldValueAsync<ulong>( 0 );
		}

		private static void AddRequestParams( MySqlCommand command, BookRequest request ) {
			command.Parameters.AddWithValue( "@isbn", request.Isbn );
			command.Parameters.AddWithValue( "@title", request.Title );
			command.Parameters.AddWithValue( "@author", request.Author );
			command.Parameters.AddWithValue( "@synopsis", request.Synopsis );
			command.Parameters.AddWithValue( "@editorial", request.Editorial );
			command.Parameters.AddWithValue( "@pages", request.Pages );
			command.Parameters.AddWithValue( "@count", request.Count);
			command.Parameters.AddWithValue( "@imageUrl", request.ImageUrl ?? string.Empty );
		}

		private async Task ManageBookGenres( int bookId, IEnumerable<int> genreIds, MySqlTransaction transaction ) {
			genreIds = genreIds.Distinct().Where( x => x > 0 ); // Limpiar lista

			if( !genreIds.Any() ) throw new Exception( "Debes asignar al menos un género con un id válido." );

			var currentGenreIds = ( await _genresService.GetByBookIdAsync( bookId ) ).Select( x => x.Id );
			var insertGenreIds = genreIds.Except( currentGenreIds );
			var deleteGenreIds = currentGenreIds.Except( genreIds );

			using var removeBooksCommand = new MySqlCommand( DELETE_BOOK_GENRES_COMMAND,
				_connection, transaction );
			using var addBooksCommand = new MySqlCommand( INSERT_BOOK_GENRES_COMMAND,
				_connection, transaction );

			// Agregar géneros
			foreach( var genreId in insertGenreIds ) {
				AddBookIdParam( addBooksCommand, bookId );
				AddGenreIdParam( addBooksCommand, genreId );

				await addBooksCommand.ExecuteNonQueryAsync();

				addBooksCommand.Parameters.Clear();
			}

			// Borrar géneros
			foreach( var genreId in deleteGenreIds ) {
				AddBookIdParam( removeBooksCommand, bookId );
				AddGenreIdParam( removeBooksCommand, genreId );

				await removeBooksCommand.ExecuteNonQueryAsync();

				removeBooksCommand.Parameters.Clear();
			}
		}

		private static void AddBookIdParam( MySqlCommand command, int bookId ) {
			command.Parameters.AddWithValue( "@bookId", bookId );
		}

		private static void AddGenreIdParam( MySqlCommand command, int genreId ) {
			command.Parameters.AddWithValue( "@genreId", genreId );
		}

		private static void AddBorrowIdParam( MySqlCommand command, int borrowId ) {
			command.Parameters.AddWithValue( "@borrowId", borrowId );
		}
	}
}
