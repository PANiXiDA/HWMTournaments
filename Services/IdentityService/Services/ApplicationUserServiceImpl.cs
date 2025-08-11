using System.Security.Claims;

using AutoMapper;

using Gen.IdentityService.ApplicationUserService;
using Gen.IdentityService.Enums;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.Identity;

using ApplicationUserDb = DAL.DbModels.Identity.ApplicationUser;
using ApplicationUserProto = Gen.IdentityService.Entities.ApplicationUser;

namespace IdentityService.Services;

public class ApplicationUserServiceImpl : ApplicationUserService.ApplicationUserServiceBase
{
    private readonly ILogger<ApplicationUserServiceImpl> _logger;

    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUserDb> _userManager;

    public ApplicationUserServiceImpl(
        ILogger<ApplicationUserServiceImpl> logger,
        IMapper mapper,
        UserManager<ApplicationUserDb> userManager)
    {
        _logger = logger;

        _mapper = mapper;
        _userManager = userManager;
    }

    public override async Task<ApplicationUserProto> Get(GetApplicationUserRequest request, ServerCallContext context)
    {
        var applicationUser = (await _userManager.FindByIdAsync(request.Id.ToString()))!;
        return _mapper.Map<ApplicationUserProto>(applicationUser);
    }

    public override async Task<GetEmailConfirmationTokenResponse> GetEmailConfirmationToken(GetEmailConfirmationTokenRequest request, ServerCallContext context)
    {
        var applicationUser = (await _userManager.FindByEmailAsync(request.Email))!;
        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

        return new GetEmailConfirmationTokenResponse()
        {
            Token = emailConfirmationToken,
        };
    }

    public override async Task<CreateApplicationUserResponse> Create(ApplicationUserProto request, ServerCallContext context)
    {
        var applicationUser = _mapper.Map<ApplicationUserDb>(request);

        var result = await _userManager.CreateAsync(applicationUser, request.Password);
        if (!result.Succeeded)
        {
            ThrowIdentityResultError(result.Errors);
        }
        await SynchronizationRolesAsync(applicationUser, request.Roles);

        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

        return new CreateApplicationUserResponse()
        {
            Id = applicationUser.Id,
            EmailConfirmationToken = emailConfirmationToken,
        };
    }

    public override async Task<Empty> AddClaim(AddClaimRequest request, ServerCallContext ctx)
    {
        var applicationUser = (await _userManager.FindByIdAsync(request.Id.ToString()))!;
        await _userManager.AddClaimAsync(applicationUser, new Claim(request.Type, request.Value));
        return new Empty();
    }

    public override async Task<Empty> ConfirmEmail(ConfirmEmailRequest request, ServerCallContext context)
    {
        var applicationUser = (await _userManager.FindByEmailAsync(request.Email))!;
        await _userManager.ConfirmEmailAsync(applicationUser, request.Token);

        return new Empty();
    }

    public override async Task<Empty> Update(ApplicationUserProto request, ServerCallContext context)
    {
        var applicationUser = (await _userManager.FindByIdAsync(request.Id.ToString()))!;

        _mapper.Map(request, applicationUser);
        var result = await _userManager.UpdateAsync(applicationUser);
        if (!result.Succeeded)
        {
            ThrowIdentityResultError(result.Errors);
        }

        if (!await _userManager.CheckPasswordAsync(applicationUser, request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
            result = await _userManager.ResetPasswordAsync(applicationUser, token, request.Password);
            if (!result.Succeeded)
            {
                ThrowIdentityResultError(result.Errors);
            }
        }
        await SynchronizationRolesAsync(applicationUser, request.Roles);

        return new Empty();
    }

    public override async Task<Empty> Delete(DeleteApplicationUserRequest request, ServerCallContext context)
    {
        var applicationUser = (await _userManager.FindByIdAsync(request.Id.ToString()))!;
        await _userManager.DeleteAsync(applicationUser);

        return new Empty();
    }

    private async Task SynchronizationRolesAsync(ApplicationUserDb applicationUser, IEnumerable<ApplicationUserRole> newRoles)
    {
        var currentRoles = await _userManager.GetRolesAsync(applicationUser);
        var targetRoles = newRoles.Select(role => role.ToString()).ToArray();

        var toAdd = targetRoles.Except(currentRoles);
        var toRemove = currentRoles.Except(targetRoles);

        if (toAdd.Any())
        {
            await _userManager.AddToRolesAsync(applicationUser, toAdd);
        }
        if (toRemove.Any())
        {
            await _userManager.RemoveFromRolesAsync(applicationUser, toRemove);
        }
    }

    private static void ThrowIdentityResultError(IEnumerable<IdentityError> errors)
    {
        var message = string.Join(", ", errors.Select(error => error.Description));
        throw new RpcException(new Status(StatusCode.InvalidArgument, message));
    }
}
