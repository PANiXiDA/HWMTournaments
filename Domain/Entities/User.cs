using Entities.Core;

namespace Entities;

public sealed class User : BaseEntity<int>
{
    public User(
        int id) : base(id)
    {
    }
}
