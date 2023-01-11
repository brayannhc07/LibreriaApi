using LibreriaApi.Interfaces;
using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace LibreriaApi.Services {
	public class GenresService: IGenresService {
		private readonly MySqlConnection _connection;

		private const string SELECT_COMMAND = "SELECT * FROM generos_view";
		private const string INSERT_COMMAND = "CALL crearGenero(@name, @imageUrl)";
		private const string UPDATE_COMMAND = "CALL editarGenero(@name, @imageUrl, @genreId)";
		private const string DELETE_COMMAND = "CALL eliminarGenero(@genreId)";

		private const string SELECT_BY_ID_COMMAND = "CALL buscarGeneroPorId(@genreId)";
		private const string SELECT_BY_BOOK_ID_COMMAND = "CALL buscarGenerosPorIdLibro(@bookId)";

		public GenresService( MySqlConnection connection ) {
			_connection = connection;
		}

		public async Task<IEnumerable<GenreResponse>> GetAllAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var genres = new List<GenreResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					genres.Add( await GetResponseFromReader( reader ) );
				}
			}
			return genres;
		}

		public async Task<IEnumerable<GenreResponse>> GetByBookIdAsync( int bookId ) {
			using var command = new MySqlCommand( SELECT_BY_BOOK_ID_COMMAND, _connection );
			AddBookIdParam( command, bookId );

			using var reader = await command.ExecuteReaderAsync();

			var genres = new List<GenreResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					genres.Add( await GetResponseFromReader( reader ) );
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
			return await GetResponseFromReader( reader );
		}

		public async Task<GenreResponse> CreateAsync( GenreRequest request ) {
			using var command = new MySqlCommand( INSERT_COMMAND, _connection );
			AddRequestParams( command, request );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows )
				throw new Exception( "No se pudo registrar el género, intenta más tarde." );

			await reader.ReadAsync();

			return GetResponseByRequest( await GetIdFromReader( reader ), request );
		}

		public async Task<GenreResponse?> UpdateAsync( GenreRequest request, int genreId ) {

			var genre = await FindByIdAsync( genreId );

			if( genre is null ) return null;

			using var command = new MySqlCommand( UPDATE_COMMAND, _connection );
			AddRequestParams( command, request );
			AddGenreIdParam( command, genreId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo editar el género, intenta más tarde." );

			return GetResponseByRequest( genreId, request );
		}

		public async Task<GenreResponse?> DeleteAsync( int genreId ) {

			var genre = await FindByIdAsync( genreId );

			if( genre is null ) return null;

			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			AddGenreIdParam( command, genreId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo elminar el género, intenta más tarde." );

			return genre;
		}

		private static async Task<int> GetIdFromReader( DbDataReader reader ) {
			return ( int )await reader.GetFieldValueAsync<ulong>( 0 );
		}

		private static async Task<GenreResponse> GetResponseFromReader( DbDataReader reader ) {
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

		private static void AddBookIdParam( MySqlCommand command, int bookId ) {
			command.Parameters.AddWithValue( "@bookId", bookId );
		}

	}
}
