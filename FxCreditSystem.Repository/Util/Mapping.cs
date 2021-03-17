using AutoMapper;

namespace FxCreditSystem.Repository
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Entities.Transaction, Common.Entities.Transaction>();
            CreateMap<Entities.AccountUser, Common.Entities.AccountUser>()
                .ForMember(au => au.AccountExternalId, o => o.MapFrom(au => au.Account.ExternalId))
                .ForMember(au => au.AccountDescription, o => o.MapFrom(au => au.Account.Description))
                .ForMember(au => au.AuthUserId, o => o.MapFrom(au => au.User.AuthUserId))
                .ForMember(au => au.UserDescription, o => o.MapFrom(au => au.User.Description));
        }
    }
}
