using BL.Interfaces.Core;

using Common.ConvertParams;
using Common.SearchParams;

using Entities;

namespace BL.Interfaces;

public interface IUsersBL : ICrudBL<User, int, UsersSearchParams, UsersConvertParams>
{
    Task SendEmailConfirmationLinkAsync(string email);
    Task ConfirmEmailAsync(string email, string token);
    Task SendPasswordResetLinkAsync(string email);
    Task ResetPasswordAsync(string email, string token, string newPassword);
}