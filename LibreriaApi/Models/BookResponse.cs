namespace LibreriaApi.Models {
	/// <summary>
	/// Representa un modelo de respuesta para un libro
	/// </summary>
	public class BookResponse {
		public BookResponse(int id, int isbn, string titulo, string autor, string sinopsis, string editorial , int numero_pag, string imageUrl, bool status ) {
			Id = id;
			Titulo = titulo;
			ImageUrl = imageUrl;
			Autor = autor;
			Sinopsis = sinopsis;
			Editorial = editorial;
            Numero_pag = numero_pag;
            Isbn = isbn;
			Status = status;
		}

        public string ImageUrl { get; private set; }
        public int Isbn { get; private set; }
        public int Numero_pag { get; private set; }
        public int Id { get; private set; }
        public string Titulo { get; private set; }
        public string Autor { get; private set; }
        public string Sinopsis { get; private set; }
        public string Editorial { get; private set; }
        public bool Status { get; private set; }
		
	}
}
