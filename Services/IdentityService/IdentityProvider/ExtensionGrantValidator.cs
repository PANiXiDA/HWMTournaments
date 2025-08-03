using Common.Constants;

using DAL.DbModels.Identity;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

using Microsoft.AspNetCore.Identity;

using static Common.Constants.IdentityServiceConstants;

using ApplicationUserRole = Gen.IdentityService.Enums.ApplicationUserRole;
using GrantTypes = Common.Constants.IdentityServiceConstants.GrantTypes;

namespace IdentityService.IdentityProvider;

public class ExtensionGrantValidator : IExtensionGrantValidator
{
    private readonly ILogger<ExtensionGrantValidator> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExtensionGrantValidator(
        ILogger<ExtensionGrantValidator> logger,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public string GrantType => GrantTypes.Login;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var login = context.Request.Raw.Get(RequestParameters.Login);
        var password = context.Request.Raw.Get(RequestParameters.Password);

        if (string.IsNullOrEmpty(password))
        {
            _logger.LogWarning(ErrorMessagesConstants.EmptyPassword);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, ErrorMessagesConstants.EmptyPassword);
            return;
        }

        ApplicationUser? applicationUser = null;
        if (!string.IsNullOrEmpty(login))
        {
            applicationUser = await _userManager.FindByNameAsync(login)
                ?? await _userManager.FindByLoginAsync(DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName, login);
        }

        if (applicationUser != null && await _userManager.CheckPasswordAsync(applicationUser, password))
        {
            var roles = await _userManager.GetRolesAsync(applicationUser);
            var client = context.Request.Client.ClientId;
            var validationIsTrue = roles.Contains(nameof(ApplicationUserRole.Developer))
                || client == Clients.ReactAdmin && (roles.Contains(nameof(ApplicationUserRole.Admin)));

            if (validationIsTrue)
            {
                context.Result = new GrantValidationResult(subject: applicationUser.Id.ToString(), AuthenticationMethods.Custom);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ErrorMessagesConstants.RoleMismatch);
            }

            return;
        }
        else
        {
            _logger.LogWarning(ErrorMessagesConstants.IncorrentLoginOrPassword);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ErrorMessagesConstants.IncorrentLoginOrPassword);
            return;
        }
    }
}
