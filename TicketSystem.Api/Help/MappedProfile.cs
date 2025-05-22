using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;
using Ticket.Core.Entity;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Help
{
	public class MappedProfile : Profile
	{
		public MappedProfile() 
		{ 
			CreateMap<Consultation ,ConsultationResponseDTO>()
				.ForMember(des=>des.ClientName , opt=>opt.MapFrom(src=>src.Client.FullName))
				.ForMember(des=>des.LawyerName , opt=>opt.MapFrom(src=>src.Lawyer.FullName));

			CreateMap<CreateConsultationDTO, Consultation>();

			CreateMap<Payment, PaymentResponseDTO>()
				.ForMember(x=>x.UserName , x=>x.MapFrom(x=>x.User.FullName))
				.ForMember(x=>x.ConsultationTitle , x=>x.MapFrom(x=>x.Consultation.Title));
		}
	}
}
