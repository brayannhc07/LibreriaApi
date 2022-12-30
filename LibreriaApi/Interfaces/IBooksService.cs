using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;

namespace LibreriaApi.Interfaces
{
    public interface IBooksService {
		Task<IEnumerable<BookResponse>> GetAllAsync();
		Task<IEnumerable<BookResponse>> GetByBorrowIdAsync( int borrowId );
		Task<BookResponse?> FindByIdAsync( int bookId );
		Task<BookResponse> CreateAsync( BookRequest request );
		Task<BookResponse?> UpdateAsync( BookRequest request, int bookId );
		Task<BookResponse?> DeleteAsync( int bookId );

	}
}
