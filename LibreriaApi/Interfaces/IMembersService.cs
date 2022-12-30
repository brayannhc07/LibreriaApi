using LibreriaApi.Models.Requests;
using LibreriaApi.Models.Responses;

namespace LibreriaApi.Interfaces
{
    public interface IMembersService {
		Task<IEnumerable<MemberResponse>> GetAllAsync();
		Task<MemberResponse?> FindByIdAsync( int memberId);
		Task<MemberResponse> CreateAsync( MemberRequest request );
		Task<MemberResponse?> UpdateAsync( MemberRequest request, int memberId );
		Task<MemberResponse?> DeleteAsync( int memberId );

	}
}
