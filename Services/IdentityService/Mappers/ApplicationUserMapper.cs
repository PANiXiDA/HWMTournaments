using AutoMapper;

using Gen.IdentityService.Enums;

using Google.Protobuf.WellKnownTypes;

using ApplicationUserDb = DAL.DbModels.Identity.ApplicationUser;
using ApplicationUserProto = Gen.IdentityService.Entities.ApplicationUser;

namespace IdentityService.Mappers;

public class ApplicationUserMapper : Profile
{
    public ApplicationUserMapper()
    {
        CreateMap<ApplicationUserProto, ApplicationUserDb>()
            .ForMember(db => db.UserName, configuration => configuration.MapFrom(proto => proto.Name))
            .ForMember(db => db.PasswordHash, configuration => configuration.MapFrom(proto => proto.Password))
            .ForMember(db => db.Roles, configuration => configuration.Ignore())
            .ForMember(db => db.LockoutEnd,
                configuration => configuration.MapFrom(proto => proto.LockoutEnd != null ? proto.LockoutEnd.ToDateTimeOffset() : (DateTimeOffset?)null))

            .ReverseMap()

            .ForMember(proto => proto.LockoutEnd,
                configuration => configuration.MapFrom(db => db.LockoutEnd.HasValue ? Timestamp.FromDateTimeOffset(db.LockoutEnd.Value) : null))
            .ForMember(proto => proto.Roles,
                configuration => configuration.MapFrom(db => db.Roles.Select(role => (ApplicationUserRole)role.RoleId)));
    }
}
