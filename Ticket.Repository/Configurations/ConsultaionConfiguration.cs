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
	internal class ConsultaionConfiguration : IEntityTypeConfiguration<Consultation>
	{
		public void Configure(EntityTypeBuilder<Consultation> builder)
		{
			builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
			builder.Property(x=>x.Question).IsRequired();
			builder.Property(x=>x.Answer).HasMaxLength(2000);
			builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(x=>x.IsPaid).IsRequired();
			builder.Property(x=>x.IsAnswered).IsRequired();
		
			builder.HasOne(x => x.Client)
				.WithMany(x=>x.SentConsultations)
				.HasForeignKey(x=>x.ClientId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Lawyer)
				.WithMany(x => x.ReceivedConsultations)
				.HasForeignKey(x => x.LawyerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Payment)
				.WithOne(x => x.Consultation)
				.HasForeignKey<Payment>(x => x.ConsultationId);
		


		}
	}
}
