namespace TicketSystem.Api.DTOs
{
	public class PaymentResponseDTO
	{
		public int Id { get; set; }
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; } = string.Empty;
		public string PaymentStatus { get; set; } = string.Empty;
		public DateTime PaidAt { get; set; }

		public string UserName { get; set; } = string.Empty;
		public string ConsultationTitle { get; set; } = string.Empty;
	}
}
