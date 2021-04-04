using AutoMapper;

namespace FxCreditSystem.Repository
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Entities.Transaction, Common.Entities.Transaction>()
                .ForMember(t => t.Id, o => o.MapFrom(t => t.ExternalId));
            CreateMap<Entities.AccountUserLink, Common.Entities.AccountUser>()
                .ForMember(au => au.AccountId, o => o.MapFrom(au => au.Account.ExternalId))
                .ForMember(au => au.AccountDescription, o => o.MapFrom(au => au.Account.Description))
                .ForMember(au => au.UserId, o => o.MapFrom(au => au.User.ExternalId))
                .ForMember(au => au.UserDescription, o => o.MapFrom(au => au.User.Description));
            CreateMap<Entities.UserIdentity, Common.Entities.UserIdentity>()
                .ForMember(ui => ui.UserId, o => o.MapFrom(ui => ui.User.ExternalId))
                .ForMember(ui => ui.Identity, o => o.MapFrom(ui => ui.Identity));
            CreateMap<Entities.Account, Common.Entities.Account>()
                .ForMember(a => a.Id, o => o.MapFrom(a => a.ExternalId));
        }
    }
}
