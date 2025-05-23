using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket.Core.Entity;
using Ticket.Repository.Data.AppDbContext;
using Ticket.Services;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentsController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;
		private readonly PaymobServices _paymobServices;

		public PaymentsController(ApplicationDbContext dbContext , IMapper mapper , PaymobServices paymobServices)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_paymobServices = paymobServices;
		}

		[HttpPost]
		public async Task<IActionResult> CreatePayment(CreatePaymentDTO dto)
		{
			var user = await _dbContext.Users.FindAsync(dto.UserId);
			if (user == null ) return BadRequest("User not found.");
			var consultations = await _dbContext.Consultations.FindAsync(dto.ConsultationId);
			if (consultations == null) return BadRequest("Consultation not found.");
			if (dto.Amount <= 0) return BadRequest("InValid Payment Amount");
			if (consultations.IsPaid) return BadRequest("Consultation is already paid.");
			var payment = new Payment
			{
				Amount = dto.Amount,
				PaidAt = DateTime.UtcNow,
				PaymentMethod = dto.PaymentMethod,
				PaymentStatus = dto.PaymentStatus,
				ConsultationId = dto.ConsultationId,
				UserId = dto.UserId,
			};

			consultations.IsPaid = true;

			await _dbContext.AddAsync(payment);
			await _dbContext.SaveChangesAsync();

			var mappedResult = _mapper.Map<PaymentResponseDTO>(payment); 
			return Ok(mappedResult);

		}


		[HttpGet]
		public async Task<IActionResult>GetAllPaid()
		{
			var payments = await _dbContext.Payments
								.Include(x => x.User)
								.Include(x => x.Consultation)
								.ToListAsync();

			if (!payments.Any()) return NotFound("No Payment Found");
			var result = _mapper.Map<List<PaymentResponseDTO>>(payments); 
			return Ok(result);

		}

		[HttpGet("User/{userId}")]
		public async Task<IActionResult>GetByUser(int  userId)
		{
			var payments = await _dbContext.Payments
								.Where(x => x.UserId == userId)
								.Include(x=>x.Consultation)
								.ToListAsync();

			if (!payments.Any()) return NotFound("Payment Is Not Found");
			var result = _mapper.Map<PaymentResponseDTO>(payments);
			return Ok(result);
		}

		[HttpGet("Consultation/{consultationId}")]
		public async Task<IActionResult>GetByConsultation(int consultationId)
		{
			var payments = await _dbContext.Payments
								.Where(x => x.ConsultationId == consultationId)
								.Include(x => x.User)
								.FirstOrDefaultAsync();
			if(payments == null)
			return NotFound("Payment Is Not Found");
			var result = _mapper.Map<PaymentResponseDTO>(payments);
			return Ok(result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePayment(int id)
		{
			var payment = await _dbContext.Payments.FindAsync(id);
			if (payment == null)
				return NotFound();

			_dbContext.Payments.Remove(payment);
			await _dbContext.SaveChangesAsync();

			return NoContent();
		}

		[HttpPost("pay-via-card")]
		public async Task<IActionResult> PayViaCard([FromQuery] decimal amount, [FromQuery] string email, [FromQuery] string phone)
		{
			if (amount <= 0)
				return BadRequest("Amount must be greater than 0");

			var token = await _paymobServices.GetAuthTokenAsync();
			var orderId = await _paymobServices.CreateOrderAsync(token, amount);
			var paymentKey = await _paymobServices.GetPaymentKeyAsync(token, orderId, amount, email, phone);
			var paymentUrl = _paymobServices.GetCardPaymentUrl(paymentKey);

			return Ok(new { paymentUrl });
		}


		[HttpPost("webhook")]
		public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookDTO webhook)
		{
			if (webhook.obj.success == true && webhook.obj.is_refunded == false)
			{
				var consultationId = webhook.obj.order.id; // أو استخدم custom fields لربطها
				var consultation = await _dbContext.Consultations.FindAsync(consultationId);

				if (consultation == null)
					return NotFound("Consultation not found");

				// منع التكرار
				var existingPayment = await _dbContext.Payments
					.FirstOrDefaultAsync(p => p.ConsultationId == consultationId);

				if (existingPayment != null)
					return Ok("Payment already exists");

				var payment = new Payment
				{
					Amount = webhook.obj.amount_cents / 100m,
					PaidAt = DateTime.UtcNow,
					PaymentMethod = "Card",
					PaymentStatus = "Paid",
					ConsultationId = consultationId,
					UserId = consultation.ClientId
				};

				consultation.IsPaid = true;

				await _dbContext.Payments.AddAsync(payment);
				await _dbContext.SaveChangesAsync();

				return Ok("Payment recorded successfully");
			}

			return BadRequest("Invalid transaction");
		}


	}
}
