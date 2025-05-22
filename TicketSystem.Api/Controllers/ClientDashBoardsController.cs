using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket.Repository.Data.AppDbContext;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClientDashBoardsController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;

		public ClientDashBoardsController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		[HttpGet("{clientId}")]
		public async Task<IActionResult>GetDashboard(int clientId)
		{
			var ClientConsultations = await _dbContext.Consultations
											.Where(x => x.ClientId == clientId)
											.ToListAsync();
		
			var total = ClientConsultations.Count;
			var paid = ClientConsultations.Count(x=>x.IsPaid);
			var answer = ClientConsultations.Count(x=>!string.IsNullOrEmpty(x.Answer));
			var payments = await _dbContext.Payments.Where(x => x.UserId == clientId && x.PaymentStatus == "Completed")
							.SumAsync(x => (decimal?)x.Amount) ?? 0 ;

			var result = new ClientDashboardDTO
			{
				TotalConsultations = total,
				PaidConsultations = paid,
				AnsweredConsultations = answer,
				TotalPayments = payments
			};

			return Ok(result);

		}

	}
}
