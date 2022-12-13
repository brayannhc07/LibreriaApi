namespace LibreriaApi.Models {
	/// <summary>
	/// Representa un modelo de respuesta para un género.
	/// </summary>
	public class GenreResponse {
		public GenreResponse(int id, string name, string imageUrl ) {
			Id = id;
			Name = name;
			ImageUrl = imageUrl;
		}

		/// <summary>
		/// Obtiene el identificador del género.
		/// </summary>
		public int Id { get; private set; }
		/// <summary>
		/// Obtiene el nombre del género.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Obtiene la url de la imagen del género.
		/// </summary>
		public string ImageUrl { get; private set; }
	}
}
