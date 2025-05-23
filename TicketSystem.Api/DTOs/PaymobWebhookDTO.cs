namespace TicketSystem.Api.DTOs
{
	public class PaymobWebhookDTO
	{
		public WebhookObj obj { get; set; }

		public class WebhookObj
		{
			public bool success { get; set; }
			public bool is_refunded { get; set; }
			public int amount_cents { get; set; }
			public Order order { get; set; }
		}

		public class Order
		{
			public int id { get; set; }
		}
	}
}
