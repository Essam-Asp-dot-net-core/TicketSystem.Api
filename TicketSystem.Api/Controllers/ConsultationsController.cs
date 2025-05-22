using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket.Core.Entity;
using Ticket.Repository.Data.AppDbContext;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConsultationsController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;

		public ConsultationsController(ApplicationDbContext dbContext , IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		//انشاء استشارة من العميل

		[HttpPost]
		public async Task<IActionResult> CreateConsultation([FromBody] CreateConsultationDTO model, [FromQuery]int ClientId)
		{
			var Client = await _dbContext.Users.FindAsync(ClientId);
			if (Client == null || Client.Role != "Client")
				return BadRequest("Client not found or unauthorized");



			var consultation = _mapper.Map<Consultation>(model);

				consultation.Title = model.Title;
				consultation.Question = model.Question;
				consultation.ClientId = ClientId;
				consultation.LawyerId = model.LawyerId;
				consultation.CreatedAt = DateTime.UtcNow;
				consultation.IsPaid = false;
				consultation.IsAnswered = false;
			
			await _dbContext.AddAsync(consultation);
			await _dbContext.SaveChangesAsync();
			var result = _mapper.Map<ConsultationResponseDTO>(consultation);
			return CreatedAtAction(nameof(GetConsultationById) , new {id = consultation.Id} , result);

		}

		//عرض استشارة معينة

		[HttpGet("{id}")]
		public async Task<IActionResult> GetConsultationById(int id)
		{
			var consultation = await _dbContext.Consultations
				.Include(c => c.Client)
				.Include(c => c.Lawyer)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (consultation == null)
				return NotFound("Consultation not found");

			var result = _mapper.Map<ConsultationResponseDTO>(consultation);
			return Ok(result);
		}


		//عرض استشارة العميل على المحامى
		[HttpGet("Client/{clientId}")]

		public async Task<IActionResult> GetClientCosultations (int clientId)
		{
			var exists = await _dbContext.Users.AnyAsync(x=>x.Id == clientId && x.Role =="Client");
			if (!exists) return NotFound("Client not found");

			var consultations = await _dbContext.Consultations
									.Where(p => p.ClientId == clientId)
									.Include(p => p.Lawyer)
									.Include(x=>x.Client)
									.ToListAsync();
			var result = _mapper.Map<List<ConsultationResponseDTO>>(consultations);
			return Ok(result);
		}


		//عرض استشارات المحامى 
		[HttpGet("Lawyer/{lawyerId}")]
		public async Task<IActionResult> AnswerConsultation(int lawyerId)
		{

			var LawyerExists = await _dbContext.Users.AnyAsync(x=>x.Id == lawyerId && x.Role == "Lawyer");
			if (!LawyerExists)
				return NotFound("Lawyer not found");

			var consultations = await _dbContext.Consultations
									.Where(x=>x.LawyerId == lawyerId)
									.Include(x=>x.Client)
									.Include (x=>x.Lawyer)
									.ToListAsync();
			var result = _mapper.Map<List<ConsultationResponseDTO>>(consultations);
			return Ok(result);
		
		}

		//الرد على استشارة العميل من المحامى
		[HttpPut("{id}/answer")]
		public async Task<IActionResult> AnswerConsultation(int id ,[FromBody] AnswerConsultationDTO dto)
		{
			var consultation = await _dbContext.Consultations.FindAsync(id);
			if (consultation == null) return NotFound("Consultation not found");

			consultation.Answer = dto.Answer;
			consultation.IsAnswered = true;
			await _dbContext.SaveChangesAsync();
			return Ok("Consultation answered successfully");
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteConsultation(int id)
		{
			var consultation = await _dbContext.Consultations.FindAsync(id);
			if(consultation == null) return NotFound();

			_dbContext.Remove(consultation);
			await _dbContext.SaveChangesAsync();

			return NoContent();
		}


	}
}
