namespace TicketSystem.Api.DTOs
{
	public class ConsultationResponseDTO
	{
		public string Title { get; set; }
		public string Question { get; set; }
		public string? Answer { get; set; }
		public string ClientName { get; set; }
		public string LawyerName { get; set; }
		public bool IsAnswered { get; set; }
		public DateTime CreatedAt { get; set; }

	}
}

