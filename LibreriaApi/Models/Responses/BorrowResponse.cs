namespace LibreriaApi.Models.Responses {
	public class BorrowResponse {
		public BorrowResponse( int id, MemberResponse member, EmployeeResponse employee,
			DateTime borrowDate, DateTime limitDate, IEnumerable<BookResponse> books,
			DevolutionResponse? devolution ) {
			Id = id;
			Member = member;
			Employee = employee;
			BorrowDate = borrowDate;
			LimitDate = limitDate;
			Books = books;
			Devolution = devolution;
		}

		public int Id { get; private set; }
		public MemberResponse Member { get; private set; }
		public EmployeeResponse Employee { get; private set; }
		public DateTime BorrowDate { get; private set; }
		public DateTime LimitDate { get; private set; }
		public IEnumerable<BookResponse> Books { get; private set; }
		public DevolutionResponse? Devolution { get; set; }
	}
}
