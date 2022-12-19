using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace LibreriaApi.Services {
	public class BooksService: IBooksService {
		private readonly MySqlConnection _connection;

		private const string SELECT_COMMAND = "SELECT * FROM libros ORDER BY titulo DESC";
		private const string INSERT_COMMAND = "INSERT INTO libros(isbn , titulo, autor , sinopsis, editorial , numero_pag, imagen_url) VALUES(@isbn ,@title, @author, @synopsis, @editorial, @pages, @imageUrl)";
		private const string UPDATE_COMMAND = "UPDATE libros SET isbn = @isbn , titulo = @title, autor = @author, sinopsis = @synopsis, editorial = @editorial, numero_pag = @pages, imagen_url = @imageUrl WHERE id_libro = @bookId";
		private const string DELETE_COMMAND = "DELETE FROM libros WHERE id_libro = @bookId";

		private const string SELECT_BY_ID_COMMAND = "SELECT * FROM libros WHERE id_libro = @bookId";

		public BooksService( MySqlConnection connection ) {
			_connection = connection;
		}

		public async Task<IEnumerable<BookResponse>> ReadAsync() {
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

		public async Task<BookResponse?> FindByIdAsync( int bookId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			AddIdParam( command, bookId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();
			return await GetResponseFromReader( reader );
		}

		public async Task<BookResponse> CreateAsync( BookRequest request ) {
			using var command = new MySqlCommand( INSERT_COMMAND, _connection );
			AddRequestParams( command, request );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo registrar al socio, intenta más tarde." );

			return GetResponseFromRequest( ( int )command.LastInsertedId, request );
		}

		public async Task<BookResponse?> UpdateAsync( BookRequest request, int bookId ) {
			var book = await FindByIdAsync( bookId );
			if( book is null ) return null;

			using var command = new MySqlCommand( UPDATE_COMMAND, _connection );
			AddRequestParams( command, request );
			AddIdParam( command, bookId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo editar el libro, intenta más tarde." );

			return GetResponseFromRequest( bookId, request, book.Available );
		}

		public async Task<BookResponse?> DeleteAsync( int bookId ) {
			var book = await FindByIdAsync( bookId );
			if( book is null ) return null;

			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			AddIdParam( command, bookId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo eliminar el libro, intenta más tarde." );

			return book;
		}

		private static async Task<BookResponse> GetResponseFromReader( DbDataReader reader ) {
			return new BookResponse(
				id: await reader.GetFieldValueAsync<int>( "id_libro" ),
				isbn: int.Parse( await reader.GetFieldValueAsync<string>( "isbn" ) ),
				title: await reader.GetFieldValueAsync<string>( "titulo" ),
				author: await reader.GetFieldValueAsync<string>( "autor" ),
				synopsis: await reader.GetFieldValueAsync<string>( "sinopsis" ),
				editorial: await reader.GetFieldValueAsync<string>( "editorial" ),
				pages: await reader.GetFieldValueAsync<int>( "numero_pag" ),
				imageUrl: await reader.GetFieldValueAsync<string>( "imagen_url" ),
				available: await reader.GetFieldValueAsync<bool>( "disponible" )
			);
		}

		private static BookResponse GetResponseFromRequest( int id, BookRequest request, bool available = true ) {
			return new BookResponse(
				id: id,
				isbn: request.Isbn ?? 0,
				title: request.Title!,
				author: request.Author!,
				synopsis: request.Synopsis!,
				editorial: request.Editorial!,
				pages: request.Pages ?? 0,
				imageUrl: request.ImageUrl ?? string.Empty,
				available: available
			);
		}

		private static void AddRequestParams( MySqlCommand command, BookRequest request ) {
			command.Parameters.AddWithValue( "@isbn", request.Isbn );
			command.Parameters.AddWithValue( "@title", request.Title );
			command.Parameters.AddWithValue( "@author", request.Author );
			command.Parameters.AddWithValue( "@synopsis", request.Synopsis );
			command.Parameters.AddWithValue( "@editorial", request.Editorial );
			command.Parameters.AddWithValue( "@pages", request.Pages );
			command.Parameters.AddWithValue( "@imageUrl", request.ImageUrl ?? string.Empty );
		}

		private static void AddIdParam( MySqlCommand command, int bookId ) {
			command.Parameters.AddWithValue( "@bookId", bookId );
		}
	}
}
