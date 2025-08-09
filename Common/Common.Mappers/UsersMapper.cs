namespace Common.Mappers;

using System.Collections.Generic;
using System.Linq;

using DTOs.Models;
using DTOs.Requests;

using Entities;

using Gen.IdentityService.Entities;

using GrpcRole = Gen.IdentityService.Enums.ApplicationUserRole;
using LocalRole = Enums.ApplicationUserRole;

public static class UsersMapper
{
    public static UserDTO EntityToDto(this User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Name = user.ApplicationUser?.Name ?? string.Empty,
            Password = user.ApplicationUser?.Password ?? string.Empty,
            Email = user.ApplicationUser?.Email ?? string.Empty,
            EmailConfirmed = user.ApplicationUser?.EmailConfirmed ?? false,
            PhoneNumber = user.ApplicationUser?.PhoneNumber ?? string.Empty,
            PhoneNumberConfirmed = user.ApplicationUser?.PhoneNumberConfirmed ?? false,
            Roles = user.ApplicationUser?.Roles?.Where(role => Enum.IsDefined(typeof(LocalRole), (int)role)).Select(role => (LocalRole)(int)role).ToList() ?? new List<LocalRole>(),
        };
    }

    public static User DTOToEntity(this UserDTO dto)
    {
        var entity = new User(
            id: dto.Id,
            applicationUserId: 0)
        {
            ApplicationUser = new ApplicationUser()
            {
                Id = dto.Id,
                Name = dto.Name,
                Password = dto.Password,
                Email = dto.Email,
                EmailConfirmed = dto.EmailConfirmed,
                PhoneNumber = dto.PhoneNumber,
                PhoneNumberConfirmed = dto.PhoneNumberConfirmed,
            }
        };

        if (dto.Roles != null)
        {
            var grpcRoles = dto.Roles.Where(role => Enum.IsDefined(typeof(GrpcRole), (int)role)).Select(role => (GrpcRole)(int)role);
            entity.ApplicationUser.Roles.AddRange(grpcRoles);
        }

        return entity;
    }

    public static User RegistrationRequestToEntity(this RegistrationRequest request)
    {
        var entity = new User(
            id: 0,
            applicationUserId: 0)
        {
            ApplicationUser = new ApplicationUser()
            {
                Id = 0,
                Name = request.Name,
                Password = request.Password,
                Email = request.Email,
                EmailConfirmed = false,
                PhoneNumber = string.Empty,
                PhoneNumberConfirmed = false,
            }
        };

        if (Enum.IsDefined(typeof(GrpcRole), (int)LocalRole.Client))
        {
            entity.ApplicationUser.Roles.Add((GrpcRole)(int)LocalRole.Client);
        }

        return entity;
    }

    public static IEnumerable<UserDTO> FromEntityToDTOList(this IEnumerable<User> list) => list.Select(EntityToDto).ToList();

    public static IEnumerable<User> FromDTOToEntityList(this IEnumerable<UserDTO> list) => list.Select(DTOToEntity).ToList();
}
