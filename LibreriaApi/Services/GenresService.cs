using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace LibreriaApi.Services {
	public class GenresService: IGenresService {
		private readonly MySqlConnection _connection;

		private const string SELECT_COMMAND = "SELECT * FROM generos ORDER BY genero DESC";
		private const string INSERT_COMMAND = "INSERT INTO generos(genero, imagen_url) VALUES(@name, @imageUrl)";
		private const string UPDATE_COMMAND = "UPDATE generos SET genero = @name, imagen_url = @imageUrl WHERE id_genero = @genreId";
		private const string DELETE_COMMAND = "DELETE FROM generos WHERE id_genero = @genreId";

		private const string SELECT_BY_ID_COMMAND = "SELECT * FROM generos WHERE id_genero = @genreId";
		private const string SELECT_BY_BOOK_ID_COMMAND = "SELECT g.*, lg.id_libro FROM libro_generos lg, generos g WHERE lg.id_genero = g.id_genero AND id_libro = @bookId ORDER BY genero DESC";

		public GenresService( MySqlConnection connection ) {
			_connection = connection;
		}

		public async Task<IEnumerable<GenreResponse>> ReadAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var genres = new List<GenreResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					genres.Add( await GetResponseByReader( reader ) );
				}
			}
			return genres;
		}

		public async Task<IEnumerable<GenreResponse>> ReadByBookIdAsync( int bookId ) {
			using var command = new MySqlCommand( SELECT_BY_BOOK_ID_COMMAND, _connection );
			command.Parameters.AddWithValue( "@bookId", bookId );

			using var reader = await command.ExecuteReaderAsync();

			var genres = new List<GenreResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					genres.Add( await GetResponseByReader( reader ) );
				}
			}

			return genres;
		}

		public async Task<GenreResponse?> FindByIdAsync( int genreId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			AddGenreIdParam( command, genreId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();
			return await GetResponseByReader( reader );
		}

		public async Task<GenreResponse> CreateAsync( GenreRequest request ) {
			using var command = new MySqlCommand( INSERT_COMMAND, _connection );
			AddRequestParams( command, request );

			await command.ExecuteNonQueryAsync();

			return GetResponseByRequest( ( int )command.LastInsertedId, request );
		}

		public async Task<GenreResponse?> UpdateAsync( GenreRequest request, int genreId ) {
			using var command = new MySqlCommand( UPDATE_COMMAND, _connection );
			AddRequestParams( command, request );
			AddGenreIdParam( command, genreId );

			if( await command.ExecuteNonQueryAsync() < 1 ) return null;

			return GetResponseByRequest( ( int )command.LastInsertedId, request );
		}

		public async Task<int?> DeleteAsync( int genreId ) {
			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			AddGenreIdParam( command, genreId );

			if( await command.ExecuteNonQueryAsync() < 1 ) return null;

			return ( int )command.LastInsertedId;
		}


		private static async Task<GenreResponse> GetResponseByReader( DbDataReader reader ) {
			return new GenreResponse(
				id: await reader.GetFieldValueAsync<int>( "id_genero" ),
				name: await reader.GetFieldValueAsync<string>( "genero" ),
				imageUrl: await reader.GetFieldValueAsync<string>( "imagen_url" )
			);
		}

		private static GenreResponse GetResponseByRequest( int id, GenreRequest request ) {
			return new GenreResponse(
				id: id,
				name: request.Name!,
				imageUrl: request.ImageUrl ?? ""
				);
		}

		private static void AddRequestParams( MySqlCommand command, GenreRequest request ) {
			command.Parameters.AddWithValue( "@name", request.Name );
			command.Parameters.AddWithValue( "@imageUrl", request.ImageUrl ?? string.Empty );
		}

		private static void AddGenreIdParam( MySqlCommand command, int genreId ) {
			command.Parameters.AddWithValue( "@genreId", genreId );
		}
	}
}
