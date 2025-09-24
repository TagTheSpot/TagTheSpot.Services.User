using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.Shared.Messaging.Users;
using TagTheSpot.Services.User.Application.Identity;
using TagTheSpot.Services.User.Domain.Enums;
using TagTheSpot.Services.User.WebAPI.Options;

namespace TagTheSpot.Services.User.WebAPI.Extensions
{
    public static class UserExtensions
    {
        public async static Task CreateSuperUserIfNotCreated(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            var superUserSettings = scope.ServiceProvider.GetRequiredService<IOptions<SuperUserSettings>>()
                .Value;

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var foundUser = await userManager.FindByEmailAsync(superUserSettings.Email);

            if (foundUser is null)
            {
                var superUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = superUserSettings.Email,
                    UserName = superUserSettings.Email,
                    EmailConfirmed = true,
                    Role = Role.Owner
                };

                var createResult = await userManager.CreateAsync(superUser, superUserSettings.Password);

                if (!createResult.Succeeded)
                {
                    var errorMessage = string.Join("; ", createResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException($"Failed to create a super user with email: {superUserSettings.Email}. Error message: {errorMessage}");
                }

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new UserCreatedEvent(
                    UserId: Guid.Parse(superUser.Id),
                    Email: superUser.Email,
                    Role: superUser.Role.ToString()));
            }
        }
    }
}
