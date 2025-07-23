using Microsoft.AspNetCore.Identity;
using TagTheSpot.Services.User.Domain.Enums;

namespace TagTheSpot.Services.User.Application.Identity
{
    public sealed class ApplicationUser : IdentityUser
    {
        public Role Role { get; init; } = Role.RegularUser;
    }
}
