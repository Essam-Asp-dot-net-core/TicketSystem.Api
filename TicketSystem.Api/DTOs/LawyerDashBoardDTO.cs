namespace TicketSystem.Api.DTOs
{
	public class LawyerDashBoardDTO
	{
		public int TotalAssignedConsultations { get; set; }
		public int TotalAnsweredConsultations { get; set; }
		public int TotalPaidConsultations { get; set; }
		public decimal TotalIncome { get; set; }

	}
}
