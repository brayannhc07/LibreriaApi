using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;

namespace LibreriaApi.Interfaces
{
    public interface IBorrowsService {
		Task<BorrowResponse> RegisterAsync(BorrowRequest request);
		Task<BorrowResponse?> DevolutionAsync(int borrowId );
		Task<BorrowResponse?> FindByIdAsync( int borrowId );
		Task<IEnumerable<BorrowResponse>> GetAllAsync();
	}
}
