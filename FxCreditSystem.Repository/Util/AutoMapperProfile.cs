using AutoMapper;

namespace FxCreditSystem.Repository
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Entities.Transaction, Common.Entities.Transaction>()
                .ForMember(t => t.Id, o => o.MapFrom(t => t.ExternalId));
            CreateMap<Entities.AccountUser, Common.Entities.AccountUser>()
                .ForMember(au => au.AccountId, o => o.MapFrom(au => au.Account.ExternalId))
                .ForMember(au => au.AccountDescription, o => o.MapFrom(au => au.Account.Description))
                .ForMember(au => au.UserId, o => o.MapFrom(au => au.User.UserId))
                .ForMember(au => au.UserDescription, o => o.MapFrom(au => au.User.Description));
        }
    }
}
