namespace TicketSystem.Api.DTOs
{
	public class CreatePaymentDTO
	{
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; }
		public string PaymentStatus { get; set; } = "Pending";
		public int UserId { get; set; }
		public int ConsultationId { get; set; }
	}
}
