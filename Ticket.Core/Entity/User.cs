using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket.Core.Entity
{
	public class User : BaseEntity
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string Role { get; set; } // "Client", "Lawyer", "Admin"
		public ICollection<Consultation> SentConsultations { get; set; }
		public ICollection<Consultation> ReceivedConsultations { get; set; }
		public ICollection<Payment> Payments { get; set; }
		
	}
}
