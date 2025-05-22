using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket.Core.Entity
{
	public class Payment : BaseEntity
	{
		public decimal Amount { get; set; }
		public DateTime PaidAt { get; set; } = DateTime.UtcNow;
		public string PaymentStatus { get; set; }
		public string PaymentMethod { get; set; }

		public int UserId { get; set; }
		public User User { get; set; }

		public int ConsultationId { get; set; }
		public Consultation Consultation { get; set; }
	}
}
