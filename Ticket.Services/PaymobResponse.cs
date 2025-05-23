using System.Text.Json.Serialization;

namespace Ticket.Services
{
	public class AuthResponse
	{
		[JsonPropertyName("token")]
		public string Token { get; set; }

		public class OrderResponse
		{
			[JsonPropertyName("id")]
			public int Id { get; set; }
		}

		public class PaymentKeyResponse
		{
			[JsonPropertyName("token")]
			public string Token { get; set; }
		}

		public class PaymobWebhookPayload
		{
			public PaymobObj obj { get; set; }

			public class PaymobObj
			{
				public bool success { get; set; }
				public PaymobOrder order { get; set; }
				public Source source { get; set; }
				public int amount_cents { get; set; }
			}

			public class PaymobOrder
			{
				public int id { get; set; }
			}

			public class Source
			{
				public string sub_type { get; set; }
			}

		}
	}
}