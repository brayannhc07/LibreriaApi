using LibreriaApi.Models.Responses;

namespace LibreriaApi.Interfaces {
	public interface IDevolutionsService {
		Task<DevolutionResponse> RegisterAsync(int borrowId);
		Task<DevolutionResponse?> FindByIdAsync( int devolutionId );
		Task<DevolutionResponse?> FindByBorrowIdAsync( int borrowId );
	}
}
