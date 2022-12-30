using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;

namespace LibreriaApi.Interfaces
{
    public interface IEmployeesService {
		Task<IEnumerable<EmployeeResponse>> GetAllAsync();
		Task<EmployeeResponse?> FindByIdAsync( int employeeId );
		Task<EmployeeResponse> CreateAsync( EmployeeRequest request );
		Task<EmployeeResponse?> UpdateAsync( EmployeeRequest request, int employeeId );
		Task<EmployeeResponse?> DeleteAsync( int employeeId );
	}
}
