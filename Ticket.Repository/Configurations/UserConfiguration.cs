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
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.Property(x=>x.FullName).IsRequired().HasMaxLength(150);
			builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
			builder.Property(x => x.PasswordHash).IsRequired();
			builder.Property(x=>x.Role).IsRequired().HasMaxLength(30);
			
			builder.HasMany(x => x.SentConsultations)
				.WithOne(x => x.Client)
				.HasForeignKey(x => x.ClientId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.ReceivedConsultations)
				.WithOne(x => x.Lawyer)
				.HasForeignKey(x => x.LawyerId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x=>x.Payments)
				.WithOne(x=>x.User)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
