namespace TicketSystem.Api.DTOs
{
	public class ClientDashboardDTO
	{
		public int TotalConsultations { get; set; }
		public int AnsweredConsultations { get; set; }
		public int PaidConsultations { get; set; }
		public decimal TotalPayments { get; set; }

	}
}
