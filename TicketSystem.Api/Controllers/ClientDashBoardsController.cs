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
		public async Task<IActionResult>GetDashboard(int clientId , DateTime? From = null , DateTime? To = null)
		{
			var ClientConsultations = _dbContext.Consultations
											.Where(x => x.ClientId == clientId);

			if(From.HasValue)
				ClientConsultations = ClientConsultations.Where(c=>c.CreatedAt >= From.Value);
			if(To.HasValue)
				ClientConsultations = ClientConsultations.Where(x=>x.CreatedAt <= To.Value);

			var consultation = await ClientConsultations.ToListAsync();

			var total = consultation.Count;
			var paid = consultation.Count(x=>x.IsPaid);
			var answer = consultation.Count(x=>!string.IsNullOrEmpty(x.Answer));

			var payments = _dbContext.Payments.Where(x => x.UserId == clientId && x.PaymentStatus == "Completed");

			if(From.HasValue)
				payments = payments.Where(x=>x.PaidAt >=  From.Value);
			if(To.HasValue)
				payments = payments.Where(x=>x.PaidAt <= To.Value);

			var paymentQuery = await payments.SumAsync(x=>(decimal?)x.Amount)??0;

			var result = new ClientDashboardDTO
			{
				TotalConsultations = total,
				PaidConsultations = paid,
				AnsweredConsultations = answer,
				TotalPayments = paymentQuery
			};

			return Ok(result);

		}

	}
}
