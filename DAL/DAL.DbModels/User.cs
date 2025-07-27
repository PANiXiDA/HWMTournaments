using DAL.DbModels.Core;

namespace DAL.DbModels;

public sealed class User : BaseDbModel<int>
{
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
