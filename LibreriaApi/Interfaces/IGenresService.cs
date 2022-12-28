using LibreriaApi.Models;
using MySql.Data.MySqlClient;

namespace LibreriaApi.Interfaces {
	public interface IGenresService {
		/// <summary>
		/// Intenta obtener los géneros disponibles de forma asíncrona.
		/// </summary>
		/// <returns>Devuelve un <see cref="Task"/> que resuelve los géneros encontrados.</returns>
		Task<IEnumerable<GenreResponse>> ReadAsync();
		/// <summary>
		/// Intenta obtener los géneros que pertenecen a un libro de forma asíncrona.
		/// </summary>
		/// <param name="bookId">Id del libro del que se quieren consultar los géneros.</param>
		/// <returns>Devuelve un <see cref="Task"/> que resuelve los géneros del libro encontrados.</returns>
		Task<IEnumerable<GenreResponse>> GetFromBookIdAsync( int bookId );
		/// <summary>
		/// Intenta obtener un género existente por su id de forma asíncrona.
		/// </summary>
		/// <param name="genreId">Id del género que se quiere consultar.</param>
		/// <returns>Devuelve un <see cref="Task"/> que resuelve el género buscado, sino existe devuelve null.</returns>
		Task<GenreResponse?> FindByIdAsync( int genreId );
		/// <summary>
		/// Intenta almacenar un nuevo género de forma asíncrona.
		/// </summary>
		/// <param name="request">Solicitud con los datos del género.</param>
		/// <returns>Devuelve un <see cref="Task"/> que resuelve los datos del género creado.</returns>
		Task<GenreResponse> CreateAsync( GenreRequest request );
		/// <summary>
		/// Intenta actualizar los datos de un género existente de forma asíncrona.
		/// </summary>
		/// <param name="request">Solicitud con los nuevos datos del género.</param>
		/// <param name="genreId">Id del género que se quiere editar.</param>
		/// <returns>Devuelve un <see cref="Task"/> que resuelve los datos del género editado, sino se editó devuelve null.</returns>
		Task<GenreResponse?> UpdateAsync( GenreRequest request, int genreId );
		/// <summary>
		/// Intenta eliminar un género existente de forma asíncrona.
		/// </summary>
		/// <param name="genreId">Id del género que se quiere eliminar.</param>
		/// <returns>Devuelve un <see cref="Task"/> que resuelve los datos del género eliminado, sino se eliminó devuelve null.</returns>
		Task<GenreResponse?> DeleteAsync( int genreId );
	}
}
