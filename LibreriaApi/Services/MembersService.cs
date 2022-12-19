using LibreriaApi.Interfaces;
using LibreriaApi.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;

namespace LibreriaApi.Services {
	public class MembersService: IMembersService {
		private readonly MySqlConnection _connection;

		private const string SELECT_COMMAND = "SELECT * FROM socios ORDER BY nombre DESC";
		private const string INSERT_COMMAND = "INSERT INTO socios(nombre, direccion, telefono, correo, fecha_nacimiento, imagen_url) VALUES(@name, @address, @phone, @email, @birthday, @imageUrl)";
		private const string UPDATE_COMMAND = "UPDATE socios SET nombre = @name, direccion = @address, telefono = @phone, correo = @email, fecha_nacimiento = @birthday, imagen_url = @imageUrl WHERE id_socio = @memberId";
		private const string DELETE_COMMAND = "DELETE FROM socios WHERE id_socio = @memberId";

		private const string SELECT_BY_ID_COMMAND = "SELECT * FROM socios WHERE id_socio = @memberId";

		public MembersService( MySqlConnection connection ) {
			_connection = connection;
		}

		public async Task<IEnumerable<MemberResponse>> ReadAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var genres = new List<MemberResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					genres.Add( await GetResponseFromReader( reader ) );
				}
			}
			return genres;
		}

		public async Task<MemberResponse?> FindByIdAsync( int genreId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			AddIdParam( command, genreId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();
			return await GetResponseFromReader( reader );
		}

		public async Task<MemberResponse> CreateAsync( MemberRequest request ) {
			using var command = new MySqlCommand( INSERT_COMMAND, _connection );
			AddRequestParams( command, request );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo registrar al socio, intenta más tarde." );

			return GetResponseFromRequest( ( int )command.LastInsertedId, request );
		}

		public async Task<MemberResponse?> UpdateAsync( MemberRequest request, int memberId ) {
			var member = await FindByIdAsync( memberId );
			if( member is null ) return null;

			using var command = new MySqlCommand( UPDATE_COMMAND, _connection );
			AddRequestParams( command, request );
			AddIdParam( command, memberId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo editar el socio, intenta más tarde." );

			return GetResponseFromRequest( memberId, request, member.ActiveMembership );
		}

		public async Task<MemberResponse?> DeleteAsync( int memberId ) {
			var member = await FindByIdAsync( memberId );
			if( member is null ) return null;

			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			AddIdParam( command, memberId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo eliminar el socio, intenta más tarde." );

			return member;
		}


		private static async Task<MemberResponse> GetResponseFromReader( DbDataReader reader ) {
			return new MemberResponse(
				id: await reader.GetFieldValueAsync<int>( "id_socio" ),
				name: await reader.GetFieldValueAsync<string>( "nombre" ),
				address: await reader.GetFieldValueAsync<string>( "direccion" ),
				phoneNumber: await reader.GetFieldValueAsync<string>( "telefono" ),
				email: await reader.GetFieldValueAsync<string>( "correo" ),
				birth: await reader.GetFieldValueAsync<DateTime>( "fecha_nacimiento" ),
				activeMembership: await reader.GetFieldValueAsync<bool>( "estado_membresia" ),
				imageUrl: await reader.GetFieldValueAsync<string>( "imagen_url" )
			);
		}

		private static MemberResponse GetResponseFromRequest( int id, MemberRequest request, bool activeMembership = true ) {
			return new MemberResponse(
				id: id,
				name: request.Name!,
				address: request.Address ?? "",
				phoneNumber: request.PhoneNumber!,
				email: request.Email ?? "",
				birth: request.Birthday,
				activeMembership: activeMembership,
				imageUrl: request.ImageUrl ?? ""
			);
		}

		private static void AddRequestParams( MySqlCommand command, MemberRequest request ) {
			command.Parameters.AddWithValue( "@name", request.Name );
			command.Parameters.AddWithValue( "@address", request.Address ?? string.Empty );
			command.Parameters.AddWithValue( "@phone", request.PhoneNumber );
			command.Parameters.AddWithValue( "@email", request.Email ?? string.Empty );
			command.Parameters.AddWithValue( "@name", request.Birthday );
			command.Parameters.AddWithValue( "@imageUrl", request.ImageUrl ?? string.Empty );
		}

		private static void AddIdParam( MySqlCommand command, int memberId ) {
			command.Parameters.AddWithValue( "@memberId", memberId );
		}
	}
}
