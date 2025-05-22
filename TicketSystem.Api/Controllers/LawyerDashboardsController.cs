using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket.Repository.Data.AppDbContext;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LawyerDashboardsController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;

		public LawyerDashboardsController(ApplicationDbContext dbContext , IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		[HttpGet("{lawyerId}")]
		public async Task<IActionResult>GetDashBoard(int lawyerId)
		{
			var assignedConsultation = await _dbContext.Consultations
											.Where(x=>x.LawyerId == lawyerId)
											.ToListAsync();

			var totalAssigned = assignedConsultation.Count();
			var totalAnswered = assignedConsultation.Count(x=>!string.IsNullOrEmpty(x.Answer));
			var totalPaid = assignedConsultation.Count(x => x.IsPaid);

			var income = await _dbContext.Payments.Where(x=>x.Consultation.LawyerId == lawyerId && x.PaymentStatus =="Complete")
								.SumAsync(p=>(decimal?)p.Amount) ?? 0;

			var result = new LawyerDashBoardDTO
			{
				TotalAssignedConsultations = totalAssigned,
				TotalAnsweredConsultations = totalAnswered,
				TotalPaidConsultations = totalPaid,
				TotalIncome =income ,
			};

			return Ok(result); 
		}
	}
}
