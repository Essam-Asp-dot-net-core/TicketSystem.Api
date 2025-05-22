using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket.Core.Entity
{
	public class Consultation : BaseEntity
	{
		public string Title { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public bool IsPaid { get; set; }
		public bool IsAnswered { get; set; }

		public int ClientId { get; set; }
		public User Client { get; set; }

		public int? LawyerId { get; set; }
		public User Lawyer { get; set; }

		public Payment Payment { get; set; }
	}
}
