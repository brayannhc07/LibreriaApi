﻿using LibreriaApi.Interfaces;
using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace LibreriaApi.Services {
	public class EmployeesService: IEmployeesService {
		private readonly MySqlConnection _connection;

		private const string SELECT_COMMAND = "SELECT * FROM empleados_view";
		private const string INSERT_COMMAND = "CALL crearEmpleado(@name, @role, @address, @phone, @email, @birthday, @imageUrl)";
		private const string UPDATE_COMMAND = "CALL editarEmpleado(@name, @role, @address, @phone, @email, @birthday, @imageUrl, @employeeId)";
		private const string DELETE_COMMAND = "CALL eliminarEmpleado(@employeeId)";

		private const string SELECT_BY_ID_COMMAND = "CALL buscarEmpleadoPorId(@employeeId)";

		public EmployeesService( MySqlConnection connection ) {
			_connection = connection;
		}

		public async Task<IEnumerable<EmployeeResponse>> GetAllAsync() {
			using var command = new MySqlCommand( SELECT_COMMAND, _connection );
			using var reader = await command.ExecuteReaderAsync();

			var employees = new List<EmployeeResponse>();

			if( reader.HasRows ) {
				while( await reader.ReadAsync() ) {
					employees.Add( await GetResponseFromReader( reader ) );
				}
			}
			return employees;
		}

		public async Task<EmployeeResponse?> FindByIdAsync( int employeeId ) {
			using var command = new MySqlCommand( SELECT_BY_ID_COMMAND, _connection );
			AddIdParam( command, employeeId );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows ) return null;

			await reader.ReadAsync();
			return await GetResponseFromReader( reader );
		}

		public async Task<EmployeeResponse> CreateAsync( EmployeeRequest request ) {
			using var command = new MySqlCommand( INSERT_COMMAND, _connection );
			AddRequestParams( command, request );

			using var reader = await command.ExecuteReaderAsync();

			if( !reader.HasRows )
				throw new Exception( "No se pudo registrar al empleado, intenta más tarde." );
			
			await reader.ReadAsync();

			return GetResponseFromRequest( await GetIdFromReader( reader ), request );
		}

		public async Task<EmployeeResponse?> UpdateAsync( EmployeeRequest request, int employeeId ) {
			var employee = await FindByIdAsync( employeeId );
			if( employee is null ) return null;

			using var command = new MySqlCommand( UPDATE_COMMAND, _connection );
			AddRequestParams( command, request );
			AddIdParam( command, employeeId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo editar el empleado, intenta más tarde." );

			return GetResponseFromRequest( employeeId, request );
		}

		public async Task<EmployeeResponse?> DeleteAsync( int employeeId ) {
			var employee = await FindByIdAsync( employeeId );
			if( employee is null ) return null;

			using var command = new MySqlCommand( DELETE_COMMAND, _connection );
			AddIdParam( command, employeeId );

			if( await command.ExecuteNonQueryAsync() < 1 )
				throw new Exception( "No se pudo eliminar el empleado, intenta más tarde." );

			return employee;
		}

		private static async Task<int> GetIdFromReader( DbDataReader reader ) {
			return ( int )await reader.GetFieldValueAsync<ulong>( 0 );
		}

		private static async Task<EmployeeResponse> GetResponseFromReader( DbDataReader reader ) {
			return new EmployeeResponse(
				id: await reader.GetFieldValueAsync<int>( "id_empleado" ),
				name: await reader.GetFieldValueAsync<string>( "nombre" ),
				role: await reader.GetFieldValueAsync<string>( "puesto" ),
				address: !await reader.IsDBNullAsync( "direccion" ) ? await reader.GetFieldValueAsync<string?>( "direccion" ) : null,
				phoneNumber: await reader.GetFieldValueAsync<string>( "telefono" ),
				email: await reader.GetFieldValueAsync<string>( "correo" ),
				birthday: !await reader.IsDBNullAsync( "fecha_nacimiento" ) ? await reader.GetFieldValueAsync<DateTime?>( "fecha_nacimiento" ) : null,
				imageUrl: await reader.GetFieldValueAsync<string>( "imagen_url" )
			);
		}

		private static EmployeeResponse GetResponseFromRequest( int id, EmployeeRequest request ) {
			return new EmployeeResponse(
				id: id,
				name: request.Name!,
				role: request.Role!,
				address: request.Address,
				phoneNumber: request.PhoneNumber!,
				email: request.Email!,
				birthday: request.Birthday,
				imageUrl: request.ImageUrl ?? ""
			);
		}

		private static void AddRequestParams( MySqlCommand command, EmployeeRequest request ) {
			command.Parameters.AddWithValue( "@name", request.Name );
			command.Parameters.AddWithValue( "@role", request.Role );
			command.Parameters.AddWithValue( "@address", request.Address );
			command.Parameters.AddWithValue( "@phone", request.PhoneNumber );
			command.Parameters.AddWithValue( "@email", request.Email );
			command.Parameters.AddWithValue( "@birthday", request.Birthday );
			command.Parameters.AddWithValue( "@imageUrl", request.ImageUrl ?? string.Empty );
		}

		private static void AddIdParam( MySqlCommand command, int employeeId ) {
			command.Parameters.AddWithValue( "@employeeId", employeeId );
		}
	}
}
