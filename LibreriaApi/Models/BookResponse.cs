namespace LibreriaApi.Models {
	/// <summary>
	/// Representa un modelo de respuesta para un libro
	/// </summary>
	public class BookResponse {
		public BookResponse( int id, int isbn, string title, string author, string synopsis, string editorial, 
			int pages, string imageUrl, bool available, IEnumerable<GenreResponse> genres ) {
			Id = id;
			Title = title;
			ImageUrl = imageUrl;
			Author = author;
			Synopsis = synopsis;
			Editorial = editorial;
			Pages = pages;
			Isbn = isbn;
			Available = available;
			Genres = genres;
		}

		public string ImageUrl { get; private set; }
		public int Isbn { get; private set; }
		public int Pages { get; private set; }
		public int Id { get; private set; }
		public string Title { get; private set; }
		public string Author { get; private set; }
		public string Synopsis { get; private set; }
		public string Editorial { get; private set; }
		public bool Available { get; private set; }
		public IEnumerable<GenreResponse> Genres { get; private set; }

	}
}
