using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Core.Entity;

namespace Ticket.Repository.Configurations
{
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)");
			builder.Property(x => x.PaidAt).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(x => x.PaymentStatus).IsRequired().HasMaxLength(50);
			builder.Property(x => x.PaymentMethod).HasMaxLength(100);
			
			builder.HasOne(x=>x.User)
				.WithMany(x=>x.Payments)
				.HasForeignKey(x=>x.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			
			builder.HasOne(x=>x.Consultation)
				.WithOne(x=>x.Payment)
				.HasForeignKey<Payment>(x=>x.ConsultationId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
