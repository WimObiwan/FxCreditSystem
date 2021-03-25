using AutoMapper;
using FxCreditSystem.API.DTO;

namespace FxCreditSystem.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Common.Entities.UserIdentity, UserIdentityResponse>();
            CreateMap<Common.Entities.AccountUser, AccountUserResponse>();
        }
    }
}
