using DAL.DbModels.Core;
using DAL.DbModels.Identity;

namespace DAL.DbModels;

public sealed class User : BaseDbModel<int>
{
    public int ApplicationUserId { get; set; }

    public ApplicationUser? ApplicationUser { get; set; }
}
