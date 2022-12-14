namespace LibreriaApi.Models {
	public class MemberResponse {
		public MemberResponse( int id, string name, string address, string phoneNumber, string email, 
			DateTime? birth, string imageUrl, bool activeMembership ) {
			Id = id;
			Name = name;
			Address = address;
			PhoneNumber = phoneNumber;
			Email = email;
			Birth = birth;
			ImageUrl = imageUrl;
			ActiveMembership = activeMembership;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public DateTime? Birth { get; set; }
		public string ImageUrl { get; set; }
		public bool ActiveMembership { get; set; }
	}
}
