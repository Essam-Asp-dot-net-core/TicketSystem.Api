namespace TicketSystem.Api.DTOs
{
	public class CreateConsultationDTO
	{
		public string Title { get; set; }
		public string Question { get; set; }
		public int? LawyerId { get; set; }
	}
}
