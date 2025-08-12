using System.Net;

using BL.Interfaces;

using Common.Configuration.Mail.Interfaces;
using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using DAL.Interfaces;

using Gen.IdentityService.ApplicationUserService;

using Microsoft.Extensions.Configuration;

using static Common.Constants.IdentityServiceConstants;

using User = Entities.User;

namespace BL.Implementations;

public sealed class UsersBL : IUsersBL
{
    private readonly ApplicationUserService.ApplicationUserServiceClient _applicationUserServiceClient;

    private readonly IEmailSender _emailSender;

    private readonly IUsersDAL _usersDAL;

    private readonly string _confirmEmailUrl;
    private readonly string _changePasswordUrl;

    public UsersBL(
        IConfiguration configuration,
        ApplicationUserService.ApplicationUserServiceClient applicationUserServiceClient,
        IEmailSender emailSender,
        IUsersDAL usersDAL)
    {
        _applicationUserServiceClient = applicationUserServiceClient;

        _emailSender = emailSender;

        _usersDAL = usersDAL;

        _confirmEmailUrl = configuration.GetValue<string>("ConfirmEmailUrl") ?? throw new ArgumentException("ConfirmEmailUrl не задано в appsettings.json.");
        _changePasswordUrl = configuration.GetValue<string>("ChangePasswordUrl") ?? throw new ArgumentException("ChangePasswordUrl не задано в appsettings.json.");
    }

    public Task<User> GetAsync(int id, UsersConvertParams? convertParams = null)
    {
        return _usersDAL.GetAsync(id, convertParams);
    }

    public Task<SearchResult<User>> GetAsync(UsersSearchParams searchParams, UsersConvertParams? convertParams = null)
    {
        return _usersDAL.GetAsync(searchParams, convertParams);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _usersDAL.ExistsAsync(id);
    }

    public Task<bool> ExistsAsync(UsersSearchParams searchParams)
    {
        return _usersDAL.ExistsAsync(searchParams);
    }

    public async Task<int> AddOrUpdateAsync(User entity)
    {
        if (entity.Id == 0)
        {
            await CreateAsync(entity);
        }
        else
        {
            await UpdateAsync(entity);
        }

        return entity.Id;
    }

    public async Task<IList<int>> AddOrUpdateAsync(IList<User> entities)
    {
        foreach (var entity in entities)
        {
            await AddOrUpdateAsync(entity);
        }

        return entities.Select(item => item.Id).ToList();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        await _applicationUserServiceClient.DeleteAsync(new DeleteApplicationUserRequest { Id = entity.ApplicationUserId });
        return await _usersDAL.DeleteAsync(id);
    }

    public async Task<bool> DeleteAsync(List<int> ids)
    {
        foreach (var id in ids)
        {
            await DeleteAsync(id);
        }

        return true;
    }

    public async Task SendEmailConfirmationLinkAsync(string email)
    {
        var response = await _applicationUserServiceClient.GetEmailConfirmationTokenAsync(new GetEmailConfirmationTokenRequest { Email = email });
        await SendEmailConfirmationLinkToEmailAsync(email, response.Token);
    }

    public async Task ConfirmEmailAsync(string email, string token)
    {
        await _applicationUserServiceClient.ConfirmEmailAsync(new ConfirmEmailRequest { Email = email, Token = token });
    }

    public async Task SendPasswordResetLinkAsync(string email)
    {
        var response = await _applicationUserServiceClient.GetPasswordResetTokenAsync(new GetPasswordResetTokenRequest { Email = email });
        await SendPasswordResetLinkToEmailAsync(email, response.Token);
    }

    public async Task ResetPasswordAsync(string email, string token, string newPassword)
    {
        await _applicationUserServiceClient.ResetPasswordAsync(new ResetPasswordRequest { Email = email, Token = token, NewPassword = newPassword });
    }

    private async Task CreateAsync(User entity)
    {
        var grpcResponse = await _applicationUserServiceClient.CreateAsync(entity.ApplicationUser);
        entity.ApplicationUserId = grpcResponse.Id;

        entity.Id = await _usersDAL.AddOrUpdateAsync(entity);

        await _applicationUserServiceClient.AddClaimAsync(
            new AddClaimRequest
            {
                Id = entity.ApplicationUserId,
                Type = CustomJwtClaimTypes.UserId,
                Value = entity.Id.ToString()
            });

        await SendEmailConfirmationLinkToEmailAsync(entity.ApplicationUser!.Email, grpcResponse.EmailConfirmationToken);
    }

    private async Task UpdateAsync(User entity)
    {
        await _applicationUserServiceClient.UpdateAsync(entity.ApplicationUser);
        await _usersDAL.AddOrUpdateAsync(entity);
    }

    private async Task SendEmailConfirmationLinkToEmailAsync(string email, string token)
    {
        var url = $"{_confirmEmailUrl}?email={email}&token={WebUtility.UrlEncode(token)}";

        const string subject = "Подтверждение регистрации";
        var body = $@"
            <p>Здравствуйте!</p>
            <p>Для подтверждения почты перейдите по <a href=""{url}"">ссылке</a>.</p>
            <p>Если вы не регистрировались, просто игнорируйте это письмо.</p>";


        await _emailSender.SendAsync(
            recipients: new[] { email },
            subject: subject,
            htmlBody: body);
    }

    private async Task SendPasswordResetLinkToEmailAsync(string email, string token)
    {
        var url = $"{_changePasswordUrl}?email={email}&token={WebUtility.UrlEncode(token)}";

        const string subject = "Смена пароля";
        var body = $@"
            <p>Здравствуйте!</p>
            <p>Для смены пароля перейдите по <a href=""{url}"">ссылке</a>.</p>
            <p>Если вы не запрашивали смену пароля, просто игнорируйте это письмо.</p>";


        await _emailSender.SendAsync(
            recipients: new[] { email },
            subject: subject,
            htmlBody: body);
    }
}