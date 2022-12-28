using LibreriaApi.Interfaces;
using LibreriaApi.Models;
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

		private const string SELECT_COMMAND = "SELECT * FROM libros ORDER BY titulo DESC";
		private const string INSERT_COMMAND = "INSERT INTO libros(isbn , titulo, autor , sinopsis, editorial , numero_pag, imagen_url) VALUES(@isbn ,@title, @author, @synopsis, @editorial, @pages, @imageUrl)";
		private const string UPDATE_COMMAND = "UPDATE libros SET isbn = @isbn , titulo = @title, autor = @author, sinopsis = @synopsis, editorial = @editorial, numero_pag = @pages, imagen_url = @imageUrl WHERE id_libro = @bookId";
		private const string DELETE_COMMAND = "DELETE FROM libros WHERE id_libro = @bookId";

		private const string SELECT_BY_ID_COMMAND = "SELECT * FROM libros WHERE id_libro = @bookId";

		private const string DELETE_BOOK_GENRES_COMMAND = "DELETE FROM libro_generos WHERE id_libro = @bookId AND id_genero = @genreId";
		private const string INSERT_BOOK_GENRES_COMMAND = "INSERT INTO libro_generos(id_genero, id_libro) VALUES(@genreId, @bookId)";

		public BooksService( MySqlConnection connection, IGenresService genresService ) {
			_connection = connection;
			_genresService = genresService;
		}

		public async Task<IEnumerable<BookResponse>> ReadAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var books = new List<BookResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					var bookId = await reader.GetFieldValueAsync<int>( "id_libro" );
					books.Add( await GetResponseFromReader( reader,
						await _genresService.GetFromBookIdAsync( bookId )
					) );
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
			return await GetResponseFromReader( reader, await _genresService.GetFromBookIdAsync( bookId ) );
		}

		public async Task<BookResponse> CreateAsync( BookRequest request ) {
			using var transaction = await _connection.BeginTransactionAsync();
			using var command = new MySqlCommand( INSERT_COMMAND, _connection, transaction );
			AddRequestParams( command, request );
			int bookId;
			try {
				if( await command.ExecuteNonQueryAsync() < 1 )
					throw new Exception( "No se pudo registrar el socio, intenta más tarde." );

				bookId = ( int )command.LastInsertedId;

				await ManageBookGenres( bookId, request.Genres!, transaction );

				await transaction.CommitAsync();
			} catch( Exception ) {
				await transaction.RollbackAsync();
				throw;
			}
			return GetResponseFromRequest( bookId, request, await _genresService.GetFromBookIdAsync( bookId ) );
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


			return GetResponseFromRequest( bookId, request, await _genresService.GetFromBookIdAsync( bookId ), book.Available );
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

		private static async Task<BookResponse> GetResponseFromReader( DbDataReader reader,
			IEnumerable<GenreResponse> genres ) {
			return new BookResponse(
				id: await reader.GetFieldValueAsync<int>( "id_libro" ),
				isbn: int.Parse( await reader.GetFieldValueAsync<string>( "isbn" ) ),
				title: await reader.GetFieldValueAsync<string>( "titulo" ),
				author: await reader.GetFieldValueAsync<string>( "autor" ),
				synopsis: await reader.GetFieldValueAsync<string>( "sinopsis" ),
				editorial: await reader.GetFieldValueAsync<string>( "editorial" ),
				pages: await reader.GetFieldValueAsync<int>( "numero_pag" ),
				imageUrl: await reader.GetFieldValueAsync<string>( "imagen_url" ),
				available: await reader.GetFieldValueAsync<bool>( "disponible" ),
				genres: genres
			);
		}

		private static BookResponse GetResponseFromRequest( int id, BookRequest request,
			IEnumerable<GenreResponse> genres, bool available = true ) {
			return new BookResponse(
				id: id,
				isbn: request.Isbn ?? 0,
				title: request.Title!,
				author: request.Author!,
				synopsis: request.Synopsis!,
				editorial: request.Editorial!,
				pages: request.Pages ?? 0,
				imageUrl: request.ImageUrl ?? string.Empty,
				available: available,
				genres: genres
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



		private async Task ManageBookGenres( int bookId, IEnumerable<int> genreIds, MySqlTransaction transaction ) {
			genreIds = genreIds.Distinct().Where( x => x > 0 ); // Limpiar lista

			var currentGenreIds = ( await _genresService.GetFromBookIdAsync( bookId ) ).Select( x => x.Id );
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
	}
}
