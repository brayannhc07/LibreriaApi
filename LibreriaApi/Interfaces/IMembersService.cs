using LibreriaApi.Models;

namespace LibreriaApi.Interfaces {
	public interface IMembersService {
		Task<IEnumerable<MemberResponse>> ReadAsync();
		Task<MemberResponse?> FindByIdAsync( int memberId);
		Task<MemberResponse> CreateAsync( MemberRequest request );
		Task<MemberResponse?> UpdateAsync( MemberRequest request, int memberId );
		Task<MemberResponse?> DeleteAsync( int memberId );

	}
}
