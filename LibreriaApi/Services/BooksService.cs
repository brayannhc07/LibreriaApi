using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace LibreriaApi.Services {
	public class BooksService: IBooksService {
		private readonly MySqlConnection _connection;

		private const string SELECT_COMMAND = "SELECT * FROM libros ORDER BY titulo DESC";
		private const string INSERT_COMMAND = "INSERT INTO libros(isbn , titulo, autor , sinopsis, editorial , numero_pag, imagen_url, status) VALUES( @isbn , @titulo,@autor , @sinopsis, @editorial , @numero_pag , @imagen_url, @status)";
		private const string UPDATE_COMMAND = "UPDATE libros SET isbn = @isbn , titulo = @titulo, autor = @autor, sinopis = @sinopsis, editorial = @editorial, numPaginas = @numPaginas, imagen_url = @imagen_url , status =@status WHERE id_libro = @libroId";
		private const string DELETE_COMMAND = "DELETE FROM libros WHERE id_libro = @libroId";

		private const string SELECT_BY_ID_COMMAND = "SELECT * FROM libros WHERE id_libro = @libroId";
		private const string SELECT_BY_BOOK_ID_COMMAND = "SELECT g.*, lg.id_libro FROM libro_generos lg, generos g WHERE lg.id_genero = g.id_genero AND id_libro = @bookId ORDER BY genero DESC";

		public BooksService( MySqlConnection connection ) {
			_connection = connection;
		}

		public async Task<IEnumerable<BookResponse>> ReadAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var books = new List<BookResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					books.Add( await GetResponse( reader ) );
				}
			}
			return books;
		}

		public async Task<IEnumerable<BookResponse>> ReadByBookIdAsync( int bookId ) {
			using var command = new MySqlCommand( SELECT_BY_BOOK_ID_COMMAND, _connection );
			command.Parameters.AddWithValue( "@bookId", bookId );

			using var reader = await command.ExecuteReaderAsync();

			var book = new List<BookResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					book.Add( await GetResponse( reader ) );
				}
			}

			return book;
		}

		public async Task<BookResponse?> FindByIdAsync( int bookId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			command.Parameters.AddWithValue( "@bookId", bookId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();
			return await GetResponse( reader );
		}

		public async Task<int> CreateAsync( BookRequest request ) {
			using var command = new MySqlCommand( INSERT_COMMAND, _connection );
            command.Parameters.AddWithValue("@isbn", request.Isbn);
            command.Parameters.AddWithValue( "@titulo", request.Titulo );
            command.Parameters.AddWithValue("@autor", request.Autor);
            command.Parameters.AddWithValue("@sinopsis", request.Sinopsis);
            command.Parameters.AddWithValue("@editorial", request.Editorial);
            command.Parameters.AddWithValue("@numPaginas", request.Numero_pag);
            command.Parameters.AddWithValue("@imageUrl", request.ImageUrl ?? string.Empty);
            command.Parameters.AddWithValue("@status", request.Status);
            await command.ExecuteNonQueryAsync();

			return ( int )command.LastInsertedId;
		}

		public async Task<int?> UpdateAsync( BookRequest request, int bookId ) {
			using var command = new MySqlCommand( UPDATE_COMMAND, _connection );
            command.Parameters.AddWithValue("@isbn", request.Isbn);
            command.Parameters.AddWithValue("@titulo", request.Titulo);
            command.Parameters.AddWithValue("@autor", request.Autor);
            command.Parameters.AddWithValue("@sinopsis", request.Sinopsis);
            command.Parameters.AddWithValue("@editorial", request.Editorial);
            command.Parameters.AddWithValue("@numPaginas", request.Numero_pag);
            command.Parameters.AddWithValue("@imageUrl", request.ImageUrl ?? string.Empty);
            command.Parameters.AddWithValue("@status", request.Status);

            if ( await command.ExecuteNonQueryAsync() < 1 ) return null;

			return ( int )command.LastInsertedId;
		}

		public async Task<int?> DeleteAsync( int bookId ) {
			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			command.Parameters.AddWithValue( "@bookId", bookId );

			if( await command.ExecuteNonQueryAsync() < 1 ) return null;

			return ( int )command.LastInsertedId;
		}

		private static async Task<BookResponse> GetResponse( DbDataReader reader ) {
			return new BookResponse(
				id: await reader.GetFieldValueAsync<int>( "id_libro" ),
                isbn: await reader.GetFieldValueAsync<int>("isbn"),
                titulo: await reader.GetFieldValueAsync<string>( "titulo" ),
				autor: await reader.GetFieldValueAsync<string>( "autor" ),
                sinopsis: await reader.GetFieldValueAsync<string>("sinopsis"),
                editorial: await reader.GetFieldValueAsync<string>("editorial"),
                numero_pag: await reader.GetFieldValueAsync<int>("numPaginas"),
                imageUrl: await reader.GetFieldValueAsync<string>("imagen_url"),
                status: await reader.GetFieldValueAsync<bool>("status")
            );
		}
	}
}
