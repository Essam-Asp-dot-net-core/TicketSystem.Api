using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Ticket.Services.AuthResponse;

namespace Ticket.Services
{
	public class PaymobServices
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public PaymobServices(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_configuration = configuration;
		}

		public async Task<string> GetAuthTokenAsync()
		{
			var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", new { api_Key = _configuration["Paymob:ApiKey"] });

			//var result = await response.Content.ReadFrom();
			var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

			return result.Token;

		}

		public async Task<int> CreateOrderAsync(string token, decimal amountCents)
		{
			var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", new
			{
				auth_token = token,
				delivery_needed = false,
				amount_cents = (int)(amountCents * 100),
				currency = "EGP",
				items = Array.Empty<object>()
			});

			var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
			return result.Id;
		}

		public async Task<string> GetPaymentKeyAsync(string token, int orderId, decimal amountCents, string email, string phone)
		{
			var billingData = new
			{
				first_name = "Client",
				last_name = "Name",
				email,
				phone_number = phone,
				city = "Cairo",
				country = "EG",
				street = "Test St",
				building = "123"
			};

			var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", new
			{
				auth_token = token,
				amount_cents = (int)(amountCents * 100),
				expiration = 3600,
				order_id = orderId,
				billing_data = billingData,
				currency = "EGP",
				integration_id = int.Parse(_configuration["Paymob:IntegrationId"])
			});

			var result = await response.Content.ReadFromJsonAsync<PaymentKeyResponse>();
			return result.Token;

		}

		public string GetCardPaymentUrl(string paymentKey)
		{
			var iframeId = _configuration["Paymob:IframeId"];
			return $"https://accept.paymob.com/api/acceptance/iframes/{_configuration["Paymob:IframeId"]}?payment_token={paymentKey}";
		}


	}
}
