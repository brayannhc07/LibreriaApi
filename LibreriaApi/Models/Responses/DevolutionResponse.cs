namespace LibreriaApi.Models.Responses {
	public class DevolutionResponse {
		public DevolutionResponse( int id, DateTime devolutionTime ) {
			Id = id;
			DevolutionTime = devolutionTime;
		}

		public int Id { get; private set; }
		public DateTime DevolutionTime { get; private set; }
	}
}
