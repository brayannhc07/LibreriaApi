using LibreriaApi.Models;

namespace LibreriaApi.Interfaces {
	public interface IBooksService {
		Task<IEnumerable<BookResponse>> ReadAsync();
		Task<BookResponse?> FindByIdAsync( int bookId );
		Task<BookResponse> CreateAsync( BookRequest request );
		Task<BookResponse?> UpdateAsync( BookRequest request, int bookId );
		Task<BookResponse?> DeleteAsync( int bookId );

	}
}
