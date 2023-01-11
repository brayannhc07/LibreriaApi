using LibreriaApi.Interfaces;
using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;
using System.Data.Common;
using System.Xml;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibreriaApi.Services {
	public class DevolutionsService: IDevolutionsService {
		private readonly MySqlConnection _connection;

		private const string INSERT_COMMAND = "CALL crearDevolucion(@borrowId);";

		private const string SELECT_BY_ID_COMMAND = "CALL buscarEntregaPorId(@devolutionId)";
		private const string SELECT_BY_BORROW_ID_COMMAND = "CALL buscarEntregaPorIdPrestamo(@borrowId)";

		public DevolutionsService( MySqlConnection connection ) {
			_connection = connection;
		}

		public async Task<DevolutionResponse?> FindByIdAsync( int devolutionId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			AddDevolutionIdParam( command, devolutionId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();

			return await GetResponseFromReader( reader );
		}

		public async Task<DevolutionResponse?> FindByBorrowIdAsync( int borrowId ) {
			using var command = new MySqlCommand( SELECT_BY_BORROW_ID_COMMAND, _connection );
			AddBorrowIdParam( command, borrowId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();

			return await GetResponseFromReader( reader );
		}

		public async Task<DevolutionResponse> RegisterAsync( int borrowId ) {
			using var command = new MySqlCommand( INSERT_COMMAND, _connection );
			AddBorrowIdParam( command, borrowId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows )
				throw new Exception( "No se pudo registrar la devolución, intenta más tarde." );

			await reader.ReadAsync();

			int devolutionId = await GetIdFromReader( reader );

			await reader.DisposeAsync();

			return ( await FindByIdAsync( devolutionId ) )!;
		}

		private static async Task<int> GetIdFromReader( DbDataReader reader ) {
			return ( int )await reader.GetFieldValueAsync<ulong>( 0 );
		}

		private static async Task<DevolutionResponse> GetResponseFromReader( DbDataReader reader ) {
			return new DevolutionResponse(
				id: await reader.GetFieldValueAsync<int>( "id_entrega" ),
				devolutionTime: await reader.GetFieldValueAsync<DateTime>( "fecha_entrega" )
			);
		}

		private static void AddBorrowIdParam( MySqlCommand command, int borrowId ) {
			command.Parameters.AddWithValue( "@borrowId", borrowId );
		}
		private static void AddDevolutionIdParam( MySqlCommand command, int devolutionId ) {
			command.Parameters.AddWithValue( "@devolutionId", devolutionId );
		}
	}
}
